using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestaurantServer.Constants;
using RestaurantServer.Controllers;
using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using RestaurantServer.Services.Interfaces;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;

namespace RestaurantServer.Tests.ControllersTest
{
    [TestClass]
    public class UserControllerTests
    {
        private Mock<IUserUpdateService> _userUpdateServiceMock;
        private Mock<IUserSessionService> _currentUserServiceMock;

        private UserController _controller;

        [TestInitialize]
        public void Setup()
        {
            _userUpdateServiceMock =
                new Mock<IUserUpdateService>();

            _currentUserServiceMock =
                new Mock<IUserSessionService>();

            _controller =
                new UserController(
                    _userUpdateServiceMock.Object,
                    _currentUserServiceMock.Object);

            ConfigureController();
        }

        [TestMethod]
        public async Task UpdateAccount_WithValidUserClaim_ShouldReturnOk()
        {
            var userId = 1L;

            var request = new UpdateAccountRequest
            {
                Name = "Updated User",
                MobileNumber = "9876543210"
            };

            var user = new RestaurantServer.Models.User
            {
                Id = userId,
                Name = "Updated User",
                Email = "user@test.com",
                MobileNumber = "9876543210"
            };

            var expectedResponse =
                new UpdateUserResponse(user);

            _currentUserServiceMock
                .Setup(service =>
                    service.GetUserId())
                .Returns(userId);

            _userUpdateServiceMock
                .Setup(service =>
                    service.UpdateAccountAsync(
                        userId,
                        request,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var result =
                await _controller.UpdateAccount(
                    request,
                    CancellationToken.None);

            var response =
                await result.ExecuteAsync(
                    CancellationToken.None);

            Assert.AreEqual(
                HttpStatusCode.OK,
                response.StatusCode);

            _currentUserServiceMock.Verify(
                service =>
                    service.GetUserId(),
                Times.Once);

            _userUpdateServiceMock.Verify(
                service =>
                    service.UpdateAccountAsync(
                        userId,
                        request,
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [TestMethod]
        public async Task UpdateAccount_WithInvalidUserIdClaim_ShouldReturnUnauthorized()
        {
            var request = new UpdateAccountRequest
            {
                Name = "Updated User",
                MobileNumber = "9876543210"
            };

            _currentUserServiceMock
                .Setup(service =>
                    service.GetUserId())
                .Returns((long?)null);

            var result =
                await _controller.UpdateAccount(
                    request,
                    CancellationToken.None);

            var response =
                await result.ExecuteAsync(
                    CancellationToken.None);

            Assert.AreEqual(
                HttpStatusCode.Unauthorized,
                response.StatusCode);

            _userUpdateServiceMock.Verify(
                service =>
                    service.UpdateAccountAsync(
                        It.IsAny<long>(),
                        It.IsAny<UpdateAccountRequest>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task DeactivateAccount_WithValidUserClaim_ShouldReturnOk()
        {
            var userId = 1L;

            var expectedMessage =
                SuccessMessages.AccountDeactivatedSuccessful;

            _currentUserServiceMock
                .Setup(service =>
                    service.GetUserId())
                .Returns(userId);

            _userUpdateServiceMock
                .Setup(service =>
                    service.DeactivateAccountAsync(
                        userId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedMessage);

            var result =
                await _controller.DeactivateAccount(
                    CancellationToken.None);

            var response =
                await result.ExecuteAsync(
                    CancellationToken.None);

            Assert.AreEqual(
                HttpStatusCode.OK,
                response.StatusCode);

            _currentUserServiceMock.Verify(
                service =>
                    service.GetUserId(),
                Times.Once);

            _userUpdateServiceMock.Verify(
                service =>
                    service.DeactivateAccountAsync(
                        userId,
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [TestMethod]
        public async Task DeactivateAccount_WithoutUserId_ShouldReturnUnauthorized()
        {
            _currentUserServiceMock
                .Setup(service =>
                    service.GetUserId())
                .Returns((long?)null);

            var result =
                await _controller.DeactivateAccount(
                    CancellationToken.None);

            var response =
                await result.ExecuteAsync(
                    CancellationToken.None);

            Assert.AreEqual(
                HttpStatusCode.Unauthorized,
                response.StatusCode);

            _userUpdateServiceMock.Verify(
                service =>
                    service.DeactivateAccountAsync(
                        It.IsAny<long>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        private void ConfigureController()
        {
            var configuration =
                new HttpConfiguration();

            var request =
                new HttpRequestMessage(
                    HttpMethod.Put,
                    "http://localhost/api/user");

            request.Properties[
                HttpPropertyKeys.HttpConfigurationKey] =
                configuration;

            _controller.Request = request;
        }
    }
}
