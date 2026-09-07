using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestaurantServer.Constants;
using RestaurantServer.Exceptions;
using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.Validators.Implementations;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Tests
{
    [TestClass]
    public class RestaurantValidatorTests
    {
        private Mock<IRestaurantRepository> _restaurantRepositoryMock;
        private RestaurantValidator _restaurantValidator;

        [TestInitialize]
        public void Setup()
        {
            _restaurantRepositoryMock =
                new Mock<IRestaurantRepository>();

            _restaurantValidator =
                new RestaurantValidator(
                    _restaurantRepositoryMock.Object);
        }

        [TestMethod]
        public void ValidateRestaurantExists_NullRestaurant_ThrowsValidationException()
        {
            var exception =
                Assert.ThrowsException<ValidationException>(
                    () => _restaurantValidator.ValidateRestaurantExists(null));

            Assert.AreEqual(
                ValidationMessages.RestaurantNotExists,
                exception.Message);
        }

        [TestMethod]
        public void ValidateRestaurantExists_DeletedRestaurant_ThrowsValidationException()
        {
            var restaurant = new Restaurant
            {
                IsDeleted = true
            };

            var exception =
                Assert.ThrowsException<ValidationException>(
                    () => _restaurantValidator.ValidateRestaurantExists(
                        restaurant));

            Assert.AreEqual(
                ValidationMessages.RestaurantNotavailable,
                exception.Message);
        }

        [TestMethod]
        public void ValidateRestaurantExists_ActiveRestaurant_DoesNotThrowException()
        {
            var restaurant = new Restaurant
            {
                IsDeleted = false
            };

            _restaurantValidator.ValidateRestaurantExists(restaurant);
        }

        [TestMethod]
        public void ValidateOwnerRelationshipDoesNotExist_ExistingRelationship_ThrowsValidationException()
        {
            var restaurantOwner = new RestaurantOwner();

            var exception =
                Assert.ThrowsException<ValidationException>(
                    () => _restaurantValidator
                        .ValidateOwnerRelationshipDoesNotExist(
                            restaurantOwner));

            Assert.AreEqual(
                ValidationMessages.OwnerRelationshipAlreadyExists,
                exception.Message);
        }

        [TestMethod]
        public void ValidateOwnerRelationshipDoesNotExist_NullRelationship_DoesNotThrowException()
        {
            _restaurantValidator
                .ValidateOwnerRelationshipDoesNotExist(null);
        }

        [TestMethod]
        public async Task ValidateMobileNumber_MobileNumberExists_ThrowsValidationException()
        {
            var mobileNumber = "9876543210";

            _restaurantRepositoryMock
                .Setup(repository =>
                    repository.ExistsByMobileNumberAsync(
                        mobileNumber,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var exception =
                await Assert.ThrowsExceptionAsync<ValidationException>(
                    () => _restaurantValidator
                        .ValidateMobileNumber(mobileNumber));

            Assert.AreEqual(
                ValidationMessages.RestaurantMobileNumberAlreadyExists,
                exception.Message);

            _restaurantRepositoryMock.Verify(
                repository =>
                    repository.ExistsByMobileNumberAsync(
                        mobileNumber,
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [TestMethod]
        public async Task ValidateMobileNumber_MobileNumberDoesNotExist_DoesNotThrowException()
        {
            var mobileNumber = "9876543210";

            _restaurantRepositoryMock
                .Setup(repository =>
                    repository.ExistsByMobileNumberAsync(
                        mobileNumber,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            await _restaurantValidator
                .ValidateMobileNumber(mobileNumber);

            _restaurantRepositoryMock.Verify(
                repository =>
                    repository.ExistsByMobileNumberAsync(
                        mobileNumber,
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [TestMethod]
        public async Task ValidateMobileNumber_WithWhitespace_TrimsMobileNumberBeforeChecking()
        {
            var mobileNumber = " 9876543210 ";
            var expectedMobileNumber = "9876543210";

            _restaurantRepositoryMock
                .Setup(repository =>
                    repository.ExistsByMobileNumberAsync(
                        expectedMobileNumber,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            await _restaurantValidator
                .ValidateMobileNumber(mobileNumber);

            _restaurantRepositoryMock.Verify(
                repository =>
                    repository.ExistsByMobileNumberAsync(
                        expectedMobileNumber,
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [TestMethod]
        public async Task ValidateMobileNumber_NullMobileNumber_DoesNotThrowWhenRepositoryReturnsFalse()
        {
            _restaurantRepositoryMock
                .Setup(repository =>
                    repository.ExistsByMobileNumberAsync(
                        null,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            await _restaurantValidator
                .ValidateMobileNumber(null);

            _restaurantRepositoryMock.Verify(
                repository =>
                    repository.ExistsByMobileNumberAsync(
                        null,
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
