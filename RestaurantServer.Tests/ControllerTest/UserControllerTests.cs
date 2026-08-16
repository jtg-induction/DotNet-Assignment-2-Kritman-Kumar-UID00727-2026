using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestaurantServer.Constants;
using RestaurantServer.Controllers;
using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using RestaurantServer.Enums;
using RestaurantServer.Exceptions;
using RestaurantServer.Services.Interfaces;
using RestaurantServer.Validators.Interfaces;
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
        private Mock<IUserValidator> _userValidatorMock;
        private UserController _controller;

        [TestInitialize]
        public void Setup()
        {
            _userUpdateServiceMock =
                new Mock<IUserUpdateService>();

            _userValidatorMock =
                new Mock<IUserValidator>();

            _controller =
                new UserController(
                    _userUpdateServiceMock.Object,
                    _userValidatorMock.Object);

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

            var expectedResponse = new UpdateUserResponse
            {
                UserId = userId,
                Name = "Updated User",
                Email = "user@test.com",
                MobileNumber = "9876543210",
                Message = "Account updated successfully"
            };

            SetAuthenticatedUser(userId);

            _userValidatorMock
                .Setup(validator =>
                    validator.ValidateUserId(
                        userId,
                        userId));

            _userUpdateServiceMock
                .Setup(service =>
                    service.UpdateAccountAsync(
                        userId,
                        request,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var result =
                await _controller.UpdateAccount(
                    userId,
                    request,
                    CancellationToken.None);

            var response =
                await result.ExecuteAsync(
                    CancellationToken.None);

            Assert.AreEqual(
                HttpStatusCode.OK,
                response.StatusCode);

            _userValidatorMock.Verify(
                validator =>
                    validator.ValidateUserId(
                        userId,
                        userId),
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

            SetAuthenticatedUser(
                "invalid-user-id");

            var result =
                await _controller.UpdateAccount(
                    1,
                    request,
                    CancellationToken.None);

            var response =
                await result.ExecuteAsync(
                    CancellationToken.None);

            Assert.AreEqual(
                HttpStatusCode.Unauthorized,
                response.StatusCode);

            _userValidatorMock.Verify(
                validator =>
                    validator.ValidateUserId(
                        It.IsAny<long>(),
                        It.IsAny<long>()),
                Times.Never);

            _userUpdateServiceMock.Verify(
                service =>
                    service.UpdateAccountAsync(
                        It.IsAny<long>(),
                        It.IsAny<UpdateAccountRequest>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task UpdateAccount_WithMismatchedUserId_ShouldThrowValidationException()
        {
            var authenticatedUserId = 1L;
            var requestedUserId = 2L;

            var request = new UpdateAccountRequest
            {
                Name = "Updated User",
                MobileNumber = "9876543210"
            };

            SetAuthenticatedUser(authenticatedUserId);

            _userValidatorMock
                .Setup(validator =>
                    validator.ValidateUserId(
                        authenticatedUserId,
                        requestedUserId))
                .Throws(
                    new ValidationException(
                        ValidationMessages.NotAuthorized));

            var exception =
                await Assert.ThrowsExceptionAsync<ValidationException>(
                    () => _controller.UpdateAccount(
                        requestedUserId,
                        request,
                        CancellationToken.None));

            Assert.AreEqual(
                ValidationMessages.NotAuthorized,
                exception.Message);

            _userValidatorMock.Verify(
                validator =>
                    validator.ValidateUserId(
                        authenticatedUserId,
                        requestedUserId),
                Times.Once);

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

            SetAuthenticatedUser(userId);

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

            _userUpdateServiceMock.Verify(
                service =>
                    service.DeactivateAccountAsync(
                        userId,
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [TestMethod]
        public async Task DeactivateAccount_WithoutPrincipal_ShouldReturnUnauthorized()
        {
            _controller.User = null;

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

        [TestMethod]
        public async Task DeactivateAccount_WithMissingUserIdClaim_ShouldReturnUnauthorized()
        {
            SetAuthenticatedUserWithoutUserIdClaim();

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

        [TestMethod]
        public async Task DeactivateAccount_WithInvalidUserIdClaim_ShouldReturnUnauthorized()
        {
            SetAuthenticatedUser(
                "invalid-user-id");

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
                    new HttpMethod("PATCH"),
                    new Uri("http://localhost/api/users/1"));

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
