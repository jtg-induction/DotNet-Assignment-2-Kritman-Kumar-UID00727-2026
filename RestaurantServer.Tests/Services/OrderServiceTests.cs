using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestaurantServer.Constants;
using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using RestaurantServer.Enums;
using RestaurantServer.Exceptions;
using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.Services.Implementations;
using RestaurantServer.Services.Interfaces;
using RestaurantServer.Validators.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Tests
{
    [TestClass]
    public class OrderServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IOrderRepository> _orderRepositoryMock;
        private Mock<IUsersRepository> _usersRepositoryMock;
        private Mock<IRestaurantRepository> _restaurantRepositoryMock;
        private Mock<IItemRepository> _itemRepositoryMock;
        private Mock<IRestaurantOwnerRepository> _restaurantOwnerRepositoryMock;
        private Mock<IOrderValidator> _orderValidatorMock;
        private Mock<IRestaurantValidator> _restaurantValidatorMock;
        private Mock<IUserValidator> _userValidatorMock;
        private Mock<IUserSessionService> _userSessionServiceMock;
        private Mock<IRequestValidator> _requestValidatorMock;
        private Mock<ITransaction> _transactionMock;

        private OrderService _orderService;

        [TestInitialize]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _usersRepositoryMock = new Mock<IUsersRepository>();
            _restaurantRepositoryMock = new Mock<IRestaurantRepository>();
            _itemRepositoryMock = new Mock<IItemRepository>();
            _restaurantOwnerRepositoryMock = new Mock<IRestaurantOwnerRepository>();
            _orderValidatorMock = new Mock<IOrderValidator>();
            _restaurantValidatorMock = new Mock<IRestaurantValidator>();
            _userValidatorMock = new Mock<IUserValidator>();
            _userSessionServiceMock = new Mock<IUserSessionService>();
            _requestValidatorMock = new Mock<IRequestValidator>();
            _transactionMock = new Mock<ITransaction>();

            _unitOfWorkMock
                .Setup(x => x.BeginTransaction())
                .Returns(_transactionMock.Object);

            _userSessionServiceMock
                .Setup(x => x.GetUserId())
                .Returns(1);

            _orderService = new OrderService(
                _unitOfWorkMock.Object,
                _orderRepositoryMock.Object,
                _usersRepositoryMock.Object,
                _restaurantRepositoryMock.Object,
                _itemRepositoryMock.Object,
                _restaurantOwnerRepositoryMock.Object,
                _orderValidatorMock.Object,
                _restaurantValidatorMock.Object,
                _userValidatorMock.Object,
                _userSessionServiceMock.Object,
                _requestValidatorMock.Object);
        }

        [TestMethod]
        public async Task PlaceOrderAsync_InvalidRequest_ShouldThrowValidationException()
        {
            var request = new CreateOrderRequest();

            _orderValidatorMock
                .Setup(x => x.ValidateOrderRequest(request))
                .Throws(new ValidationException(ValidationMessages.OrderItemsRequired));

            var exception = await Assert.ThrowsExceptionAsync<ValidationException>(
                () => _orderService.PlaceOrderAsync(1, request));

            Assert.AreEqual(
                ValidationMessages.OrderItemsRequired,
                exception.Message);
        }

        [TestMethod]
        public async Task GetOrderDetailsAsync_OrderNotFound_ShouldThrowValidationException()
        {
            var user = new User
            {
                Id = 1,
                Role = (int)UserRole.Customer,
                IsActive = true
            };

            _usersRepositoryMock
                .Setup(x => x.GetByIdAsync(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _orderRepositoryMock
                .Setup(x => x.GetOrderWithItemsByIdAsync(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order)null);

            _orderValidatorMock
                .Setup(x => x.ValidateOrderExists(null))
                .Throws(new ValidationException(ValidationMessages.OrderNotFound));

            var exception = await Assert.ThrowsExceptionAsync<ValidationException>(
                () => _orderService.GetOrderDetailsAsync(1));

            Assert.AreEqual(
                ValidationMessages.OrderNotFound,
                exception.Message);
        }

        [TestMethod]
        public async Task CancelOrderAsync_OrderNotFound_ShouldThrowValidationException()
        {
            var user = new User
            {
                Id = 1,
                IsActive = true
            };

            _usersRepositoryMock
                .Setup(x => x.GetByIdAsync(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _orderRepositoryMock
                .Setup(x => x.GetOrderForUpdateAsync(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order)null);

            _orderValidatorMock
                .Setup(x => x.ValidateOrderExists(null))
                .Throws(new ValidationException(ValidationMessages.OrderNotFound));

            var exception = await Assert.ThrowsExceptionAsync<ValidationException>(
                () => _orderService.CancelOrderAsync(1));

            Assert.AreEqual(
                ValidationMessages.OrderNotFound,
                exception.Message);

            _transactionMock.Verify(
                x => x.Rollback(),
                Times.Once);
        }

        [TestMethod]
        public async Task FilterOrdersAsync_ValidRequest_ShouldReturnResponse()
        {
            var parameters = new OrderQueryParameters
            {
                PageNumber = 1,
                PageSize = 10
            };

            var orders = new List<OrderResponse>();

            _orderRepositoryMock
                .Setup(x => x.GetFilteredOrders(
                    1,
                    parameters,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((5, orders));

            _orderValidatorMock
                .Setup(x => x.ValidateQueryParameters(parameters))
                .Returns(parameters);

            var result = await _orderService.FilterOrdersAsync(parameters);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Orders);
        }

        [TestMethod]
        public async Task UpdateOrderStatusAsync_UserIsNotRestaurantOwner_ShouldThrowValidationException()
        {
            var request = new UpdateOrderStatusRequest
            {
                Status = OrderStatus.Accepted
            };

            var user = new User
            {
                Id = 1,
                IsActive = true
            };

            var order = new Order
            {
                UserId = 2,
                RestaurantId = 1,
                Status = (int)OrderStatus.Placed
            };

            _usersRepositoryMock
                .Setup(x => x.GetByIdAsync(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _orderRepositoryMock
                .Setup(x => x.GetOrderForUpdateAsync(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            _restaurantOwnerRepositoryMock
                .Setup(x => x.IsOwnerAsync(
                    1,
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var exception = await Assert.ThrowsExceptionAsync<ValidationException>(
                () => _orderService.UpdateOrderStatusAsync(1, request));

            Assert.AreEqual(
                ValidationMessages.RestaurantOwnerRequired,
                exception.Message);

            _transactionMock.Verify(
                x => x.Rollback(),
                Times.Once);
        }
    }
}