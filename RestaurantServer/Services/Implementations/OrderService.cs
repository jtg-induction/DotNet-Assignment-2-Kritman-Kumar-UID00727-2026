using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
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
        private readonly IOrderValidator _orderValidator;
        private readonly IRestaurantValidator _restaurantValidator;

        public OrderService(
            IUnitOfWork unitOfWork,
            IOrderRepository orderRepository,
            IUsersRepository usersRepository,
            IRestaurantRepository restaurantRepository,
            IItemRepository itemRepository,
            IOrderValidator orderValidator,
            IRestaurantValidator restaurantValidator)
        {
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _usersRepository = usersRepository;
            _restaurantRepository = restaurantRepository;
            _itemRepository = itemRepository;
            _orderValidator = orderValidator;
            _restaurantValidator = restaurantValidator;
        }

        public async Task<CreateOrderResponse> PlaceOrderAsync(
            long userId,
            long restaurantId,
            CreateOrderRequest request,
            CancellationToken cancellationToken = default)
        {
            _orderValidator.ValidateOrderRequest(request);

            var consolidatedItems = request.Items
                .GroupBy(item => item.ItemId)
                .Select(g => new OrderItemRequest
                {
                    ItemId = g.Key,
                    Quantity = g.Sum(item => item.Quantity)
                })
                .ToList();

            var sortedItemIds = consolidatedItems
                .Select(item => item.ItemId)
                .OrderBy(id => id)
                .ToList();

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var user = await _usersRepository.GetUserForUpdateAsync(userId, cancellationToken);

                    _orderValidator.ValidateUserForOrder(user);

                    var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId, cancellationToken);

                    _restaurantValidator.ValidateRestaurantExists(restaurant);

                    var lockedItems = await _itemRepository.GetItemsForUpdateAsync(sortedItemIds, cancellationToken);

                    var lockedItemsById = lockedItems.ToDictionary(item => item.Id);

                    _orderValidator.ValidateItemsForOrder(restaurantId, consolidatedItems, lockedItemsById);

                    decimal totalPrice = consolidatedItems.Sum(ci => lockedItemsById[ci.ItemId].Price * ci.Quantity);

                    _orderValidator.ValidateUserBalance(user, totalPrice);

                    user.Balance -= totalPrice;

                    var orderItems = new List<OrderItem>();

                    foreach (var ci in consolidatedItems)
                    {
                        var item = lockedItemsById[ci.ItemId];
                        item.Stock -= ci.Quantity;

                        orderItems.Add(new OrderItem(item,  ci.Quantity));
                    }

                    var order = new Order(restaurantId, userId, totalPrice, request, orderItems);

                    await _orderRepository.Add(order);

                    await _unitOfWork.SaveChangesAsync(cancellationToken);

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
    }
}
