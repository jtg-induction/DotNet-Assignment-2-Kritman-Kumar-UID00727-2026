using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestaurantServer.Constants;
using RestaurantServer.DTOs.Requests;
using RestaurantServer.Enums;
using RestaurantServer.Exceptions;
using RestaurantServer.Helpers.Interfaces;
using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.Services.Implementations;
using System;
using System.Threading.Tasks;

namespace RestaurantServer.Tests
{
    [TestClass]
    public class AuthServiceTests
    {
        private Mock<IAuthRepository> _authRepositoryMock;
        private Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private Mock<IPasswordHasher> _passwordHasherMock;
        private Mock<IJwtTokenService> _jwtTokenServiceMock;

        private AuthService _authService;

        [TestInitialize]
        public void Setup()
        {
            _authRepositoryMock = new Mock<IAuthRepository>();
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();

            _authService = new AuthService(
                _authRepositoryMock.Object,
                _refreshTokenRepositoryMock.Object,
                _passwordHasherMock.Object,
                _jwtTokenServiceMock.Object
            );
        }

        [TestMethod]
        public async Task SignupAsync_EmailAlreadyExists_ThrowsBusinessException()
        {
            // Arrange
            var request = new SignupRequest
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "Password@123"
            };

            var existingUser = new User
            {
                Id = 1,
                Name = "Existing User",
                Email = "test@example.com"
            };

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(existingUser);

            // Act
            Func<Task> action = async () =>
                await _authService.SignupAsync(request);

            // Assert
            await Assert.ThrowsExceptionAsync<BusinessException>(action);
        }

        [TestMethod]
        public async Task SignupAsync_ValidRequest_ReturnsSignupResponse()
        {
            // Arrange
            var request = new SignupRequest
            {
                Name = "New User",
                Email = "NEWUSER@EXAMPLE.COM",
                Password = "Password@123"
            };

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetUserByEmailAsync("newuser@example.com"))
                .ReturnsAsync((User)null);

            _passwordHasherMock
                .Setup(hasher =>
                    hasher.HashPassword(request.Password))
                .Returns("hashed-password");

            _authRepositoryMock
                .Setup(repository =>
                    repository.AddUser(It.IsAny<User>()))
                .Callback<User>(user =>
                    user.Id = 10008);

            _authRepositoryMock
                .Setup(repository =>
                    repository.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            var response =
                await _authService.SignupAsync(request);

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(10008, response.UserId);
            Assert.AreEqual("New User", response.Name);
            Assert.AreEqual("newuser@example.com", response.Email);
            Assert.AreEqual(
                SuccessMessages.UserRegistered,
                response.Message);
        }

        [TestMethod]
        public async Task LoginAsync_UserNotFound_ThrowsBusinessException()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "notfound@example.com",
                Password = "Password@123"
            };

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetUserByEmailAsync("notfound@example.com"))
                .ReturnsAsync((User)null);

            // Act
            Func<Task> action = async () =>
                await _authService.LoginAsync(request);

            // Assert
            await Assert.ThrowsExceptionAsync<BusinessException>(action);
        }

        [TestMethod]
        public async Task LoginAsync_UserInactive_ThrowsBusinessException()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "inactive@example.com",
                Password = "Password@123"
            };

            var inactiveUser = new User
            {
                Id = 1,
                Email = "inactive@example.com",
                PasswordHash = "hashed-password",
                IsActive = false
            };

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(inactiveUser);

            // Act
            Func<Task> action = async () =>
                await _authService.LoginAsync(request);

            // Assert
            await Assert.ThrowsExceptionAsync<BusinessException>(action);
        }

        [TestMethod]
        public async Task LoginAsync_InvalidPassword_ThrowsBusinessException()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };

            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                PasswordHash = "hashed-password",
                IsActive = true
            };

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(hasher =>
                    hasher.VerifyPassword(
                        request.Password,
                        user.PasswordHash))
                .Returns(false);

            // Act
            Func<Task> action = async () =>
                await _authService.LoginAsync(request);

            // Assert
            await Assert.ThrowsExceptionAsync<BusinessException>(action);
        }

        [TestMethod]
        public async Task LoginAsync_ValidCredentials_ReturnsLoginResult()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "Password@123"
            };

            var user = new User
            {
                Id = 1,
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashed-password",
                Role = 1,
                IsActive = true
            };

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(hasher =>
                    hasher.VerifyPassword(
                        request.Password,
                        user.PasswordHash))
                .Returns(true);

            _jwtTokenServiceMock
                .Setup(service =>
                    service.GenerateAccessToken(user))
                .Returns("access-token");

            _jwtTokenServiceMock
                .Setup(service =>
                    service.GenerateRefreshToken())
                .Returns("refresh-token");

            _refreshTokenRepositoryMock
                .Setup(repository =>
                    repository.AddAsync(It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);

            _refreshTokenRepositoryMock
                .Setup(repository =>
                    repository.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result =
                await _authService.LoginAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Response);

            Assert.AreEqual(
                "access-token",
                result.Response.AccessToken);

            Assert.AreEqual(
                "refresh-token",
                result.RefreshToken);

            Assert.AreEqual(
                user.Id,
                result.Response.UserId);

            Assert.AreEqual(
                user.Name,
                result.Response.Name);

            Assert.AreEqual(
                user.Role,
                (int)result.Response.Role);

            Assert.AreEqual(
                SuccessMessages.LoginSuccessful,
                result.Response.Message);

            _refreshTokenRepositoryMock.Verify(
                repository =>
                    repository.AddAsync(It.Is<RefreshToken>(
                        token =>
                            token.UserId == user.Id &&
                            token.Token == "refresh-token" &&
                            token.IsRevoked == false)),
                Times.Once);

            _refreshTokenRepositoryMock.Verify(
                repository =>
                    repository.SaveAsync(),
                Times.Once);
        }

        [TestMethod]
        public async Task RefreshTokenAsync_TokenNotFound_ThrowsBusinessException()
        {
            // Arrange
            var refreshToken = "invalid-refresh-token";

            _refreshTokenRepositoryMock
                .Setup(repository =>
                    repository.GetByTokenAsync(refreshToken))
                .ReturnsAsync((RefreshToken)null);

            // Act
            Func<Task> action = async () =>
                await _authService.RefreshTokenAsync(refreshToken);

            // Assert
            await Assert.ThrowsExceptionAsync<BusinessException>(action);
        }

        [TestMethod]
        public async Task RefreshTokenAsync_TokenRevoked_ThrowsBusinessException()
        {
            // Arrange
            var refreshToken = "revoked-refresh-token";

            var revokedToken = new RefreshToken
            {
                Id = 1,
                UserId = 1,
                Token = refreshToken,
                IsRevoked = true,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                ExpiresAt = DateTime.UtcNow.AddDays(29)
            };

            _refreshTokenRepositoryMock
                .Setup(repository =>
                    repository.GetByTokenAsync(refreshToken))
                .ReturnsAsync(revokedToken);

            // Act
            Func<Task> action = async () =>
                await _authService.RefreshTokenAsync(refreshToken);

            // Assert
            await Assert.ThrowsExceptionAsync<BusinessException>(action);
        }
 
        [TestMethod]
        public async Task RefreshTokenAsync_TokenExpired_ThrowsBusinessException()
        {
            // Arrange
            var refreshToken = "expired-refresh-token";

            var existingRefreshToken = new RefreshToken
            {
                Id = 1,
                UserId = 1,
                Token = refreshToken,
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow.AddDays(-31),
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                ExpiresAt = DateTime.UtcNow.AddMinutes(-1)
            };

            _refreshTokenRepositoryMock
                .Setup(repository =>
                    repository.GetByTokenAsync(refreshToken))
                .ReturnsAsync(existingRefreshToken);

            // Act
            Func<Task> action = async () =>
                await _authService.RefreshTokenAsync(refreshToken);

            // Assert
            await Assert.ThrowsExceptionAsync<BusinessException>(action);
        }

        [TestMethod]
        public async Task RefreshTokenAsync_UserInactive_ThrowsBusinessException()
        {
            // Arrange
            var refreshToken = "valid-refresh-token";

            var existingRefreshToken = new RefreshToken
            {
                Id = 1,
                UserId = 1,
                Token = refreshToken,
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(29)
            };

            var inactiveUser = new User
            {
                Id = 1,
                Name = "Inactive User",
                Email = "inactive@example.com",
                IsActive = false
            };

            _refreshTokenRepositoryMock
                .Setup(repository =>
                    repository.GetByTokenAsync(refreshToken))
                .ReturnsAsync(existingRefreshToken);

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetUserByIdAsync(existingRefreshToken.UserId))
                .ReturnsAsync(inactiveUser);

            // Act
            Func<Task> action = async () =>
                await _authService.RefreshTokenAsync(refreshToken);

            // Assert
            await Assert.ThrowsExceptionAsync<BusinessException>(action);
        }


        [TestMethod]
        public async Task RefreshTokenAsync_UserNotFound_ThrowsBusinessException()
        {
            // Arrange
            var refreshToken = "valid-refresh-token";

            var existingRefreshToken = new RefreshToken
            {
                Id = 1,
                UserId = 999,
                Token = refreshToken,
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(29)
            };

            _refreshTokenRepositoryMock
                .Setup(repository =>
                    repository.GetByTokenAsync(refreshToken))
                .ReturnsAsync(existingRefreshToken);

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetUserByIdAsync(existingRefreshToken.UserId))
                .ReturnsAsync((User)null);

            // Act
            Func<Task> action = async () =>
                await _authService.RefreshTokenAsync(refreshToken);

            // Assert
            await Assert.ThrowsExceptionAsync<BusinessException>(action);
        }

        [TestMethod]
        public async Task RefreshTokenAsync_ValidToken_ReturnsLoginResult()
        {
            // Arrange
            var refreshToken = "old-refresh-token";
            var newRefreshToken = "new-refresh-token";

            var existingRefreshToken = new RefreshToken
            {
                Id = 1,
                UserId = 1,
                Token = refreshToken,
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(29)
            };

            var user = new User
            {
                Id = 1,
                Name = "Test User",
                Email = "test@example.com",
                Role = 1,
                IsActive = true
            };

            _refreshTokenRepositoryMock
                .Setup(repository =>
                    repository.GetByTokenAsync(refreshToken))
                .ReturnsAsync(existingRefreshToken);

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetUserByIdAsync(user.Id))
                .ReturnsAsync(user);

            _jwtTokenServiceMock
                .Setup(service =>
                    service.GenerateAccessToken(user))
                .Returns("new-access-token");

            _jwtTokenServiceMock
                .Setup(service =>
                    service.GenerateRefreshToken())
                .Returns(newRefreshToken);

            _refreshTokenRepositoryMock
                .Setup(repository =>
                    repository.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result =
                await _authService.RefreshTokenAsync(refreshToken);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Response);

            Assert.AreEqual(
                "new-access-token",
                result.Response.AccessToken);

            Assert.AreEqual(
                newRefreshToken,
                result.RefreshToken);

            Assert.AreEqual(
                user.Id,
                result.Response.UserId);

            Assert.AreEqual(
                user.Name,
                result.Response.Name);

            Assert.AreEqual(
                (UserRole)user.Role,
                result.Response.Role);

            Assert.AreEqual(
                newRefreshToken,
                existingRefreshToken.Token);

            Assert.IsFalse(existingRefreshToken.IsRevoked);

            _refreshTokenRepositoryMock.Verify(
                repository =>
                    repository.Update(existingRefreshToken),
                Times.Once);

            _refreshTokenRepositoryMock.Verify(
                repository =>
                    repository.SaveAsync(),
                Times.Once);
        }

        [TestMethod]
        public async Task LogoutAsync_EmptyToken_ThrowsBusinessException()
        {
            // Arrange
            var refreshToken = "";

            // Act
            Func<Task> action = async () =>
                await _authService.LogoutAsync(refreshToken);

            // Assert
            await Assert.ThrowsExceptionAsync<BusinessException>(action);
        }

        [TestMethod]
        public async Task LogoutAsync_TokenNotFound_ThrowsBusinessException()
        {
            // Arrange
            var refreshToken = "invalid-refresh-token";

            _refreshTokenRepositoryMock
                .Setup(repository =>
                    repository.GetByTokenAsync(refreshToken))
                .ReturnsAsync((RefreshToken)null);

            // Act
            Func<Task> action = async () =>
                await _authService.LogoutAsync(refreshToken);

            // Assert
            await Assert.ThrowsExceptionAsync<BusinessException>(action);
        }

        [TestMethod]
        public async Task LogoutAsync_ValidToken_RevokesToken()
        {
            // Arrange
            var refreshToken = "valid-refresh-token";

            var existingRefreshToken = new RefreshToken
            {
                Id = 1,
                UserId = 1,
                Token = refreshToken,
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(29)
            };

            _refreshTokenRepositoryMock
                .Setup(repository =>
                    repository.GetByTokenAsync(refreshToken))
                .ReturnsAsync(existingRefreshToken);

            _refreshTokenRepositoryMock
                .Setup(repository =>
                    repository.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _authService.LogoutAsync(refreshToken);

            // Assert
            Assert.IsTrue(existingRefreshToken.IsRevoked);

            _refreshTokenRepositoryMock.Verify(
                repository =>
                    repository.Update(existingRefreshToken),
                Times.Once);

            _refreshTokenRepositoryMock.Verify(
                repository =>
                    repository.SaveAsync(),
                Times.Once);
        }
    }
}
