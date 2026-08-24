using RestaurantServer.Constants;
using RestaurantServer.DTOs.Requests;
using RestaurantServer.Enums;
using RestaurantServer.Exceptions;
using RestaurantServer.Models;
using RestaurantServer.Validators.Interfaces;
using System.Collections.Generic;

namespace RestaurantServer.Validators.Implementations
{
    public class OrderValidator : IOrderValidator
    {

        private readonly UserValidator _userValidator;

        public OrderValidator(UserValidator userValidator)
        {
            _userValidator = userValidator;
        }

        public void ValidateOrderRequest(CreateOrderRequest request)
        {
            if (request.Items == null || request.Items.Count == 0)
            {
                throw new ValidationException(ValidationMessages.OrderItemsRequired);
            }

            foreach (var item in request.Items)
            {
                if (item.ItemId <= 0)
                {
                    throw new ValidationException(ValidationMessages.InvalidItemId);
                }

                if (item.Quantity <= 0)
                {
                    throw new ValidationException(ValidationMessages.InvalidQuantity);
                }
            }
        }

        public void ValidateUserForOrder(User user)
        {
            _userValidator.IsUserNullOrDeactivated(user);

            if (user.Role != (int)UserRole.Customer &&
                user.Role != (int)UserRole.Owner &&
                user.Role != (int)UserRole.Admin)
            {
                    throw new ValidationException(ValidationMessages.InvalidRole);
            }
        }

        public void ValidateItemsForOrder(
            long restaurantId,
            List<OrderItemRequest> consolidatedItems,
            Dictionary<long, Item> lockedItemsById)
        {
            foreach (var requestedItem in consolidatedItems)
            {
                if (!lockedItemsById.TryGetValue(requestedItem.ItemId, out var item))
                {
                    throw new ValidationException(ValidationMessages.ItemNotFound);
                }

                if (item.IsDeleted)
                {
                    throw new ValidationException(ValidationMessages.ItemNotAvailable);
                }

                if (item.RestaurantId != restaurantId)
                {
                    throw new ValidationException(ValidationMessages.ItemDoesNotBelongToRestaurant);
                }

                if (item.Stock < requestedItem.Quantity)
                {
                    throw new ValidationException(ValidationMessages.InsufficientStock);
                }
            }
        }

        public void ValidateUserBalance(User user, decimal totalPrice)
        {
            if (user.Balance < totalPrice)
            {
                throw new ValidationException(ValidationMessages.InsufficientBalance);
            }
        }
    }
}
