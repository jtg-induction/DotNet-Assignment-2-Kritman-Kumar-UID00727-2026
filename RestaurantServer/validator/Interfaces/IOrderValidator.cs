using RestaurantServer.DTOs.Requests;
using RestaurantServer.Models;
using System.Collections.Generic;

namespace RestaurantServer.Validators.Interfaces
{
    public interface IOrderValidator
    {
        void ValidateOrderRequest(CreateOrderRequest request);
        void ValidateUserForOrder(User user);
        void ValidateItemsForOrder(
            long restaurantId,
            List<OrderItemRequest> consolidatedItems,
            Dictionary<long, Item> lockedItemsById);
        void ValidateUserBalance(User user, decimal totalPrice);
    }
}
