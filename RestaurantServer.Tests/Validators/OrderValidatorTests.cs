using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestaurantServer.Constants;
using RestaurantServer.DTOs.Requests;
using RestaurantServer.Enums;
using RestaurantServer.Exceptions;
using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.Validators.Implementations;
using System.Collections.Generic;

namespace RestaurantServer.Tests.Validators
{
    [TestClass]
    public class OrderValidatorTests
    {
        private OrderValidator _orderValidator;

        [TestInitialize]
        public void Setup()
        {
            var usersRepository = new Mock<IUsersRepository>();
            var userValidator = new UserValidator(usersRepository.Object);
            _orderValidator = new OrderValidator(userValidator);
        }

        [TestMethod]
        public void ValidateOrderRequest_EmptyItems_ThrowsValidationException()
        {
            var request = new CreateOrderRequest
            {
                Items = new List<OrderItemRequest>()
            };

            var exception = Assert.ThrowsException<ValidationException>(
                () => _orderValidator.ValidateOrderRequest(request));

            Assert.AreEqual(
                ValidationMessages.OrderItemsRequired,
                exception.Message);
        }

        [TestMethod]
        public void ValidateOrderRequest_InvalidItemId_ThrowsValidationException()
        {
            var request = new CreateOrderRequest
            {
                Items = new List<OrderItemRequest>
                {
                    new OrderItemRequest
                    {
                        ItemId = 0,
                        Quantity = 1
                    }
                }
            };

            var exception = Assert.ThrowsException<ValidationException>(
                () => _orderValidator.ValidateOrderRequest(request));

            Assert.AreEqual(
                ValidationMessages.InvalidItemId,
                exception.Message);
        }

        [TestMethod]
        public void ValidateItemsForOrder_ItemNotFound_ThrowsValidationException()
        {
            var requestedItems = new List<OrderItemRequest>
            {
                new OrderItemRequest
                {
                    ItemId = 1,
                    Quantity = 1
                }
            };

            var items = new Dictionary<long, Item>();

            var exception = Assert.ThrowsException<ValidationException>(
                () => _orderValidator.ValidateItemsForOrder(
                    1,
                    requestedItems,
                    items));

            Assert.AreEqual(
                ValidationMessages.ItemNotFound,
                exception.Message);
        }

        [TestMethod]
        public void ValidateOrderAccess_CustomerDoesNotOwnOrder_ThrowsValidationException()
        {
            var user = new User
            {
                Id = 1,
                Role = (int)UserRole.Customer
            };

            var order = new Order
            {
                UserId = 2
            };

            var exception = Assert.ThrowsException<ValidationException>(
                () => _orderValidator.ValidateOrderAccess(order, user));

            Assert.AreEqual(
                ValidationMessages.NotAuthorized,
                exception.Message);
        }

        [TestMethod]
        public void ValidateOrderStatusForCancellation_DispatchedOrder_ThrowsValidationException()
        {
            var order = new Order
            {
                Status = (int)OrderStatus.Dispatched
            };

            var exception = Assert.ThrowsException<ValidationException>(
                () => _orderValidator.ValidateOrderStatusForCancellation(order));

            Assert.AreEqual(
                ValidationMessages.OrderCannotBeCancelled,
                exception.Message);
        }

        [TestMethod]
        public void ValidateOrderStatusTransition_InvalidTransition_ThrowsValidationException()
        {
            var exception = Assert.ThrowsException<ValidationException>(
                () => _orderValidator.ValidateOrderStatusTransition(
                    OrderStatus.Placed,
                    OrderStatus.Delivered));

            Assert.AreEqual(
                ValidationMessages.InvalidOrderStatusTransition,
                exception.Message);
        }
    }
}
