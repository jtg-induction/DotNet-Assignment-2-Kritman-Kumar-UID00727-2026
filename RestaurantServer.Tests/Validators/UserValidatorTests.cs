using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestaurantServer.Constants;
using RestaurantServer.Exceptions;
using RestaurantServer.Models;
using RestaurantServer.Validators.Implementations;
using System;

namespace RestaurantServer.Tests.Validators
{
    [TestClass]
    public class UserValidatorTests
    {
        private UserValidator _userValidator;

        [TestInitialize]
        public void Setup()
        {
            _userValidator = new UserValidator();
        }

        [TestMethod]
        public void ValidateUserExists_UserExists_DoesNotThrowException()
        {
            var user = new User
            {
                Id = 1,
                Name = "Test User",
                Email = "test@example.com",
                IsActive = true
            };

            _userValidator.ValidateUserExists(user);
        }

        [TestMethod]
        public void ValidateUserExists_UserIsNull_ThrowsValidationException()
        {
            var exception = Assert.ThrowsException<ValidationException>(
                () => _userValidator.ValidateUserExists(null));

            Assert.AreEqual(
                ValidationMessages.UserNotFound,
                exception.Message);
        }
    }
}
