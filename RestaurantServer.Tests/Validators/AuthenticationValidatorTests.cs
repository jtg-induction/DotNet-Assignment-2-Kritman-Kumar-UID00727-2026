using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestaurantServer.Constants;
using RestaurantServer.Exceptions;
using RestaurantServer.Models;
using RestaurantServer.Validators.Implementations;

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
        public void ValidateUser_ValidUser_DoesNotThrowException()
        {
            var user = new User
            {
                IsActive = true
            };

            _authenticationValidator.ValidateUser(user);
        }

        [TestMethod]
        public void ValidateUser_NullUser_ThrowsValidationException()
        {
            var exception = Assert.ThrowsException<ValidationException>(
                () => _authenticationValidator.ValidateUser(null));

            Assert.AreEqual(
                ErrorMessages.InvalidCredentials,
                exception.Message);
        }

        [TestMethod]
        public void ValidateUserIsActive_ActiveUser_DoesNotThrowException()
        {
            var user = new User
            {
                IsActive = true
            };

            _authenticationValidator.ValidateUserIsActive(user);
        }

        [TestMethod]
        public void ValidateUserIsActive_InactiveUser_ThrowsValidationException()
        {
            var user = new User
            {
                IsActive = false
            };

            var exception = Assert.ThrowsException<ValidationException>(
                () => _authenticationValidator.ValidateUserIsActive(user));

            Assert.AreEqual(
                ErrorMessages.UserInactive,
                exception.Message);
        }

        [TestMethod]
        public void ValidatePassword_ValidPassword_DoesNotThrowException()
        {
            _authenticationValidator.ValidatePassword(true);
        }

        [TestMethod]
        public void ValidatePassword_InvalidPassword_ThrowsValidationException()
        {
            var exception = Assert.ThrowsException<ValidationException>(
                () => _authenticationValidator.ValidatePassword(false));

            Assert.AreEqual(
                ErrorMessages.InvalidCredentials,
                exception.Message);
        }
    }
}
