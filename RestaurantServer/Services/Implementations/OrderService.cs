using RestaurantServer.Constants;
using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using RestaurantServer.Enums;
using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.Services.Interfaces;
using RestaurantServer.Validators.Interfaces;
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
            IUserSessionService userSessionService)
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

        public async Task<OrderResponse> GetOrderDetailsAsync(
            long orderId, 
            CancellationToken cancellationToken = default)
        {
            _orderValidator.ValidateOrderId(orderId);

            var userId = _userSessionService.GetUserId().Value;

            var user = await _usersRepository.GetByIdAsync(userId, cancellationToken);

            _userValidator.IsUserNullOrDeactivated(user, ValidationMessages.UserNotFound);

            var order = await _orderRepository.GetOrderWithItemsByIdAsync(orderId, true, cancellationToken);

            _orderValidator.ValidateOrderExists(order);

            bool isRestaurantOwner = false;

            if (user.Role == (int)UserRole.Owner && order.UserId != user.Id)
            {
                isRestaurantOwner = await _restaurantOwnerRepository.IsOwnerAsync(order.RestaurantId, user.Id, cancellationToken);
            }

            _orderValidator.ValidateOrderAccess(order, user, isRestaurantOwner);

            return new OrderResponse(order);
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
    }
}
