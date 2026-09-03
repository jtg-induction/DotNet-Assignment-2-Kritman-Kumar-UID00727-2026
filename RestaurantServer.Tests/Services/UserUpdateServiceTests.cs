using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestaurantServer.Constants;
using RestaurantServer.DTOs.Requests;
using RestaurantServer.Exceptions;
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
    public class UserUpdateServiceTests
    {
        private Mock<IUsersRepository> _usersRepositoryMock;
        private Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IUserValidator> _userValidatorMock;
        private Mock<IRequestValidator> _requestValidatorMock;

        private UserUpdateService _userUpdateService;

        [TestInitialize]
        public void Setup()
        {
            _usersRepositoryMock = new Mock<IUsersRepository>();
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userValidatorMock = new Mock<IUserValidator>();
            _requestValidatorMock = new Mock<IRequestValidator>();

            _unitOfWorkMock
                .Setup(unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _refreshTokenRepositoryMock
                .Setup(repository =>
                    repository.RevokeAllByUserIdAsync(
                        It.IsAny<long>(),
                        It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _userUpdateService = new UserUpdateService(
                _usersRepositoryMock.Object,
                _refreshTokenRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _userValidatorMock.Object,
                _requestValidatorMock.Object);
        }

        [TestMethod]
        public async Task UpdateAccountAsync_UserNotFound_ThrowsValidationException()
        {
            var userId = 1L;

            var request = new UpdateAccountRequest
            {
                Name = "Updated User",
                MobileNumber = "9876543210"
            };

            _usersRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAsync(
                        userId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            _userValidatorMock
                .Setup(validator =>
                    validator.ValidateUserExists(null))
                .Throws(
                    new ValidationException(
                        ErrorMessages.UserNotFound));

            var exception = await Assert.ThrowsExceptionAsync<ValidationException>(
                () => _userUpdateService.UpdateAccountAsync(
                    userId,
                    request));

            Assert.AreEqual(
                ErrorMessages.UserNotFound,
                exception.Message);

            _userValidatorMock.Verify(
                validator =>
                    validator.ValidateUserExists(null),
                Times.Once);

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task UpdateAccountAsync_MobileNumberExists_ThrowsValidationException()
        {
            var userId = 1L;

            var request = new UpdateAccountRequest
            {
                Name = "Updated User",
                MobileNumber = "9876543210"
            };

            _userValidatorMock
                .Setup(validator =>
                    validator.ValidateMobileNumberIsUnique(
                        "9876543210",
                        userId))
                .Throws(
                    new ValidationException(
                        ErrorMessages.MobileNumberAlreadyExists));

            var exception = await Assert.ThrowsExceptionAsync<ValidationException>(
                () => _userUpdateService.UpdateAccountAsync(
                    userId,
                    request));

            Assert.AreEqual(
                ErrorMessages.MobileNumberAlreadyExists,
                exception.Message);

            _usersRepositoryMock.Verify(
                repository =>
                    repository.GetByIdAsync(
                        It.IsAny<long>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task UpdateAccountAsync_ValidRequest_UpdatesUserAndReturnsResponse()
        {
            var userId = 1L;

            var user = new User
            {
                Id = userId,
                Name = "Old Name",
                Email = "test@example.com",
                MobileNumber = "9876543210",
                IsActive = true
            };

            var request = new UpdateAccountRequest
            {
                Name = "  Updated Name  ",
                MobileNumber = "9123456789"
            };

            _usersRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAsync(
                        userId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var beforeUpdate = DateTime.UtcNow;

            var result = await _userUpdateService.UpdateAccountAsync(
                userId,
                request);

            var afterUpdate = DateTime.UtcNow;

            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.UserId);
            Assert.AreEqual("Updated Name", result.Name);
            Assert.AreEqual("test@example.com", result.Email);
            Assert.AreEqual("9123456789", result.MobileNumber);

            Assert.AreEqual("Updated Name", user.Name);
            Assert.AreEqual("9123456789", user.MobileNumber);

            Assert.IsTrue(
                user.UpdatedAt >= beforeUpdate &&
                user.UpdatedAt <= afterUpdate);

            _requestValidatorMock.Verify(
                validator =>
                    validator.IsRequestNull(request),
                Times.Once);

            _userValidatorMock.Verify(
                validator =>
                    validator.ValidateMobileNumberIsUnique(
                        "9123456789",
                        userId),
                Times.Once);

            _userValidatorMock.Verify(
                validator =>
                    validator.ValidateUserExists(user),
                Times.Once);

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [TestMethod]
        public async Task DeactivateAccountAsync_UserNotFound_ThrowsValidationException()
        {
            var userId = 1L;

            _usersRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAsync(
                        userId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            _userValidatorMock
                .Setup(validator =>
                    validator.IsUserNullOrDeactivated(null, ""))
                .Throws(
                    new ValidationException(
                        ErrorMessages.InvalidRefreshToken));

            var exception = await Assert.ThrowsExceptionAsync<ValidationException>(
                () => _userUpdateService.DeactivateAccountAsync(userId));

            Assert.AreEqual(
                ErrorMessages.InvalidRefreshToken,
                exception.Message);

            _userValidatorMock.Verify(
                validator =>
                    validator.IsUserNullOrDeactivated(null, ""),
                Times.Once);

            _refreshTokenRepositoryMock.Verify(
                repository =>
                    repository.RevokeAllByUserIdAsync(
                        userId,
                        It.IsAny<CancellationToken>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task DeactivateAccountAsync_UserInactive_ThrowsValidationException()
        {
            var userId = 1L;

            var user = new User
            {
                Id = userId,
                Name = "Inactive User",
                Email = "inactive@example.com",
                IsActive = false
            };

            _usersRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAsync(
                        userId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _userValidatorMock
                .Setup(validator =>
                    validator.IsUserNullOrDeactivated(user, ""))
                .Throws(
                    new ValidationException(
                        ErrorMessages.InvalidRefreshToken));

            var exception = await Assert.ThrowsExceptionAsync<ValidationException>(
                () => _userUpdateService.DeactivateAccountAsync(userId));

            Assert.AreEqual(
                ErrorMessages.InvalidRefreshToken,
                exception.Message);

            _userValidatorMock.Verify(
                validator =>
                    validator.IsUserNullOrDeactivated(user, ""),
                Times.Once);

            Assert.IsFalse(user.IsActive);

            _refreshTokenRepositoryMock.Verify(
                repository =>
                    repository.RevokeAllByUserIdAsync(
                        userId,
                        It.IsAny<CancellationToken>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task DeactivateAccountAsync_ActiveUser_DeactivatesAccountAndRevokesTokens()
        {
            var userId = 1L;

            var user = new User
            {
                Id = userId,
                Name = "Test User",
                Email = "test@example.com",
                IsActive = true
            };

            _usersRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAsync(
                        userId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var beforeUpdate = DateTime.UtcNow;

            var result = await _userUpdateService.DeactivateAccountAsync(userId);

            var afterUpdate = DateTime.UtcNow;

            Assert.AreEqual(
                SuccessMessages.AccountDeactivatedSuccessful,
                result);

            Assert.IsFalse(user.IsActive);

            Assert.IsTrue(
                user.UpdatedAt >= beforeUpdate &&
                user.UpdatedAt <= afterUpdate);

            _userValidatorMock.Verify(
                validator =>
                    validator.IsUserNullOrDeactivated(user, ""),
                Times.Once);

            _refreshTokenRepositoryMock.Verify(
                repository =>
                    repository.RevokeAllByUserIdAsync(
                        userId,
                        It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
