using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestaurantServer.Constants;
using RestaurantServer.Controllers;
using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using RestaurantServer.Enums;
using RestaurantServer.Exceptions;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;

namespace RestaurantServer.Tests.ControllersTest
{
    [TestClass]
    public class AuthControllerTests
    {
        private Mock<IAuthService> _authServiceMock;
        private AuthController _controller;

        [TestInitialize]
        public void Setup()
        {
            _authServiceMock = new Mock<IAuthService>();

            _controller =
                new AuthController(
                    _authServiceMock.Object);

            ConfigureController();
        }

        [TestMethod]
        public async Task Signup_ShouldReturnCreated()
        {
            var request = new SignupRequest
            {
                Name = "Test User",
                Email = "test@test.com",
                Password = "Password@123"
            };

            var expectedResponse = new SignupResponse
            {
                UserId = 1,
                Name = "Test User",
                Email = "test@test.com",
                Message = "Signup successful"
            };

            _authServiceMock
                .Setup(service =>
                    service.SignupAsync(request))
                .ReturnsAsync(expectedResponse);

            var result =
                await _controller.Signup(request);

            var response =
                await result.ExecuteAsync(
                    CancellationToken.None);

            Assert.AreEqual(
                HttpStatusCode.Created,
                response.StatusCode);

            _authServiceMock.Verify(
                service =>
                    service.SignupAsync(request),
                Times.Once);
        }

        [TestMethod]
        public async Task Login_ShouldReturnOk()
        {
            var request = new LoginRequest
            {
                Email = "test@test.com",
                Password = "Password@123"
            };

            var loginResult = CreateLoginResult(
                "access-token",
                "refresh-token");

            _authServiceMock
                .Setup(service =>
                    service.LoginAsync(request))
                .ReturnsAsync(loginResult);

            var result =
                await _controller.Login(request);

            var response =
                await result.ExecuteAsync(
                    CancellationToken.None);

            Assert.AreEqual(
                HttpStatusCode.OK,
                response.StatusCode);

            _authServiceMock.Verify(
                service =>
                    service.LoginAsync(request),
                Times.Once);
        }

        [TestMethod]
        public async Task Login_ShouldSetRefreshTokenCookie()
        {
            var request = new LoginRequest
            {
                Email = "test@test.com",
                Password = "Password@123"
            };

            var loginResult = CreateLoginResult(
                "access-token",
                "refresh-token");

            _authServiceMock
                .Setup(service =>
                    service.LoginAsync(request))
                .ReturnsAsync(loginResult);

            var result =
                await _controller.Login(request);

            var response =
                await result.ExecuteAsync(
                    CancellationToken.None);

            var setCookieHeaders =
                response.Headers
                    .GetValues("Set-Cookie")
                    .ToList();

            Assert.IsTrue(
                setCookieHeaders.Count > 0);

            var cookieHeader =
                setCookieHeaders.First();

            StringAssert.Contains(
                cookieHeader,
                "refreshToken=refresh-token");

            StringAssert.Contains(
                cookieHeader.ToLowerInvariant(),
                "httponly");

            StringAssert.Contains(
                cookieHeader.ToLowerInvariant(),
                "secure");

            StringAssert.Contains(
                cookieHeader.ToLowerInvariant(),
                "path=/auth");
        }

        [TestMethod]
        public async Task Login_ShouldPassRequestToAuthService()
        {
            var request = new LoginRequest
            {
                Email = "user@test.com",
                Password = "Password@123"
            };

            var loginResult = CreateLoginResult(
                "access-token",
                "refresh-token");

            _authServiceMock
                .Setup(service =>
                    service.LoginAsync(request))
                .ReturnsAsync(loginResult);

            await _controller.Login(request);

            _authServiceMock.Verify(
                service =>
                    service.LoginAsync(request),
                Times.Once);
        }

        [TestMethod]
        public async Task Refresh_WithoutCookie_ShouldThrowValidationException()
        {
            var exception =
                await Assert.ThrowsExceptionAsync<ValidationException>(
                    async () =>
                        await _controller.Refresh());

            Assert.AreEqual(
                ValidationMessages.InvalidRefreshToken,
                exception.Message);

            _authServiceMock.Verify(
                service =>
                    service.RefreshTokenAsync(
                        It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public async Task Refresh_WithEmptyCookie_ShouldThrowValidationException()
        {
            AddRefreshTokenCookie("");

            var exception =
                await Assert.ThrowsExceptionAsync<ValidationException>(
                    async () =>
                        await _controller.Refresh());

            Assert.AreEqual(
                ValidationMessages.InvalidRefreshToken,
                exception.Message);

            _authServiceMock.Verify(
                service =>
                    service.RefreshTokenAsync(
                        It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public async Task Refresh_WithValidCookie_ShouldReturnOkAndNewCookie()
        {
            AddRefreshTokenCookie(
                "old-refresh-token");

            var refreshResult = CreateLoginResult(
                "new-access-token",
                "new-refresh-token");

            _authServiceMock
                .Setup(service =>
                    service.RefreshTokenAsync(
                        "old-refresh-token"))
                .ReturnsAsync(refreshResult);

            var result =
                await _controller.Refresh();

            var response =
                await result.ExecuteAsync(
                    CancellationToken.None);

            Assert.AreEqual(
                HttpStatusCode.OK,
                response.StatusCode);

            _authServiceMock.Verify(
                service =>
                    service.RefreshTokenAsync(
                        "old-refresh-token"),
                Times.Once);

            var setCookieHeaders =
                response.Headers
                    .GetValues("Set-Cookie")
                    .ToList();

            Assert.IsTrue(
                setCookieHeaders.Count > 0);

            var cookieHeader =
                setCookieHeaders.First();

            StringAssert.Contains(
                cookieHeader,
                "refreshToken=new-refresh-token");

            StringAssert.Contains(
                cookieHeader.ToLowerInvariant(),
                "httponly");

            StringAssert.Contains(
                cookieHeader.ToLowerInvariant(),
                "secure");

            StringAssert.Contains(
                cookieHeader.ToLowerInvariant(),
                "path=/auth");
        }

        [TestMethod]
        public async Task Logout_WithoutCookie_ShouldThrowValidationException()
        {
            var exception =
                await Assert.ThrowsExceptionAsync<ValidationException>(
                    async () =>
                        await _controller.Logout());

            Assert.AreEqual(
                ValidationMessages.InvalidRefreshToken,
                exception.Message);

            _authServiceMock.Verify(
                service =>
                    service.LogoutAsync(
                        It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public async Task Logout_WithEmptyCookie_ShouldThrowValidationException()
        {
            AddRefreshTokenCookie("");

            var exception =
                await Assert.ThrowsExceptionAsync<ValidationException>(
                    async () =>
                        await _controller.Logout());

            Assert.AreEqual(
                ValidationMessages.InvalidRefreshToken,
                exception.Message);

            _authServiceMock.Verify(
                service =>
                    service.LogoutAsync(
                        It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public async Task Logout_WithValidCookie_ShouldReturnOkAndExpireCookie()
        {
            AddRefreshTokenCookie(
                "refresh-token");

            _authServiceMock
                .Setup(service =>
                    service.LogoutAsync(
                        "refresh-token"))
                .Returns(Task.CompletedTask);

            var result =
                await _controller.Logout();

            var response =
                await result.ExecuteAsync(
                    CancellationToken.None);

            Assert.AreEqual(
                HttpStatusCode.OK,
                response.StatusCode);

            _authServiceMock.Verify(
                service =>
                    service.LogoutAsync(
                        "refresh-token"),
                Times.Once);

            var setCookieHeaders =
                response.Headers
                    .GetValues("Set-Cookie")
                    .ToList();

            Assert.IsTrue(
                setCookieHeaders.Count > 0);

            var cookieHeader =
                setCookieHeaders.First();

            StringAssert.Contains(
                cookieHeader,
                "refreshToken=");

            StringAssert.Contains(
                cookieHeader.ToLowerInvariant(),
                "httponly");

            StringAssert.Contains(
                cookieHeader.ToLowerInvariant(),
                "secure");

            StringAssert.Contains(
                cookieHeader.ToLowerInvariant(),
                "path=/auth");

            StringAssert.Contains(
                cookieHeader.ToLowerInvariant(),
                "expires=");
        }

        private LoginResult CreateLoginResult(
            string accessToken,
            string refreshToken)
        {
            return new LoginResult
            {
                Response = new LoginResponse
                {
                    AccessToken = accessToken,
                    UserId = 1,
                    Name = "Test User",
                    Role = UserRole.Customer,
                    Message = "Login successful"
                },

                RefreshToken = refreshToken
            };
        }

        private void AddRefreshTokenCookie(
            string refreshToken)
        {
            _controller.Request.Headers.Add(
                "Cookie",
                "refreshToken=" + refreshToken);
        }

        private void ConfigureController()
        {
            var configuration =
                new HttpConfiguration();

            var request =
                new HttpRequestMessage(
                    HttpMethod.Post,
                    "http://localhost/auth");

            request.Properties[
                HttpPropertyKeys.HttpConfigurationKey] =
                configuration;

            _controller.Request = request;
        }
    }
}
