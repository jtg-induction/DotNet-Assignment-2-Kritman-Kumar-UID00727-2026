using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestaurantServer.Exceptions;
using System;

namespace RestaurantServer.Tests.Exceptions
{
    [TestClass]
    public class ValidationExceptionTests
    {
        [TestMethod]
        public void Constructor_WithMessage_ShouldSetMessage()
        {
            var expectedMessage =
                "Invalid email address.";

            var exception =
                new ValidationException(expectedMessage);

            Assert.AreEqual(
                expectedMessage,
                exception.Message);
        }

        [TestMethod]
        public void Constructor_WithMessage_ShouldHaveNoInnerException()
        {
            var exception =
                new ValidationException(
                    "Validation failed.");

            Assert.IsNull(
                exception.InnerException);
        }

        [TestMethod]
        public void Constructor_WithMessageAndInnerException_ShouldSetMessage()
        {
            var expectedMessage =
                "Validation failed.";

            var innerException =
                new Exception(
                    "Original exception.");

            var exception =
                new ValidationException(
                    expectedMessage,
                    innerException);

            Assert.AreEqual(
                expectedMessage,
                exception.Message);
        }

        [TestMethod]
        public void Constructor_WithMessageAndInnerException_ShouldSetInnerException()
        {
            var innerException =
                new Exception(
                    "Original exception.");

            var exception =
                new ValidationException(
                    "Validation failed.",
                    innerException);

            Assert.AreSame(
                innerException,
                exception.InnerException);
        }

        [TestMethod]
        public void ValidationException_ShouldInheritFromException()
        {
            var exception =
                new ValidationException(
                    "Validation failed.");

            Assert.IsInstanceOfType(
                exception,
                typeof(Exception));
        }
    }
}
