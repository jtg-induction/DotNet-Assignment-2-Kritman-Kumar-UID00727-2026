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
using RestaurantServer.validator.Interfaces;
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
        private Mock<IRefreshTokenValidator> _refreshTokenValidatorMock;
        private Mock<IUserValidator> _userValidatorMock;
        private Mock<IRequestValidator> _requestValidatorMock;

        private AuthService _authService;

        [TestInitialize]
        public void Setup()
        {
            _authRepositoryMock = new Mock<IAuthRepository>();
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _authenticationValidatorMock = new Mock<IAuthenticationValidator>();
            _refreshTokenValidatorMock = new Mock<IRefreshTokenValidator>();
            _userValidatorMock = new Mock<IUserValidator>();
            _requestValidatorMock = new Mock<IRequestValidator>();

            _authService = new AuthService(
                _authRepositoryMock.Object,
                _refreshTokenRepositoryMock.Object,
                _passwordHasherMock.Object,
                _jwtTokenServiceMock.Object,
                _unitOfWorkMock.Object,
                _authenticationValidatorMock.Object,
                _refreshTokenValidatorMock.Object,
                _userValidatorMock.Object,
                _requestValidatorMock.Object);
        }

        [TestMethod]
        public async Task SignupAsync_EmailAlreadyExists_ShouldThrowValidationException()
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
                        request.Email,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            var exception = await Assert.ThrowsExceptionAsync<ValidationException>(
                () => _authService.SignupAsync(request));

            Assert.AreEqual(
                ValidationMessages.EmailAlreadyExists,
                exception.Message);
        }

        [TestMethod]
        public async Task SignupAsync_ValidRequest_ShouldReturnSignupResponse()
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
                        "newuser@example.com",
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            _passwordHasherMock
                .Setup(hasher =>
                    hasher.HashPassword(request.Password))
                .Returns("hashed-password");

            _authRepositoryMock
                .Setup(repository =>
                    repository.Add(It.IsAny<User>()))
                .Callback<User>(user =>
                {
                    user.Id = 10008;
                })
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var response = await _authService.SignupAsync(request);

            Assert.IsNotNull(response);
            Assert.AreEqual(10008, response.UserId);
            Assert.AreEqual("New User", response.Name);
            Assert.AreEqual("newuser@example.com", response.Email);
            Assert.AreEqual(
                SuccessMessages.UserRegistered,
                response.Message);

            _passwordHasherMock.Verify(
                hasher =>
                    hasher.HashPassword(request.Password),
                Times.Once);

            _authRepositoryMock.Verify(
                repository =>
                    repository.Add(
                        It.Is<User>(user =>
                            user.Name == "New User" &&
                            user.Email == "newuser@example.com" &&
                            user.PasswordHash == "hashed-password")),
                Times.Once);

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [TestMethod]
        public async Task LoginAsync_UserNotFound_ShouldThrowValidationException()
        {
            var request = new LoginRequest
            {
                Email = "notfound@example.com",
                Password = "Password@123"
            };

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetUserByEmailAsync(
                        request.Email,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            var exception = await Assert.ThrowsExceptionAsync<ValidationException>(
                () => _authService.LoginAsync(request));

            Assert.AreEqual(
                ValidationMessages.InvalidCredentials,
                exception.Message);

            _authenticationValidatorMock.Verify(
                validator =>
                    validator.ValidateUserIsActive(
                        It.IsAny<User>()),
                Times.Never);
        }

        [TestMethod]
        public async Task LoginAsync_UserInactive_ShouldThrowValidationException()
        {
            var request = new LoginRequest
            {
                Email = "inactive@example.com",
                Password = "Password@123"
            };

            var inactiveUser = new User
            {
                Id = 1,
                Name = "Inactive User",
                Email = "inactive@example.com",
                PasswordHash = "hashed-password",
                IsActive = false
            };

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetUserByEmailAsync(
                        request.Email,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(inactiveUser);

            _authenticationValidatorMock
                .Setup(validator =>
                    validator.ValidateUserIsActive(inactiveUser))
                .Throws(
                    new ValidationException(
                        ValidationMessages.UserInactive));

            var exception = await Assert.ThrowsExceptionAsync<ValidationException>(
                () => _authService.LoginAsync(request));

            Assert.AreEqual(
                ValidationMessages.UserInactive,
                exception.Message);

            _passwordHasherMock.Verify(
                hasher =>
                    hasher.VerifyPassword(
                        It.IsAny<string>(),
                        It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public async Task LoginAsync_InvalidPassword_ShouldThrowValidationException()
        {
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };

            var user = new User
            {
                Id = 1,
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashed-password",
                IsActive = true
            };

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetUserByEmailAsync(
                        request.Email,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

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

            var exception = await Assert.ThrowsExceptionAsync<ValidationException>(
                () => _authService.LoginAsync(request));

            Assert.AreEqual(
                ValidationMessages.InvalidCredentials,
                exception.Message);

            _jwtTokenServiceMock.Verify(
                service =>
                    service.GenerateAccessToken(
                        It.IsAny<User>()),
                Times.Never);

            _refreshTokenRepositoryMock.Verify(
                repository =>
                    repository.Add(
                        It.IsAny<RefreshToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task LoginAsync_ValidCredentials_ShouldReturnLoginResult()
        {
            var request = new LoginRequest
            {
                Email = "TEST@EXAMPLE.COM",
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
                        "test@example.com",
                        It.IsAny<CancellationToken>()))
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
                    repository.Add(
                        It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _authService.LoginAsync(request);

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
                (UserRole)user.Role,
                result.Response.Role);
            Assert.AreEqual(
                SuccessMessages.LoginSuccessful,
                result.Response.Message);

            _refreshTokenRepositoryMock.Verify(
                repository =>
                    repository.Add(
                        It.Is<RefreshToken>(token =>
                            token.UserId == user.Id &&
                            token.Token == "refresh-token")),
                Times.Once);

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [TestMethod]
        public async Task RefreshTokenAsync_TokenInvalid_ShouldThrowValidationException()
        {
            var refreshToken = "invalid-refresh-token";

            _refreshTokenRepositoryMock
                .Setup(repository =>
                    repository.GetByTokenAsync(
                        refreshToken,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync((RefreshToken)null);

            _refreshTokenValidatorMock
                .Setup(validator =>
                    validator.ValidateRefreshTokenIsValid(null))
                .Throws(
                    new ValidationException(
                        ValidationMessages.InvalidRefreshToken));

            var exception = await Assert.ThrowsExceptionAsync<ValidationException>(
                () => _authService.RefreshTokenAsync(refreshToken));

            Assert.AreEqual(
                ValidationMessages.InvalidRefreshToken,
                exception.Message);

            _authRepositoryMock.Verify(
                repository =>
                    repository.GetByIdAsync(
                        It.IsAny<long>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TestMethod] 
        public async Task RefreshTokenAsync_UserInactive_ShouldThrowValidationException()
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

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAsync(
                        existingRefreshToken.UserId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(inactiveUser);

            _userValidatorMock
                .Setup(validator =>
                    validator.IsUserNullOrDeactivated(inactiveUser, "Invalid refresh token."))
                .Throws(
                    new ValidationException(
                        ValidationMessages.InvalidRefreshToken));

            var exception = await Assert.ThrowsExceptionAsync<ValidationException>(
                () => _authService.RefreshTokenAsync(refreshToken));

            Assert.AreEqual(
                ValidationMessages.InvalidRefreshToken,
                exception.Message);

            _jwtTokenServiceMock.Verify(
                service =>
                    service.GenerateAccessToken(
                        It.IsAny<User>()),
                Times.Never);
        }

        [TestMethod]
        public async Task RefreshTokenAsync_ValidToken_ShouldReturnRefreshResult()
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

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAsync(
                        user.Id,
                        It.IsAny<CancellationToken>()))
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
                    repository.Update(existingRefreshToken));

            _unitOfWorkMock
                .Setup(unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _authService.RefreshTokenAsync(refreshToken);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Response);
            Assert.AreEqual(
                "new-access-token",
                result.Response.AccessToken);
            Assert.AreEqual(
                "Bearer",
                result.Response.TokenType);
            Assert.AreEqual(
                newRefreshToken,
                result.RefreshToken);
            Assert.AreEqual(
                newRefreshToken,
                existingRefreshToken.Token);
            Assert.IsFalse(
                existingRefreshToken.IsRevoked);

            _refreshTokenRepositoryMock.Verify(
                repository =>
                    repository.Update(existingRefreshToken),
                Times.Once);

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [TestMethod]
        public async Task LogoutAsync_EmptyToken_ShouldThrowValidationException()
        {
            var exception = await Assert.ThrowsExceptionAsync<ValidationException>(
                () => _authService.LogoutAsync(""));

            Assert.AreEqual(
                ValidationMessages.InvalidRefreshToken,
                exception.Message);

            _refreshTokenRepositoryMock.Verify(
                repository =>
                    repository.GetByTokenAsync(
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task LogoutAsync_TokenNotFound_ShouldThrowValidationException()
        {
            var refreshToken = "invalid-refresh-token";

            _refreshTokenRepositoryMock
                .Setup(repository =>
                    repository.GetByTokenAsync(
                        refreshToken,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync((RefreshToken)null);

            _refreshTokenValidatorMock
                .Setup(validator =>
                    validator.ValidateRefreshToken(null))
                .Throws(
                    new ValidationException(
                        ValidationMessages.InvalidRefreshToken));

            var exception = await Assert.ThrowsExceptionAsync<ValidationException>(
                () => _authService.LogoutAsync(refreshToken));

            Assert.AreEqual(
                ValidationMessages.InvalidRefreshToken,
                exception.Message);
        }

        [TestMethod]
        public async Task LogoutAsync_RevokedToken_ShouldThrowValidationException()
        {
            var refreshToken = "revoked-refresh-token";

            var revokedToken = new RefreshToken
            {
                Id = 1,
                UserId = 1,
                Token = refreshToken,
                IsRevoked = true,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(29)
            };

            _refreshTokenRepositoryMock
                .Setup(repository =>
                    repository.GetByTokenAsync(
                        refreshToken,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(revokedToken);

            _refreshTokenValidatorMock
                .Setup(validator =>
                    validator.ValidateRefreshTokenIsNotRevoked(
                        revokedToken))
                .Throws(
                    new ValidationException(
                        ValidationMessages.InvalidRefreshToken));

            var exception = await Assert.ThrowsExceptionAsync<ValidationException>(
                () => _authService.LogoutAsync(refreshToken));

            Assert.AreEqual(
                ValidationMessages.InvalidRefreshToken,
                exception.Message);

            _refreshTokenRepositoryMock.Verify(
                repository =>
                    repository.Update(
                        It.IsAny<RefreshToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task LogoutAsync_ValidToken_ShouldRevokeToken()
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

            _refreshTokenRepositoryMock
                .Setup(repository =>
                    repository.Update(existingRefreshToken));

            _unitOfWorkMock
                .Setup(unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _authService.LogoutAsync(refreshToken);

            Assert.IsTrue(existingRefreshToken.IsRevoked);

            _refreshTokenRepositoryMock.Verify(
                repository =>
                    repository.Update(existingRefreshToken),
                Times.Once);

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
