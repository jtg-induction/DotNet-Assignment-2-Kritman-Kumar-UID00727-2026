using RestaurantServer.Constants;
using RestaurantServer.DTOs.Requests;
using RestaurantServer.Enums;
using RestaurantServer.Exceptions;
using RestaurantServer.Models;
using RestaurantServer.Validators.Interfaces;
using System;
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


        public void ValidateUserRoleForOrder(User user)
        {
            _userValidator.IsUserNullOrDeactivated(user);

            if (!Enum.IsDefined(typeof(UserRole), user.Role))
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

        public void ValidateOrderId(long orderId)
        {
            if (orderId <= 0)
            {
                throw new ValidationException(ValidationMessages.InvalidOrderId);
            }
        }

        public void ValidateOrderExists(Order order)
        {
            if (order == null)
            {
                throw new ValidationException(ValidationMessages.OrderNotFound);
            }
        }

        public void ValidateOrderAccess(Order order, User user, bool isRestaurantOwner = false)
        {
            if (user.Role == (int)UserRole.Admin)
            {
                return;
            }

            if (user.Role == (int)UserRole.Owner)
            {
                if (order.UserId == user.Id || isRestaurantOwner)
                {
                    return;
                }

                throw new ValidationException(ValidationMessages.NotAuthorized);
            }

            if (user.Role == (int)UserRole.Customer)
            {
                if (order.UserId == user.Id)
                {
                    return;
                }

                throw new ValidationException(ValidationMessages.NotAuthorized);
            }

            throw new ValidationException(ValidationMessages.NotAuthorized);
        }

        public void ValidateOrderOwnership(Order order, long userId)
        {
            if (order.UserId != userId)
            {
                throw new ValidationException(ValidationMessages.NotAuthorized);
            }
        }

        public void ValidateOrderStatusForCancellation(Order order)
        {
            if (order.Status != (int)OrderStatus.Placed && order.Status != (int)OrderStatus.Accepted)
            {
                throw new ValidationException(ValidationMessages.OrderCannotBeCancelled);
            }
        }

        /// <summary>
        /// Validates the order query parameters and sets default values for invalid pagination values.
        /// </summary>
        /// <param name="orderQueryParameters">The order query parameters.</param>
        /// <returns>The validated order query parameters.</returns>
        public OrderQueryParameters ValidateQueryParameters(OrderQueryParameters orderQueryParameters)
        {
            orderQueryParameters = orderQueryParameters ?? new OrderQueryParameters();

            if (orderQueryParameters.PageNumber < 1)
            {
                orderQueryParameters.PageNumber = 1;
            }

            if (orderQueryParameters.PageSize < 1)
            {
                orderQueryParameters.PageSize = 10;
            }

            return orderQueryParameters;
        }

        /// <summary>
        /// Validates whether the specified order status is valid.
        /// </summary>
        /// <param name="status">The order status to validate.</param>
        public void ValidateOrderStatus(OrderStatus status)
        {
            if (!System.Enum.IsDefined(typeof(OrderStatus), status))
            {
                throw new ValidationException(
                    ValidationMessages.InvalidOrderStatus);
            }
        }

        /// <summary>
        /// Validates whether the specified order status is valid.
        /// </summary>
        /// <param name="status">The order status to validate.</param>
        public void ValidateOrderStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            bool isValidTransition = (currentStatus == OrderStatus.Placed
                && (newStatus == OrderStatus.Accepted || newStatus == OrderStatus.Rejected))
                || (currentStatus == OrderStatus.Accepted && newStatus == OrderStatus.Dispatched)
                || (currentStatus == OrderStatus.Dispatched && newStatus == OrderStatus.Delivered);

            if (!isValidTransition)
            {
                throw new ValidationException(
                    ValidationMessages.InvalidOrderStatusTransition);
            }
        }
    }
}
