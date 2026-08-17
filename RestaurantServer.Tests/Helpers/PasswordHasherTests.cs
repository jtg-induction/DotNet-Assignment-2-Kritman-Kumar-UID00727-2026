using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestaurantServer.Helpers.Implementations;

namespace RestaurantServer.Tests.Helpers
{
    [TestClass]
    public class PasswordHasherTests
    {
        private PasswordHasher _passwordHasher;

        [TestInitialize]
        public void Setup()
        {
            _passwordHasher = new PasswordHasher();
        }

        [TestMethod]
        public void HashPassword_ShouldReturnNonEmptyHash()
        {
            var password = "Password@123";

            var passwordHash =
                _passwordHasher.HashPassword(password);

            Assert.IsFalse(string.IsNullOrWhiteSpace(passwordHash));
        }

        [TestMethod]
        public void HashPassword_ShouldNotReturnOriginalPassword()
        {
            var password = "Password@123";

            var passwordHash =
                _passwordHasher.HashPassword(password);

            Assert.AreNotEqual(password, passwordHash);
        }

        [TestMethod]
        public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
        {
            var password = "Password@123";

            var passwordHash =
                _passwordHasher.HashPassword(password);

            var result =
                _passwordHasher.VerifyPassword(
                    password,
                    passwordHash);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
        {
            var password = "Password@123";
            var incorrectPassword = "WrongPassword@123";

            var passwordHash =
                _passwordHasher.HashPassword(password);

            var result =
                _passwordHasher.VerifyPassword(
                    incorrectPassword,
                    passwordHash);

            Assert.IsFalse(result);
        }
    }
}
