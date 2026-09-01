using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestaurantServer.Constants;
using RestaurantServer.DTOs.Requests;
using RestaurantServer.Enums;
using RestaurantServer.Exceptions;
using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.Services.Implementations;
using RestaurantServer.Services.Interfaces;
using RestaurantServer.Validators.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Tests
{
    [TestClass]
    public class AdminServiceTests
    {
        private Mock<IRestaurantRepository> _restaurantRepositoryMock;
        private Mock<IRestaurantOwnerRepository> _restaurantOwnerRepositoryMock;
        private Mock<IUsersRepository> _usersRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IRestaurantValidator> _restaurantValidatorMock;
        private Mock<IUserValidator> _userValidatorMock;
        private Mock<IUserSessionService> _userSessionServiceMock;

        private AdminService _adminService;

        [TestInitialize]
        public void Setup()
        {
            _restaurantRepositoryMock =
                new Mock<IRestaurantRepository>();

            _restaurantOwnerRepositoryMock =
                new Mock<IRestaurantOwnerRepository>();

            _usersRepositoryMock =
                new Mock<IUsersRepository>();

            _unitOfWorkMock =
                new Mock<IUnitOfWork>();

            _restaurantValidatorMock =
                new Mock<IRestaurantValidator>();

            _userValidatorMock =
                new Mock<IUserValidator>();

            _userSessionServiceMock =
                new Mock<IUserSessionService>();

            _adminService = new AdminService(
                _restaurantRepositoryMock.Object,
                _restaurantOwnerRepositoryMock.Object,
                _usersRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _restaurantValidatorMock.Object,
                _userValidatorMock.Object,
                _userSessionServiceMock.Object);
        }

        [TestMethod]
        public async Task CreateRestaurantAsync_UserNotFound_ShouldThrowValidationException()
        {
            var request = new CreateRestaurantRequest
            {
                RestaurantName = "Test Restaurant",
                Description = "Test Description",
                MobileNumber = "9876543210",
                AddressLine1 = "Test Address 1",
                AddressLine2 = "Test Address 2",
                City = "Test City",
                PostalCode = "110001",
                Country = "India",
                OwnersEmails = new List<string>
                {
                    "missing@example.com"
                }
            };

            _userSessionServiceMock
                .Setup(service => service.GetUserId())
                .Returns(1L);

            _usersRepositoryMock
                .Setup(repository =>
                    repository.GetUsersByEmailsAsync(
                        It.IsAny<List<string>>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User>());

            var exception =
                await Assert.ThrowsExceptionAsync<ValidationException>(
                    () => _adminService.CreateRestaurantAsync(
                        request,
                        CancellationToken.None));

            Assert.AreEqual(
                ValidationMessages.UserNotFound,
                exception.Message);

            _restaurantRepositoryMock.Verify(
                repository =>
                    repository.Add(It.IsAny<Restaurant>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<long?>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task CreateRestaurantAsync_InactiveUser_ShouldThrowValidationException()
        {
            var request = new CreateRestaurantRequest
            {
                RestaurantName = "Test Restaurant",
                Description = "Test Description",
                MobileNumber = "9876543210",
                AddressLine1 = "Test Address 1",
                AddressLine2 = "Test Address 2",
                City = "Test City",
                PostalCode = "110001",
                Country = "India",
                OwnersEmails = new List<string>
                {
                    "owner@example.com"
                }
            };

            var inactiveUser = new User
            {
                Id = 10,
                Name = "Inactive Owner",
                Email = "owner@example.com",
                Role = (int)UserRole.Admin,
                IsActive = false
            };

            _userSessionServiceMock
                .Setup(service => service.GetUserId())
                .Returns(1L);

            _usersRepositoryMock
                .Setup(repository =>
                    repository.GetUsersByEmailsAsync(
                        It.IsAny<List<string>>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User>
                {
                    inactiveUser
                });

            _userValidatorMock
                .Setup(validator =>
                    validator.IsUserNullOrDeactivated(
                        inactiveUser,
                        ValidationMessages.UserNotFound))
                .Throws(
                    new ValidationException(
                        ValidationMessages.UserNotFound));

            var exception =
                await Assert.ThrowsExceptionAsync<ValidationException>(
                    () => _adminService.CreateRestaurantAsync(
                        request,
                        CancellationToken.None));

            Assert.AreEqual(
                ValidationMessages.UserNotFound,
                exception.Message);

            _restaurantRepositoryMock.Verify(
                repository =>
                    repository.Add(It.IsAny<Restaurant>()),
                Times.Never);
        }

        [TestMethod]
        public async Task CreateRestaurantAsync_NonAdminUser_ShouldThrowValidationException()
        {
            var request = new CreateRestaurantRequest
            {
                RestaurantName = "Test Restaurant",
                Description = "Test Description",
                MobileNumber = "9876543210",
                AddressLine1 = "Test Address 1",
                AddressLine2 = "Test Address 2",
                City = "Test City",
                PostalCode = "110001",
                Country = "India",
                OwnersEmails = new List<string>
                {
                    "user@example.com"
                }
            };

            var user = new User
            {
                Id = 10,
                Name = "Normal User",
                Email = "user@example.com",
                Role = (int)UserRole.Customer,
                IsActive = true
            };

            _userSessionServiceMock
                .Setup(service => service.GetUserId())
                .Returns(1L);

            _usersRepositoryMock
                .Setup(repository =>
                    repository.GetUsersByEmailsAsync(
                        It.IsAny<List<string>>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User>
                {
                    user
                });

            _restaurantValidatorMock
                .Setup(validator =>
                    validator.ValidateAdminRole(user.Role))
                .Throws(
                    new ValidationException(
                        ValidationMessages.InvalidRole));

            var exception =
                await Assert.ThrowsExceptionAsync<ValidationException>(
                    () => _adminService.CreateRestaurantAsync(
                        request,
                        CancellationToken.None));

            Assert.AreEqual(
                ValidationMessages.InvalidRole,
                exception.Message);

            _restaurantRepositoryMock.Verify(
                repository =>
                    repository.Add(It.IsAny<Restaurant>()),
                Times.Never);
        }

        [TestMethod]
        public async Task CreateRestaurantAsync_MultipleOwners_ShouldCreateAllOwners()
        {
            var request = new CreateRestaurantRequest
            {
                RestaurantName = "Test Restaurant",
                Description = "Test Description",
                MobileNumber = "9876543210",
                AddressLine1 = "Test Address 1",
                AddressLine2 = "Test Address 2",
                City = "Test City",
                PostalCode = "110001",
                Country = "India",
                OwnersEmails = new List<string>
                {
                    "owner1@example.com",
                    "owner2@example.com"
                }
            };

            var user1 = new User
            {
                Id = 10,
                Name = "Owner One",
                Email = "owner1@example.com",
                Role = (int)UserRole.Admin,
                IsActive = true
            };

            var user2 = new User
            {
                Id = 20,
                Name = "Owner Two",
                Email = "owner2@example.com",
                Role = (int)UserRole.Admin,
                IsActive = true
            };

            _userSessionServiceMock
                .Setup(service => service.GetUserId())
                .Returns(1L);

            _usersRepositoryMock
                .Setup(repository =>
                    repository.GetUsersByEmailsAsync(
                        It.IsAny<List<string>>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User>
                {
                    user1,
                    user2
                });

            _restaurantRepositoryMock
                .Setup(repository =>
                    repository.Add(It.IsAny<Restaurant>()))
                .Returns(Task.CompletedTask);

            _restaurantOwnerRepositoryMock
                .Setup(repository =>
                    repository.Add(It.IsAny<RestaurantOwner>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<long?>(),
                        It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result =
                await _adminService.CreateRestaurantAsync(
                    request,
                    CancellationToken.None);

            Assert.IsNotNull(result);

            Assert.AreEqual(
                2,
                result.Owners.Count);

            Assert.AreEqual(
                (int)UserRole.Owner,
                user1.Role);

            Assert.AreEqual(
                (int)UserRole.Owner,
                user2.Role);

            _restaurantOwnerRepositoryMock.Verify(
                repository =>
                    repository.Add(It.IsAny<RestaurantOwner>()),
                Times.Exactly(2));

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        1L,
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [TestMethod]
        public async Task OnboardRestaurantOwnerAsync_RestaurantNotFound_ShouldThrowValidationException()
        {
            var request = new OnboardRestaurantOwnerRequest
            {
                Emails = new List<string>
                {
                    "owner@example.com"
                }
            };

            _restaurantRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAsync(
                        100L,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync((Restaurant)null);

            _restaurantValidatorMock
                .Setup(validator =>
                    validator.ValidateRestaurantExists(null))
                .Throws(
                    new ValidationException(
                        ValidationMessages.RestaurantNotExists));

            var exception =
                await Assert.ThrowsExceptionAsync<ValidationException>(
                    () => _adminService.OnboardRestaurantOwnerAsync(
                        100L,
                        request,
                        CancellationToken.None));

            Assert.AreEqual(
                ValidationMessages.RestaurantNotExists,
                exception.Message);

            _usersRepositoryMock.Verify(
                repository =>
                    repository.GetUsersByEmailsAsync(
                        It.IsAny<List<string>>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task OnboardRestaurantOwnerAsync_UserNotFound_ShouldThrowValidationException()
        {
            var request = new OnboardRestaurantOwnerRequest
            {
                Emails = new List<string>
                {
                    "missing@example.com"
                }
            };

            var restaurant = new Restaurant
            {
                Id = 100L,
                RestaurantName = "Test Restaurant",
                AddressLine1 = "Address",
                City = "Delhi",
                PostalCode = "110001",
                Country = "India"
            };

            _restaurantRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAsync(
                        100L,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(restaurant);

            _usersRepositoryMock
                .Setup(repository =>
                    repository.GetUsersByEmailsAsync(
                        It.IsAny<List<string>>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User>());

            var exception =
                await Assert.ThrowsExceptionAsync<ValidationException>(
                    () => _adminService.OnboardRestaurantOwnerAsync(
                        100L,
                        request,
                        CancellationToken.None));

            Assert.AreEqual(
                ValidationMessages.UserNotFound,
                exception.Message);

            _restaurantOwnerRepositoryMock.Verify(
                repository =>
                    repository.Add(It.IsAny<RestaurantOwner>()),
                Times.Never);
        }

        [TestMethod]
        public async Task OnboardRestaurantOwnerAsync_ValidRequest_ShouldOnboardOwners()
        {
            var request = new OnboardRestaurantOwnerRequest
            {
                Emails = new List<string>
                {
                    "owner@example.com"
                }
            };

            var restaurant = new Restaurant
            {
                Id = 100L,
                RestaurantName = "Test Restaurant",
                AddressLine1 = "Address",
                City = "Delhi",
                PostalCode = "110001",
                Country = "India"
            };

            var user = new User
            {
                Id = 10L,
                Name = "Owner",
                Email = "owner@example.com",
                Role = (int)UserRole.Admin,
                IsActive = true
            };

            _restaurantRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAsync(
                        100L,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(restaurant);

            _usersRepositoryMock
                .Setup(repository =>
                    repository.GetUsersByEmailsAsync(
                        It.IsAny<List<string>>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User>
                {
                    user
                });

            _restaurantOwnerRepositoryMock
                .Setup(repository =>
                    repository.GetOwnersByRestaurantAndUserIdsAsync(
                        100L,
                        It.IsAny<List<long>>(),
                        true,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RestaurantOwner>());

            _restaurantOwnerRepositoryMock
                .Setup(repository =>
                    repository.Add(It.IsAny<RestaurantOwner>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        null,
                        It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result =
                await _adminService.OnboardRestaurantOwnerAsync(
                    100L,
                    request,
                    CancellationToken.None);

            Assert.IsNotNull(result);

            Assert.AreEqual(
                100L,
                result.RestaurantId);

            Assert.AreEqual(
                1,
                result.Owners.Count);

            Assert.AreEqual(
                (int)UserRole.Owner,
                user.Role);

            Assert.AreEqual(
                SuccessMessages.ownersOnboardedSuccessful,
                result.Message);

            _restaurantOwnerRepositoryMock.Verify(
                repository =>
                    repository.Add(It.IsAny<RestaurantOwner>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        null,
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
