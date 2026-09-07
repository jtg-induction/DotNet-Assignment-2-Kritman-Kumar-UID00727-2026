using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestaurantServer.Constants;
using RestaurantServer.DTOs.Requests;
using RestaurantServer.Exceptions;
using RestaurantServer.Validators.Implementations;

namespace RestaurantServer.Tests.Validators
{
    [TestClass]
    public class ReportValidatorTests
    {
        private ReportValidator _reportValidator;

        [TestInitialize]
        public void Setup()
        {
            _reportValidator = new ReportValidator();
        }

        [TestMethod]
        public void ValidateTopOrderedItemsRequest_NullRequest_ThrowsValidationException()
        {
            var exception = Assert.ThrowsException<ValidationException>(
                () => _reportValidator.ValidateTopOrderedItemsRequest(null));

            Assert.AreEqual(
                ValidationMessages.ReportRequestRequired,
                exception.Message);
        }

        [TestMethod]
        public void ValidateTopOrderedItemsRequest_InvalidTopItems_ThrowsValidationException()
        {
            var request = new TopOrderedItemsRequest
            {
                TopItems = 0
            };

            var exception = Assert.ThrowsException<ValidationException>(
                () => _reportValidator.ValidateTopOrderedItemsRequest(request));

            Assert.AreEqual(
                ValidationMessages.InvalidTopItemsCount,
                exception.Message);
        }

        [TestMethod]
        public void ValidateTopOrderedItemsRequest_InvalidDateRange_ThrowsValidationException()
        {
            var request = new TopOrderedItemsRequest
            {
                TopItems = 10,
                StartDate = new System.DateTime(2025, 2, 1),
                EndDate = new System.DateTime(2025, 1, 1)
            };

            var exception = Assert.ThrowsException<ValidationException>(
                () => _reportValidator.ValidateTopOrderedItemsRequest(request));

            Assert.AreEqual(
                ValidationMessages.InvalidDateRange,
                exception.Message);
        }

        [TestMethod]
        public void ValidateFrequentlyBoughtTogetherRequest_InvalidRestaurantId_ThrowsValidationException()
        {
            var request = new FrequentlyBoughtTogetherRequest
            {
                TopPairs = 5
            };

            var exception = Assert.ThrowsException<ValidationException>(
                () => _reportValidator.ValidateFrequentlyBoughtTogetherRequest(0, request));

            Assert.AreEqual(
                ValidationMessages.InvalidRestaurantId,
                exception.Message);
        }

        [TestMethod]
        public void ValidateFrequentlyBoughtTogetherRequest_InvalidTopPairs_ThrowsValidationException()
        {
            var request = new FrequentlyBoughtTogetherRequest
            {
                TopPairs = 0
            };

            var exception = Assert.ThrowsException<ValidationException>(
                () => _reportValidator.ValidateFrequentlyBoughtTogetherRequest(1, request));

            Assert.AreEqual(
                ValidationMessages.InvalidTopPairsCount,
                exception.Message);
        }
    }
}
