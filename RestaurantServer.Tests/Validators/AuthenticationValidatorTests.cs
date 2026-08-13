using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestaurantServer.Constants;
using RestaurantServer.Exceptions;
using RestaurantServer.Models;
using RestaurantServer.Validators.Implementations;
using System;

namespace RestaurantServer.Tests
{
    [TestClass]
    public class AuthenticationValidatorTests
    {
        private AuthenticationValidator _authenticationValidator;

        [TestInitialize]
        public void Setup()
        {
            _authenticationValidator = new AuthenticationValidator();
        }

        [TestMethod]
        public void ValidateUserIsActive_ActiveUser_DoesNotThrowException()
        {
            var user = new User
            {
                Id = 1,
                Name = "Test User",
                Email = "test@example.com",
                IsActive = true
            };

            _authenticationValidator.ValidateUserIsActive(user);
        }

        [TestMethod]
        public void ValidateUserIsActive_InactiveUser_ThrowsValidationException()
        {
            var user = new User
            {
                Id = 1,
                Name = "Inactive User",
                Email = "inactive@example.com",
                IsActive = false
            };

            var exception = Assert.ThrowsException<ValidationException>(
                () => _authenticationValidator.ValidateUserIsActive(user));

            Assert.AreEqual(
                ValidationMessages.UserInactive,
                exception.Message);
        }

        [TestMethod]
        public void ValidateRefreshTokenIsNotRevoked_ActiveToken_DoesNotThrowException()
        {
            var refreshToken = new RefreshToken
            {
                Id = 1,
                UserId = 1,
                Token = "valid-refresh-token",
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(29)
            };

            _authenticationValidator.ValidateRefreshTokenIsNotRevoked(
                refreshToken);
        }

        [TestMethod]
        public void ValidateRefreshTokenIsNotRevoked_RevokedToken_ThrowsValidationException()
        {
            var refreshToken = new RefreshToken
            {
                Id = 1,
                UserId = 1,
                Token = "revoked-refresh-token",
                IsRevoked = true,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(29)
            };

            var exception = Assert.ThrowsException<ValidationException>(
                () => _authenticationValidator
                    .ValidateRefreshTokenIsNotRevoked(refreshToken));

            Assert.AreEqual(
                ValidationMessages.InvalidRefreshToken,
                exception.Message);
        }

        [TestMethod]
        public void ValidateRefreshTokenUser_ActiveUser_DoesNotThrowException()
        {
            var user = new User
            {
                Id = 1,
                Name = "Test User",
                Email = "test@example.com",
                IsActive = true
            };

            _authenticationValidator.ValidateRefreshTokenUser(user);
        }

        [TestMethod]
        public void ValidateRefreshTokenUser_InactiveUser_ThrowsValidationException()
        {
            var user = new User
            {
                Id = 1,
                Name = "Inactive User",
                Email = "inactive@example.com",
                IsActive = false
            };

            var exception = Assert.ThrowsException<ValidationException>(
                () => _authenticationValidator
                    .ValidateRefreshTokenUser(user));

            Assert.AreEqual(
                ValidationMessages.InvalidRefreshToken,
                exception.Message);
        }

        [TestMethod]
        public void ValidateRefreshTokenUser_NullUser_ThrowsValidationException()
        {
            var exception = Assert.ThrowsException<ValidationException>(
                () => _authenticationValidator
                    .ValidateRefreshTokenUser(null));

            Assert.AreEqual(
                ValidationMessages.InvalidRefreshToken,
                exception.Message);
        }
    }
}
