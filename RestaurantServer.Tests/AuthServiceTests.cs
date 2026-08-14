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
using RestaurantServer.Validators.Interfaces;
using System;
using System.Threading;
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
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IAuthenticationValidator> _authenticationValidatorMock;

        private AuthService _authService;

        [TestInitialize]
        public void Setup()
        {
            _authRepositoryMock =
                new Mock<IAuthRepository>();

            _refreshTokenRepositoryMock =
                new Mock<IRefreshTokenRepository>();

            _passwordHasherMock =
                new Mock<IPasswordHasher>();

            _jwtTokenServiceMock =
                new Mock<IJwtTokenService>();

            _unitOfWorkMock =
                new Mock<IUnitOfWork>();

            _authenticationValidatorMock =
                new Mock<IAuthenticationValidator>();

            _unitOfWorkMock
                .Setup(unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _authService = new AuthService(
                _authRepositoryMock.Object,
                _refreshTokenRepositoryMock.Object,
                _passwordHasherMock.Object,
                _jwtTokenServiceMock.Object,
                _unitOfWorkMock.Object,
                _authenticationValidatorMock.Object);
        }

        #region Signup Tests

        [TestMethod]
        public async Task SignupAsync_EmailAlreadyExists_ThrowsBusinessException()
        {
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
                    repository.GetUserByEmailAsync(
                        request.Email))
                .ReturnsAsync(existingUser);

            Func<Task> action = async () =>
                await _authService.SignupAsync(request);

            await Assert.ThrowsExceptionAsync<ValidationException>(
                action);
        }

        [TestMethod]
        public async Task SignupAsync_ValidRequest_ReturnsSignupResponse()
        {
            var request = new SignupRequest
            {
                Name = "New User",
                Email = "NEWUSER@EXAMPLE.COM",
                Password = "Password@123"
            };

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetUserByEmailAsync(
                        "newuser@example.com"))
                .ReturnsAsync((User)null);

            _passwordHasherMock
                .Setup(hasher =>
                    hasher.HashPassword(
                        request.Password))
                .Returns("hashed-password");

            _authRepositoryMock
                .Setup(repository =>
                    repository.AddAsync(
                        It.IsAny<User>()))
                .Callback<User>(user =>
                    user.Id = 10008)
                .Returns(Task.CompletedTask);

            var response =
                await _authService.SignupAsync(request);

            Assert.IsNotNull(response);

            Assert.AreEqual(
                10008,
                response.UserId);

            Assert.AreEqual(
                "New User",
                response.Name);

            Assert.AreEqual(
                "newuser@example.com",
                response.Email);

            Assert.AreEqual(
                SuccessMessages.UserRegistered,
                response.Message);

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #endregion

        #region Login Tests

        [TestMethod]
        public async Task LoginAsync_UserNotFound_ThrowsBusinessException()
        {
            var request = new LoginRequest
            {
                Email = "notfound@example.com",
                Password = "Password@123"
            };

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetUserByEmailAsync(
                        "notfound@example.com"))
                .ReturnsAsync((User)null);

            Func<Task> action = async () =>
                await _authService.LoginAsync(request);

            await Assert.ThrowsExceptionAsync<ValidationException>(
                action);
        }

        [TestMethod]
        public async Task LoginAsync_UserInactive_ThrowsBusinessException()
        {
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
                    repository.GetUserByEmailAsync(
                        request.Email))
                .ReturnsAsync(inactiveUser);

            _authenticationValidatorMock
                .Setup(validator =>
                    validator.ValidateUserIsActive(
                        inactiveUser))
                .Throws(
                    new ValidationException(
                        ValidationMessages.UserInactive));

            Func<Task> action = async () =>
                await _authService.LoginAsync(request);

            await Assert.ThrowsExceptionAsync<ValidationException>(
                action);
        }

        [TestMethod]
        public async Task LoginAsync_InvalidPassword_ThrowsBusinessException()
        {
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
                    repository.GetUserByEmailAsync(
                        request.Email))
                .ReturnsAsync(user);

            _authenticationValidatorMock
                .Setup(validator =>
                    validator.ValidateUserIsActive(
                        user));

            _passwordHasherMock
                .Setup(hasher =>
                    hasher.VerifyPassword(
                        request.Password,
                        user.PasswordHash))
                .Returns(false);

            _authenticationValidatorMock
                .Setup(validator =>
                    validator.ValidatePassword(false))
                .Throws(
                    new ValidationException(
                        ValidationMessages.InvalidCredentials));

            Func<Task> action = async () =>
                await _authService.LoginAsync(request);

            await Assert.ThrowsExceptionAsync<ValidationException>(
                action);
        }

        [TestMethod]
        public async Task LoginAsync_ValidCredentials_ReturnsLoginResult()
        {
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
                    repository.GetUserByEmailAsync(
                        request.Email))
                .ReturnsAsync(user);

            _authenticationValidatorMock
                .Setup(validator =>
                    validator.ValidateUserIsActive(
                        user));

            _passwordHasherMock
                .Setup(hasher =>
                    hasher.VerifyPassword(
                        request.Password,
                        user.PasswordHash))
                .Returns(true);

            _authenticationValidatorMock
                .Setup(validator =>
                    validator.ValidatePassword(true));

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
                    repository.AddAsync(
                        It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);

            var result =
                await _authService.LoginAsync(request);

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
                    repository.AddAsync(
                        It.Is<RefreshToken>(
                            token =>
                                token.UserId == user.Id &&
                                token.Token == "refresh-token" &&
                                token.IsRevoked == false)),
                Times.Once);

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #endregion

        #region Refresh Token Tests

        [TestMethod]
        public async Task RefreshTokenAsync_TokenNotFound_ThrowsBusinessException()
        {
            var refreshToken = "invalid-refresh-token";

            _refreshTokenRepositoryMock
                .Setup(repository =>
                    repository.GetByTokenAsync(
                        refreshToken,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync((RefreshToken)null);

            _authenticationValidatorMock
                .Setup(validator =>
                    validator.ValidateRefreshToken(null))
                .Throws(
                    new ValidationException(
                        ValidationMessages.InvalidRefreshToken));

            Func<Task> action = async () =>
                await _authService.RefreshTokenAsync(
                    refreshToken);

            await Assert.ThrowsExceptionAsync<ValidationException>(
                action);
        }

        [TestMethod]
        public async Task RefreshTokenAsync_TokenRevoked_ThrowsBusinessException()
        {
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
                    repository.GetByTokenAsync(
                        refreshToken,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(revokedToken);

            _authenticationValidatorMock
                .Setup(validator =>
                    validator.ValidateRefreshToken(
                        revokedToken));

            _authenticationValidatorMock
                .Setup(validator =>
                    validator.ValidateRefreshTokenIsNotRevoked(
                        revokedToken))
                .Throws(
                    new ValidationException(
                        ValidationMessages.InvalidRefreshToken));

            Func<Task> action = async () =>
                await _authService.RefreshTokenAsync(
                    refreshToken);

            await Assert.ThrowsExceptionAsync<ValidationException>(
                action);
        }

        [TestMethod]
        public async Task RefreshTokenAsync_TokenExpired_ThrowsBusinessException()
        {
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
                    repository.GetByTokenAsync(
                        refreshToken,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingRefreshToken);

            _authenticationValidatorMock
                .Setup(validator =>
                    validator.ValidateRefreshToken(
                        existingRefreshToken));

            _authenticationValidatorMock
                .Setup(validator =>
                    validator.ValidateRefreshTokenIsNotRevoked(
                        existingRefreshToken));

            _authenticationValidatorMock
                .Setup(validator =>
                    validator.ValidateRefreshTokenIsNotExpired(
                        existingRefreshToken))
                .Throws(
                    new ValidationException(
                        ValidationMessages.InvalidRefreshToken));

            Func<Task> action = async () =>
                await _authService.RefreshTokenAsync(
                    refreshToken);

            await Assert.ThrowsExceptionAsync<ValidationException>(
                action);
        }

        [TestMethod]
        public async Task RefreshTokenAsync_UserInactive_ThrowsBusinessException()
        {
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
                    repository.GetByTokenAsync(
                        refreshToken,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingRefreshToken);

            _authenticationValidatorMock
                .Setup(validator =>
                    validator.ValidateRefreshToken(
                        existingRefreshToken));

            _authenticationValidatorMock
                .Setup(validator =>
                    validator.ValidateRefreshTokenIsNotRevoked(
                        existingRefreshToken));

            _authenticationValidatorMock
                .Setup(validator =>
                    validator.ValidateRefreshTokenIsNotExpired(
                        existingRefreshToken));

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAsync(
                        existingRefreshToken.UserId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(inactiveUser);

            _authenticationValidatorMock
                .Setup(validator =>
                    validator.ValidateRefreshTokenUser(
                        inactiveUser))
                .Throws(
                    new ValidationException(
                        ValidationMessages.InvalidRefreshToken));

            Func<Task> action = async () =>
                await _authService.RefreshTokenAsync(
                    refreshToken);

            await Assert.ThrowsExceptionAsync<ValidationException>(
                action);
        }

        [TestMethod]
        public async Task RefreshTokenAsync_UserNotFound_ThrowsBusinessException()
        {
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
                    repository.GetByTokenAsync(
                        refreshToken,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingRefreshToken);

            _authenticationValidatorMock
                .Setup(validator =>
                    validator.ValidateRefreshToken(
                        existingRefreshToken));

            _authenticationValidatorMock
                .Setup(validator =>
                    validator.ValidateRefreshTokenIsNotRevoked(
                        existingRefreshToken));

            _authenticationValidatorMock
                .Setup(validator =>
                    validator.ValidateRefreshTokenIsNotExpired(
                        existingRefreshToken));

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAsync(
                        existingRefreshToken.UserId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            _authenticationValidatorMock
                .Setup(validator =>
                    validator.ValidateRefreshTokenUser(null))
                .Throws(
                    new ValidationException(
                        ValidationMessages.InvalidRefreshToken));

            Func<Task> action = async () =>
                await _authService.RefreshTokenAsync(
                    refreshToken);

            await Assert.ThrowsExceptionAsync<ValidationException>(
                action);
        }

        [TestMethod]
        public async Task RefreshTokenAsync_ValidToken_ReturnsLoginResult()
        {
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
                    repository.GetByTokenAsync(
                        refreshToken,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingRefreshToken);

            _authenticationValidatorMock
                .Setup(validator =>
                    validator.ValidateRefreshToken(
                        existingRefreshToken));

            _authenticationValidatorMock
                .Setup(validator =>
                    validator.ValidateRefreshTokenIsNotRevoked(
                        existingRefreshToken));

            _authenticationValidatorMock
                .Setup(validator =>
                    validator.ValidateRefreshTokenIsNotExpired(
                        existingRefreshToken));

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAsync(
                        user.Id,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _authenticationValidatorMock
                .Setup(validator =>
                    validator.ValidateRefreshTokenUser(
                        user));

            _jwtTokenServiceMock
                .Setup(service =>
                    service.GenerateAccessToken(user))
                .Returns("new-access-token");

            _jwtTokenServiceMock
                .Setup(service =>
                    service.GenerateRefreshToken())
                .Returns(newRefreshToken);

            var result =
                await _authService.RefreshTokenAsync(
                    refreshToken);

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
                SuccessMessages.TokenRefreshed,
                result.Response.Message);

            Assert.AreEqual(
                newRefreshToken,
                existingRefreshToken.Token);

            Assert.IsFalse(
                existingRefreshToken.IsRevoked);

            _refreshTokenRepositoryMock.Verify(
                repository =>
                    repository.Update(
                        existingRefreshToken),
                Times.Once);

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #endregion

        #region Logout Tests

        [TestMethod]
        public async Task LogoutAsync_EmptyToken_ThrowsBusinessException()
        {
            var refreshToken = "";

            Func<Task> action = async () =>
                await _authService.LogoutAsync(
                    refreshToken);

            await Assert.ThrowsExceptionAsync<ValidationException>(
                action);
        }

        [TestMethod]
        public async Task LogoutAsync_TokenNotFound_ThrowsBusinessException()
        {
            var refreshToken = "invalid-refresh-token";

            _refreshTokenRepositoryMock
                .Setup(repository =>
                    repository.GetByTokenAsync(
                        refreshToken,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync((RefreshToken)null);

            _authenticationValidatorMock
                .Setup(validator =>
                    validator.ValidateRefreshToken(null))
                .Throws(
                    new ValidationException(
                        ValidationMessages.InvalidRefreshToken));

            Func<Task> action = async () =>
                await _authService.LogoutAsync(
                    refreshToken);

            await Assert.ThrowsExceptionAsync<ValidationException>(
                action);
        }

        [TestMethod]
        public async Task LogoutAsync_ValidToken_RevokesToken()
        {
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
                    repository.GetByTokenAsync(
                        refreshToken,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingRefreshToken);

            _authenticationValidatorMock
                .Setup(validator =>
                    validator.ValidateRefreshToken(
                        existingRefreshToken));

            _authenticationValidatorMock
                .Setup(validator =>
                    validator.ValidateRefreshTokenIsNotRevoked(
                        existingRefreshToken));

            await _authService.LogoutAsync(
                refreshToken);

            Assert.IsTrue(
                existingRefreshToken.IsRevoked);

            _refreshTokenRepositoryMock.Verify(
                repository =>
                    repository.Update(
                        existingRefreshToken),
                Times.Once);

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #endregion
    }
}
