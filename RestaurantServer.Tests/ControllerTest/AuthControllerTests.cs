using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestaurantServer.Constants;
using RestaurantServer.Controllers;
using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using RestaurantServer.Enums;
using RestaurantServer.Exceptions;
using RestaurantServer.Helpers.Interfaces;
using RestaurantServer.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
        private Mock<IRefreshTokenHelper> _refreshTokenHelperMock;
        private Mock<ICookieHelper> _cookieHelperMock;

        private AuthController _controller;

        [TestInitialize]
        public void Setup()
        {
            _authServiceMock = new Mock<IAuthService>();
            _refreshTokenHelperMock = new Mock<IRefreshTokenHelper>();
            _cookieHelperMock = new Mock<ICookieHelper>();

            _controller = new AuthController(
                _authServiceMock.Object,
                _refreshTokenHelperMock.Object,
                _cookieHelperMock.Object);

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

            var user = new User
            {
                Id = 1,
                Name = "Test User",
                Email = "test@test.com"
            };

            var expectedResponse = new SignupResponse(user);

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

            SetupLogin(request, loginResult);

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

            SetupLogin(request, loginResult);

            var cookie = CreateCookie(
                "refreshToken",
                "refresh-token");

            _cookieHelperMock
                .Setup(helper =>
                    helper.CreateHttpOnlySecureCookie(
                        "refreshToken",
                        "refresh-token",
                        "auth"))
                .Returns(cookie);

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

            _cookieHelperMock.Verify(
                helper =>
                    helper.CreateHttpOnlySecureCookie(
                        "refreshToken",
                        "refresh-token",
                        "auth"),
                Times.Once);
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

            SetupLogin(request, loginResult);

            await _controller.Login(request);

            _authServiceMock.Verify(
                service =>
                    service.LoginAsync(request),
                Times.Once);
        }

        [TestMethod]
        public async Task Refresh_ShouldGetRefreshTokenFromHelper()
        {
            var refreshToken =
                "old-refresh-token";

            var refreshResult =
                CreateRefreshResult(
                    "new-access-token",
                    "new-refresh-token");

            _refreshTokenHelperMock
                .Setup(helper =>
                    helper.GetRefreshTokenFromRequest(
                        _controller.Request,
                        "refreshToken"))
                .Returns(refreshToken);

            SetupRefresh(
                refreshToken,
                refreshResult);

            var result =
                await _controller.Refresh();

            var response =
                await result.ExecuteAsync(
                    CancellationToken.None);

            Assert.AreEqual(
                HttpStatusCode.OK,
                response.StatusCode);

            _refreshTokenHelperMock.Verify(
                helper =>
                    helper.GetRefreshTokenFromRequest(
                        _controller.Request,
                        "refreshToken"),
                Times.Once);
        }

        [TestMethod]
        public async Task Refresh_ShouldPassRefreshTokenToAuthService()
        {
            var refreshToken =
                "old-refresh-token";

            var refreshResult =
                CreateRefreshResult(
                    "new-access-token",
                    "new-refresh-token");

            _refreshTokenHelperMock
                .Setup(helper =>
                    helper.GetRefreshTokenFromRequest(
                        _controller.Request,
                        "refreshToken"))
                .Returns(refreshToken);

            SetupRefresh(
                refreshToken,
                refreshResult);

            await _controller.Refresh();

            _authServiceMock.Verify(
                service =>
                    service.RefreshTokenAsync(
                        refreshToken),
                Times.Once);
        }

        [TestMethod]
        public async Task Refresh_ShouldSetNewRefreshTokenCookie()
        {
            var oldRefreshToken =
                "old-refresh-token";

            var newRefreshToken =
                "new-refresh-token";

            var refreshResult =
                CreateRefreshResult(
                    "new-access-token",
                    newRefreshToken);

            _refreshTokenHelperMock
                .Setup(helper =>
                    helper.GetRefreshTokenFromRequest(
                        _controller.Request,
                        "refreshToken"))
                .Returns(oldRefreshToken);

            SetupRefresh(
                oldRefreshToken,
                refreshResult);

            var cookie = CreateCookie(
                "refreshToken",
                newRefreshToken);

            _cookieHelperMock
                .Setup(helper =>
                    helper.CreateHttpOnlySecureCookie(
                        "refreshToken",
                        newRefreshToken,
                        "auth"))
                .Returns(cookie);

            var result =
                await _controller.Refresh();

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

            _cookieHelperMock.Verify(
                helper =>
                    helper.CreateHttpOnlySecureCookie(
                        "refreshToken",
                        newRefreshToken,
                        "auth"),
                Times.Once);
        }

        [TestMethod]
        public async Task Refresh_WhenHelperThrowsValidationException_ShouldPropagateException()
        {
            var exception =
                new ValidationException(
                    ValidationMessages.InvalidRefreshToken);

            _refreshTokenHelperMock
                .Setup(helper =>
                    helper.GetRefreshTokenFromRequest(
                        _controller.Request,
                        "refreshToken"))
                .Throws(exception);

            var actualException =
                await Assert.ThrowsExceptionAsync<ValidationException>(
                    async () =>
                        await _controller.Refresh());

            Assert.AreEqual(
                ValidationMessages.InvalidRefreshToken,
                actualException.Message);

            _authServiceMock.Verify(
                service =>
                    service.RefreshTokenAsync(
                        It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public async Task Logout_ShouldGetRefreshTokenFromHelper()
        {
            var refreshToken =
                "refresh-token";

            _refreshTokenHelperMock
                .Setup(helper =>
                    helper.GetRefreshTokenFromRequest(
                        _controller.Request,
                        "refreshToken"))
                .Returns(refreshToken);

            _authServiceMock
                .Setup(service =>
                    service.LogoutAsync(refreshToken))
                .Returns(Task.CompletedTask);

            SetupLogoutCookie();

            var result =
                await _controller.Logout();

            var response =
                await result.ExecuteAsync(
                    CancellationToken.None);

            Assert.AreEqual(
                HttpStatusCode.OK,
                response.StatusCode);

            _refreshTokenHelperMock.Verify(
                helper =>
                    helper.GetRefreshTokenFromRequest(
                        _controller.Request,
                        "refreshToken"),
                Times.Once);
        }

        [TestMethod]
        public async Task Logout_ShouldPassRefreshTokenToAuthService()
        {
            var refreshToken =
                "refresh-token";

            _refreshTokenHelperMock
                .Setup(helper =>
                    helper.GetRefreshTokenFromRequest(
                        _controller.Request,
                        "refreshToken"))
                .Returns(refreshToken);

            _authServiceMock
                .Setup(service =>
                    service.LogoutAsync(refreshToken))
                .Returns(Task.CompletedTask);

            SetupLogoutCookie();

            await _controller.Logout();

            _authServiceMock.Verify(
                service =>
                    service.LogoutAsync(refreshToken),
                Times.Once);
        }

        [TestMethod]
        public async Task Logout_ShouldReturnLogoutSuccessMessage()
        {
            var refreshToken =
                "refresh-token";

            _refreshTokenHelperMock
                .Setup(helper =>
                    helper.GetRefreshTokenFromRequest(
                        _controller.Request,
                        "refreshToken"))
                .Returns(refreshToken);

            _authServiceMock
                .Setup(service =>
                    service.LogoutAsync(refreshToken))
                .Returns(Task.CompletedTask);

            SetupLogoutCookie();

            var result =
                await _controller.Logout();

            var response =
                await result.ExecuteAsync(
                    CancellationToken.None);

            var content =
                await response.Content.ReadAsStringAsync();

            StringAssert.Contains(
                content,
                SuccessMessages.LogoutSuccessful);
        }

        [TestMethod]
        public async Task Logout_ShouldExpireRefreshTokenCookie()
        {
            var refreshToken =
                "refresh-token";

            _refreshTokenHelperMock
                .Setup(helper =>
                    helper.GetRefreshTokenFromRequest(
                        _controller.Request,
                        "refreshToken"))
                .Returns(refreshToken);

            _authServiceMock
                .Setup(service =>
                    service.LogoutAsync(refreshToken))
                .Returns(Task.CompletedTask);

            var expiredCookie =
                CreateCookie(
                    "refreshToken",
                    string.Empty);

            expiredCookie.Expires =
                DateTimeOffset.UtcNow.AddDays(-1);

            _cookieHelperMock
                .Setup(helper =>
                    helper.CreateHttpOnlySecureCookie(
                        "refreshToken",
                        string.Empty,
                        "auth"))
                .Returns(expiredCookie);

            var result =
                await _controller.Logout();

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

            _cookieHelperMock.Verify(
                helper =>
                    helper.CreateHttpOnlySecureCookie(
                        "refreshToken",
                        string.Empty,
                        "auth"),
                Times.Once);
        }

        [TestMethod]
        public async Task Logout_WhenHelperThrowsValidationException_ShouldPropagateException()
        {
            var exception =
                new ValidationException(
                    ValidationMessages.InvalidRefreshToken);

            _refreshTokenHelperMock
                .Setup(helper =>
                    helper.GetRefreshTokenFromRequest(
                        _controller.Request,
                        "refreshToken"))
                .Throws(exception);

            var actualException =
                await Assert.ThrowsExceptionAsync<ValidationException>(
                    async () =>
                        await _controller.Logout());

            Assert.AreEqual(
                ValidationMessages.InvalidRefreshToken,
                actualException.Message);

            _authServiceMock.Verify(
                service =>
                    service.LogoutAsync(
                        It.IsAny<string>()),
                Times.Never);
        }

        private void SetupLogin(
            LoginRequest request,
            LoginResult result)
        {
            _authServiceMock
                .Setup(service =>
                    service.LoginAsync(request))
                .ReturnsAsync(result);

            _cookieHelperMock
                .Setup(helper =>
                    helper.CreateHttpOnlySecureCookie(
                        "refreshToken",
                        result.RefreshToken,
                        "auth"))
                .Returns(
                    CreateCookie(
                        "refreshToken",
                        result.RefreshToken));
        }

        private void SetupRefresh(
            string refreshToken,
            RefreshResult result)
        {
            _authServiceMock
                .Setup(service =>
                    service.RefreshTokenAsync(
                        refreshToken))
                .ReturnsAsync(result);

            _cookieHelperMock
                .Setup(helper =>
                    helper.CreateHttpOnlySecureCookie(
                        "refreshToken",
                        result.RefreshToken,
                        "auth"))
                .Returns(
                    CreateCookie(
                        "refreshToken",
                        result.RefreshToken));
        }

        private void SetupLogoutCookie()
        {
            _cookieHelperMock
                .Setup(helper =>
                    helper.CreateHttpOnlySecureCookie(
                        "refreshToken",
                        string.Empty,
                        "auth"))
                .Returns(
                    CreateCookie(
                        "refreshToken",
                        string.Empty));
        }

        private LoginResult CreateLoginResult(
            string accessToken,
            string refreshToken)
        {
            var user = new User
            {
                Id = 1,
                Name = "Test User",
                Role = (int)UserRole.Customer
            };

            return new LoginResult
            {
                Response = new LoginResponse(
                    user,
                    accessToken,
                    "Login successful"),

                RefreshToken = refreshToken
            };
        }

        private RefreshResult CreateRefreshResult(
            string accessToken,
            string refreshToken)
        {
            return new RefreshResult
            {
                Response = new RefreshResponse(
                    accessToken,
                    "Bearer"),

                RefreshToken = refreshToken
            };
        }

        private CookieHeaderValue CreateCookie(
            string name,
            string value)
        {
            var cookie =
                new CookieHeaderValue(
                    name,
                    value);

            cookie.Path = "/auth";
            cookie.HttpOnly = true;
            cookie.Secure = true;

            return cookie;
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
