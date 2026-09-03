using RestaurantServer.Constants;
using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using RestaurantServer.Enums;
using RestaurantServer.Exceptions;
using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.Services.Interfaces;
using RestaurantServer.Validators.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IRestaurantOwnerRepository _restaurantOwnerRepository;
        private readonly IOrderValidator _orderValidator;
        private readonly IRestaurantValidator _restaurantValidator;
        private readonly IUserValidator _userValidator;
        private readonly IUserSessionService _userSessionService;
        private readonly IRequestValidator _requestValidator;

        public OrderService(
            IUnitOfWork unitOfWork,
            IOrderRepository orderRepository,
            IUsersRepository usersRepository,
            IRestaurantRepository restaurantRepository,
            IItemRepository itemRepository,
            IRestaurantOwnerRepository restaurantOwnerRepository,
            IOrderValidator orderValidator,
            IRestaurantValidator restaurantValidator,
            IUserValidator userValidator,
            IUserSessionService userSessionService,
            IRequestValidator requestValidator)
        {
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _usersRepository = usersRepository;
            _restaurantRepository = restaurantRepository;
            _itemRepository = itemRepository;
            _restaurantOwnerRepository = restaurantOwnerRepository;
            _orderValidator = orderValidator;
            _restaurantValidator = restaurantValidator;
            _userValidator = userValidator;
            _userSessionService = userSessionService;
            _requestValidator = requestValidator;
        }

        public async Task<CreateOrderResponse> PlaceOrderAsync(
            long restaurantId,
            CreateOrderRequest request,
            CancellationToken cancellationToken = default)
        {
            _orderValidator.ValidateOrderRequest(request);

            var userId = _userSessionService.GetUserId().Value;

            var groupedItems = request.Items
                .GroupBy(item => item.ItemId)
                .Select(g => new OrderItemRequest
                {
                    ItemId = g.Key,
                    Quantity = g.Sum(item => item.Quantity)
                })
                .ToList();

            var sortedItemIds = groupedItems
                .Select(item => item.ItemId)
                .OrderBy(id => id).ToList();

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var user = await _usersRepository.GetUserForUpdateAsync(userId, cancellationToken);

                    _orderValidator.ValidateUserRoleForOrder(user);

                    var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId, cancellationToken);

                    _restaurantValidator.ValidateRestaurantExists(restaurant);

                    var lockedItems = await _itemRepository.GetItemsForUpdateAsync(sortedItemIds, cancellationToken);

                    var lockedItemsById = lockedItems.ToDictionary(item => item.Id);

                    _orderValidator.ValidateItemsForOrder(restaurantId, groupedItems, lockedItemsById);

                    decimal totalPrice = groupedItems.Sum(item => lockedItemsById[item.ItemId].Price * item.Quantity);

                    _userValidator.ValidateUserBalance(user, totalPrice);

                    user.Balance -= totalPrice;

                    var orderItems = new List<OrderItem>();

                    foreach (var groupedItem in groupedItems)
                    {
                        var item = lockedItemsById[groupedItem.ItemId];
                        item.Stock -= groupedItem.Quantity;

                        orderItems.Add(new OrderItem(item, groupedItem.Quantity));
                    }

                    var order = new Order(restaurantId, userId, totalPrice, request, orderItems);

                    await _orderRepository.Add(order);

                    await _unitOfWork.SaveChangesAsync(null, cancellationToken);

                    transaction.Commit();

                    return new CreateOrderResponse(order);
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task<OrderDetailsResponse> GetOrderDetailsAsync(
            long orderId,
            CancellationToken cancellationToken = default)
        {
            _orderValidator.ValidateOrderId(orderId);

            var userId = _userSessionService.GetUserId().Value;

            var user = await _usersRepository.GetByIdAsync(userId, cancellationToken);

            _userValidator.IsUserNullOrDeactivated(user, ValidationMessages.UserNotFound);

            var order = await _orderRepository.GetOrderWithItemsByIdAsync(orderId, cancellationToken);

            _orderValidator.ValidateOrderExists(order);

            bool isRestaurantOwner = false;

            if (user.Role == (int)UserRole.Owner && order.UserId != user.Id)
            {
                isRestaurantOwner = await _restaurantOwnerRepository.IsOwnerAsync(order.RestaurantId, user.Id, cancellationToken);
            }

            _orderValidator.ValidateOrderAccess(order, user, isRestaurantOwner);

            return new OrderDetailsResponse(order);
        }

        public async Task<CancelOrderResponse> CancelOrderAsync(long orderId,
            CancellationToken cancellationToken = default)
        {
            _orderValidator.ValidateOrderId(orderId);

            var userId = _userSessionService.GetUserId().Value;

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var user = await _usersRepository.GetByIdAsync(userId, cancellationToken);

                    _userValidator.IsUserNullOrDeactivated(user, ValidationMessages.UserNotFound);

                    var order = await _orderRepository.GetOrderForUpdateAsync(orderId, cancellationToken);

                    _orderValidator.ValidateOrderExists(order);

                    _orderValidator.ValidateOrderOwnership(order, userId);

                    _orderValidator.ValidateOrderStatusForCancellation(order);

                    order.Status = (int)OrderStatus.Cancelled;
                    user.Balance += order.TotalPrice;

                    await _unitOfWork.SaveChangesAsync(null, cancellationToken);

                    transaction.Commit();

                    return new CancelOrderResponse(order);
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Filters orders based on the provided query parameters and returns the results with pagination.
        /// </summary>
        /// <param name="orderQueryParameters">The parameters used to filter and paginate the orders.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A filtered and paginated list of orders.</returns>
        public async Task<FilterOrdersResponse> FilterOrdersAsync(OrderQueryParameters orderQueryParameters,
            CancellationToken cancellationToken = default)
        {
            var userId = _userSessionService.GetUserId().Value;

            orderQueryParameters = _orderValidator.ValidateQueryParameters(orderQueryParameters);

            var filterOrders = await _orderRepository.GetFilteredOrders(userId, orderQueryParameters, cancellationToken);

            var paginationResponse = new PaginatedResponse(orderQueryParameters.PageNumber,
                orderQueryParameters.PageSize, filterOrders.TotalRecords);

            return new FilterOrdersResponse(paginationResponse, filterOrders.Orders);
        }

        /// <summary>
        /// Updates the status of an existing order.
        /// </summary>
        /// <param name="orderId">The unique identifier of the order.</param>
        /// <param name="request">The request containing the new order status.</param>
        /// <param name="cancellationToken">A token used to cancel the operation.</param>
        /// <returns>The updated order status response.</returns>
        public async Task<UpdateOrderStatusResponse> UpdateOrderStatusAsync(long orderId,
            UpdateOrderStatusRequest request, CancellationToken cancellationToken = default)
        {

            _requestValidator.IsRequestNull(request);

            _orderValidator.ValidateOrderId(orderId);
            _orderValidator.ValidateOrderStatus(request.Status);

            var userId = _userSessionService.GetUserId().Value;

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var user = await _usersRepository.GetByIdAsync(userId, cancellationToken);

                    _userValidator.IsUserNullOrDeactivated(
                        user, ValidationMessages.UserNotFound);

                    var order = await _orderRepository.GetOrderForUpdateAsync(
                        orderId, cancellationToken);

                    _orderValidator.ValidateOrderExists(order);

                    var isRestaurantOwner = await _restaurantOwnerRepository.IsOwnerAsync(
                            order.RestaurantId, userId, cancellationToken);

                    if (!isRestaurantOwner)
                    {
                        throw new ValidationException(ValidationMessages.RestaurantOwnerRequired);
                    }

                    _orderValidator.ValidateOrderStatusTransition((OrderStatus)order.Status, request.Status);

                    order.Status = (int)request.Status;

                    await _unitOfWork.SaveChangesAsync(null, cancellationToken);

                    transaction.Commit();

                    return new UpdateOrderStatusResponse(order,
                        SuccessMessages.OrderStatusUpdatedSuccessfully);
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
