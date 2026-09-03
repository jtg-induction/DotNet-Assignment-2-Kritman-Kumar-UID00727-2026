using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestaurantServer.Constants;
using RestaurantServer.Exceptions;
using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.Validators.Implementations;

namespace RestaurantServer.Tests.Validators
{
    [TestClass]
    public class UserValidatorTests
    {
        private UserValidator _userValidator;
        private Mock<IUsersRepository> _usersRepository;

        [TestInitialize]
        public void Setup()
        {
            _usersRepository = new Mock<IUsersRepository>();
            _userValidator = new UserValidator(_usersRepository.Object);
        }

        [TestMethod]
        public void ValidateUserExists_UserExists_DoesNotThrowException()
        {
            var user = new User
            {
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
                ErrorMessages.UserNotFound,
                exception.Message);
        }

        [TestMethod]
        public void ValidateUserId_MatchingUserIds_DoesNotThrowException()
        {
            _userValidator.ValidateUserId(1, 1);
        }

        [TestMethod]
        public void ValidateUserId_DifferentUserIds_ThrowsValidationException()
        {
            var exception = Assert.ThrowsException<ValidationException>(
                () => _userValidator.ValidateUserId(1, 2));

            Assert.AreEqual(
                ErrorMessages.NotAuthorized,
                exception.Message);
        }

        [TestMethod]
        public void IsUserNullOrDeactivated_ActiveUser_DoesNotThrowException()
        {
            var user = new User
            {
                IsActive = true
            };

            _userValidator.IsUserNullOrDeactivated(user);
        }

        [TestMethod]
        public void IsUserNullOrDeactivated_NullUser_ThrowsValidationException()
        {
            var exception = Assert.ThrowsException<ValidationException>(
                () => _userValidator.IsUserNullOrDeactivated(null));

            Assert.AreEqual(
                ErrorMessages.InvalidRefreshToken,
                exception.Message);
        }

        [TestMethod]
        public void IsUserNullOrDeactivated_InactiveUser_ThrowsValidationException()
        {
            var user = new User
            {
                IsActive = false
            };

            var exception = Assert.ThrowsException<ValidationException>(
                () => _userValidator.IsUserNullOrDeactivated(user));

            Assert.AreEqual(
                ErrorMessages.InvalidRefreshToken,
                exception.Message);
        }

        [TestMethod]
        public void ValidateMobileNumberIsUnique_EmptyMobileNumber_DoesNotThrowException()
        {
            _userValidator.ValidateMobileNumberIsUnique("", 1);
        }

        [TestMethod]
        public void ValidateMobileNumberIsUnique_MobileNumberDoesNotExist_DoesNotThrowException()
        {
            _usersRepository
                .Setup(x => x.IsMobileNumberExists("9876543210", 1))
                .Returns(false);

            _userValidator.ValidateMobileNumberIsUnique("9876543210", 1);
        }

        [TestMethod]
        public void ValidateMobileNumberIsUnique_MobileNumberExists_ThrowsValidationException()
        {
            _usersRepository
                .Setup(x => x.IsMobileNumberExists("9876543210", 1))
                .Returns(true);

            var exception = Assert.ThrowsException<ValidationException>(
                () => _userValidator.ValidateMobileNumberIsUnique("9876543210", 1));

            Assert.AreEqual(
                ErrorMessages.MobileNumberAlreadyExists,
                exception.Message);
        }
    }
}
