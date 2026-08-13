using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestaurantServer.Controllers;
using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using RestaurantServer.Enums;
using RestaurantServer.Services.Interfaces;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
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
        private UserController _controller;

        [TestInitialize]
        public void Setup()
        {
            _userUpdateServiceMock =
                new Mock<IUserUpdateService>();

            _controller =
                new UserController(
                    _userUpdateServiceMock.Object);

            ConfigureController();
        }

        [TestMethod]
        public async Task UpdateAccount_WithValidUserClaim_ShouldReturnOk()
        {
            var request = new UpdateAccountRequest
            {
                Name = "Updated User",
                MobileNumber = "9876543210"
            };

            var expectedResponse = new UpdateUserResponse
            {
                UserId = 1,
                Name = "Updated User",
                Email = "user@test.com",
                MobileNumber = "9876543210",
                Message = "Account updated successfully"
            };

            _userUpdateServiceMock
                .Setup(service =>
                    service.UpdateAccountAsync(
                        1,
                        request))
                .ReturnsAsync(expectedResponse);

            SetAuthenticatedUser(1);

            var result =
                await _controller.UpdateAccount(request);

            var response =
                await result.ExecuteAsync(
                    CancellationToken.None);

            Assert.AreEqual(
                HttpStatusCode.OK,
                response.StatusCode);

            _userUpdateServiceMock.Verify(
                service =>
                    service.UpdateAccountAsync(
                        1,
                        request),
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

            SetAuthenticatedUser(
                "invalid-user-id");

            var result =
                await _controller.UpdateAccount(request);

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
                        It.IsAny<UpdateAccountRequest>()),
                Times.Never);
        }

        [TestMethod]
        public async Task DeactivateAccount_WithValidUserClaim_ShouldReturnOk()
        {
            var expectedMessage =
                "Account deactivated successfully";

            _userUpdateServiceMock
                .Setup(service =>
                    service.DeactivateAccountAsync(1))
                .ReturnsAsync(expectedMessage);

            SetAuthenticatedUser(1);

            var result =
                await _controller.DeactivateAccount();

            var response =
                await result.ExecuteAsync(
                    CancellationToken.None);

            Assert.AreEqual(
                HttpStatusCode.OK,
                response.StatusCode);

            _userUpdateServiceMock.Verify(
                service =>
                    service.DeactivateAccountAsync(1),
                Times.Once);
        }

        [TestMethod]
        public async Task DeactivateAccount_WithoutPrincipal_ShouldReturnUnauthorized()
        {
            _controller.User = null;

            var result =
                await _controller.DeactivateAccount();

            var response =
                await result.ExecuteAsync(
                    CancellationToken.None);

            Assert.AreEqual(
                HttpStatusCode.Unauthorized,
                response.StatusCode);

            _userUpdateServiceMock.Verify(
                service =>
                    service.DeactivateAccountAsync(
                        It.IsAny<long>()),
                Times.Never);
        }

        [TestMethod]
        public async Task DeactivateAccount_WithMissingUserIdClaim_ShouldReturnUnauthorized()
        {
            SetAuthenticatedUserWithoutUserIdClaim();

            var result =
                await _controller.DeactivateAccount();

            var response =
                await result.ExecuteAsync(
                    CancellationToken.None);

            Assert.AreEqual(
                HttpStatusCode.Unauthorized,
                response.StatusCode);

            _userUpdateServiceMock.Verify(
                service =>
                    service.DeactivateAccountAsync(
                        It.IsAny<long>()),
                Times.Never);
        }

        [TestMethod]
        public async Task DeactivateAccount_WithInvalidUserIdClaim_ShouldReturnUnauthorized()
        {
            SetAuthenticatedUser(
                "invalid-user-id");

            var result =
                await _controller.DeactivateAccount();

            var response =
                await result.ExecuteAsync(
                    CancellationToken.None);

            Assert.AreEqual(
                HttpStatusCode.Unauthorized,
                response.StatusCode);

            _userUpdateServiceMock.Verify(
                service =>
                    service.DeactivateAccountAsync(
                        It.IsAny<long>()),
                Times.Never);
        }
        
        private void ConfigureController()
        {
            var configuration =
                new HttpConfiguration();

            var request =
                new HttpRequestMessage(
                    new HttpMethod("PATCH"),
                    new Uri("http://localhost/users/1"));

            request.Properties[
                HttpPropertyKeys.HttpConfigurationKey] =
                configuration;

            _controller.Request = request;
        }

        private void SetAuthenticatedUser(long userId)
        {
            SetAuthenticatedUserClaim(
                userId.ToString());
        }

        private void SetAuthenticatedUser(string userId)
        {
            SetAuthenticatedUserClaim(userId);
        }

        private void SetAuthenticatedUserClaim(
            string userId)
        {
            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    userId),

                new Claim(
                    ClaimTypes.Name,
                    "Test User"),

                new Claim(
                    ClaimTypes.Role,
                    ((int)UserRole.Customer).ToString())
            };

            var identity =
                new ClaimsIdentity(
                    claims,
                    "TestAuthentication");

            _controller.User =
                new ClaimsPrincipal(identity);
        }

        private void SetAuthenticatedUserWithoutUserIdClaim()
        {
            var claims = new[]
            {
                new Claim(
                    ClaimTypes.Name,
                    "Test User"),

                new Claim(
                    ClaimTypes.Role,
                    ((int)UserRole.Customer).ToString())
            };

            var identity =
                new ClaimsIdentity(
                    claims,
                    "TestAuthentication");

            _controller.User =
                new ClaimsPrincipal(identity);
        }
    }
}
