using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestaurantServer.Constants;
using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using RestaurantServer.Enums;
using RestaurantServer.Exceptions;
using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.Services.Implementations;
using RestaurantServer.Validators.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Tests
{
    [TestClass]
    public class AdminServiceTests
    {
        private Mock<IRestaurantRepository> _restaurantRepositoryMock;
        private Mock<IRestaurantOwnerRepository> _restaurantOwnerRepositoryMock;
        private Mock<IAuthRepository> _authRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IRestaurantValidator> _restaurantValidatorMock;
        private Mock<IUserValidator> _userValidatorMock;

        private AdminService _adminService;

        [TestInitialize]
        public void Setup()
        {
            _restaurantRepositoryMock =
                new Mock<IRestaurantRepository>();

            _restaurantOwnerRepositoryMock =
                new Mock<IRestaurantOwnerRepository>();

            _authRepositoryMock =
                new Mock<IAuthRepository>();

            _unitOfWorkMock =
                new Mock<IUnitOfWork>();

            _restaurantValidatorMock =
                new Mock<IRestaurantValidator>();

            _userValidatorMock =
                new Mock<IUserValidator>();

            _unitOfWorkMock
                .Setup(unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _restaurantRepositoryMock
                .Setup(repository =>
                    repository.Add(It.IsAny<Restaurant>()))
                .Returns(Task.CompletedTask);

            _restaurantOwnerRepositoryMock
                .Setup(repository =>
                    repository.Add(It.IsAny<RestaurantOwner>()))
                .Returns(Task.CompletedTask);

            _adminService = new AdminService(
                _restaurantRepositoryMock.Object,
                _restaurantOwnerRepositoryMock.Object,
                _authRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _restaurantValidatorMock.Object,
                _userValidatorMock.Object);
        }

        // ============================================================
        // CreateRestaurantAsync
        // ============================================================

        [TestMethod]
        public async Task CreateRestaurantAsync_UserNotFound_ThrowsValidationException()
        {
            var createdBy = 1L;

            var request = new CreateRestaurantRequest
            {
                OwnerEmail = "owner@example.com",
                MobileNumber = "9876543210"
            };

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetUserByEmailAsync(
                        request.OwnerEmail,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            _userValidatorMock
                .Setup(validator =>
                    validator.IsUserNullOrDeactivated(
                        null,
                        ValidationMessages.UserNotFound))
                .Throws(
                    new ValidationException(
                        ValidationMessages.UserNotFound));

            var exception =
                await Assert.ThrowsExceptionAsync<ValidationException>(
                    () => _adminService.CreateRestaurantAsync(
                        request,
                        createdBy));

            Assert.AreEqual(
                ValidationMessages.UserNotFound,
                exception.Message);

            _restaurantRepositoryMock.Verify(
                repository =>
                    repository.Add(It.IsAny<Restaurant>()),
                Times.Never);

            _restaurantOwnerRepositoryMock.Verify(
                repository =>
                    repository.Add(It.IsAny<RestaurantOwner>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task CreateRestaurantAsync_AdminRoleValidationFails_ThrowsValidationException()
        {
            var createdBy = 1L;

            var request = new CreateRestaurantRequest
            {
                OwnerEmail = "owner@example.com",
                MobileNumber = "9876543210"
            };

            var owner = new User
            {
                Id = 2L,
                Email = request.OwnerEmail,
                IsActive = true,
                Role = (int)UserRole.Admin
            };

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetUserByEmailAsync(
                        request.OwnerEmail,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(owner);

            _restaurantValidatorMock
                .Setup(validator =>
                    validator.ValidateAdminRole(owner.Role))
                .Throws(
                    new ValidationException(
                        ValidationMessages.InvalidRestaurantOwner));

            var exception =
                await Assert.ThrowsExceptionAsync<ValidationException>(
                    () => _adminService.CreateRestaurantAsync(
                        request,
                        createdBy));

            Assert.AreEqual(
                ValidationMessages.InvalidRestaurantOwner,
                exception.Message);

            _restaurantRepositoryMock.Verify(
                repository =>
                    repository.Add(It.IsAny<Restaurant>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task CreateRestaurantAsync_MobileNumberExists_ThrowsValidationException()
        {
            var createdBy = 1L;

            var request = new CreateRestaurantRequest
            {
                OwnerEmail = "owner@example.com",
                MobileNumber = "9876543210"
            };

            var owner = new User
            {
                Id = 2L,
                Email = request.OwnerEmail,
                IsActive = true,
                Role = (int)UserRole.Owner
            };

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetUserByEmailAsync(
                        request.OwnerEmail,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(owner);

            _restaurantValidatorMock
                .Setup(validator =>
                    validator.ValidateMobileNumber(
                        request.MobileNumber))
                .ThrowsAsync(
                    new ValidationException(
                        ValidationMessages.RestaurantMobileNumberAlreadyExists));

            var exception =
                await Assert.ThrowsExceptionAsync<ValidationException>(
                    () => _adminService.CreateRestaurantAsync(
                        request,
                        createdBy));

            Assert.AreEqual(
                ValidationMessages.RestaurantMobileNumberAlreadyExists,
                exception.Message);

            _restaurantRepositoryMock.Verify(
                repository =>
                    repository.Add(It.IsAny<Restaurant>()),
                Times.Never);

            _restaurantOwnerRepositoryMock.Verify(
                repository =>
                    repository.Add(It.IsAny<RestaurantOwner>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task CreateRestaurantAsync_ValidRequest_CreatesRestaurantAndOwner()
        {
            var createdBy = 1L;

            var request = new CreateRestaurantRequest
            {
                RestaurantName = "Test Restaurant",
                Description = "Test restaurant description",
                MobileNumber = "9876543210",
                AddressLine1 = "123 Main Street",
                AddressLine2 = "Near City Mall",
                City = "Dehradun",
                Country = "India",
                PostalCode = "248001",
                OwnerEmail = "owner@example.com"
            };

            var owner = new User
            {
                Id = 2L,
                Email = request.OwnerEmail,
                IsActive = true,
                Role = (int)UserRole.Customer
            };

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetUserByEmailAsync(
                        request.OwnerEmail,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(owner);

            _restaurantValidatorMock
                .Setup(validator =>
                    validator.ValidateMobileNumber(
                        request.MobileNumber))
                .Returns(Task.CompletedTask);

            Restaurant createdRestaurant = null;
            RestaurantOwner createdRestaurantOwner = null;

            _restaurantRepositoryMock
                .Setup(repository =>
                    repository.Add(It.IsAny<Restaurant>()))
                .Callback<Restaurant>(
                    restaurant => createdRestaurant = restaurant)
                .Returns(Task.CompletedTask);

            _restaurantOwnerRepositoryMock
                .Setup(repository =>
                    repository.Add(It.IsAny<RestaurantOwner>()))
                .Callback<RestaurantOwner>(
                    restaurantOwner => createdRestaurantOwner = restaurantOwner)
                .Returns(Task.CompletedTask);

            var result = await _adminService.CreateRestaurantAsync(
                request,
                createdBy);

            Assert.IsNotNull(result);
            Assert.IsNotNull(createdRestaurant);
            Assert.IsNotNull(createdRestaurantOwner);

            Assert.AreEqual(
                createdBy,
                createdRestaurant.CreatedBy);

            Assert.AreEqual(
                (int)UserRole.Owner,
                owner.Role);

            _authRepositoryMock.Verify(
                repository =>
                    repository.GetUserByEmailAsync(
                        request.OwnerEmail,
                        It.IsAny<CancellationToken>()),
                Times.Once);

            _restaurantValidatorMock.Verify(
                validator =>
                    validator.ValidateAdminRole(
                        (int)UserRole.Customer),
                Times.Once);

            _restaurantValidatorMock.Verify(
                validator =>
                    validator.ValidateMobileNumber(
                        request.MobileNumber),
                Times.Once);

            _restaurantRepositoryMock.Verify(
                repository =>
                    repository.Add(
                        It.IsAny<Restaurant>()),
                Times.Once);

            _restaurantOwnerRepositoryMock.Verify(
                repository =>
                    repository.Add(
                        It.IsAny<RestaurantOwner>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }
        // ============================================================
        // OnboardRestaurantOwnerAsync
        // ============================================================

        [TestMethod]
        public async Task OnboardRestaurantOwnerAsync_EmptyEmails_ThrowsValidationException()
        {
            var restaurantId = 1L;

            var request = new OnboardRestaurantOwnerRequest
            {
                Emails = new List<string>()
            };

            _restaurantValidatorMock
                .Setup(validator =>
                    validator.IsOwnersEmailEmpty(request.Emails))
                .Throws(
                    new ValidationException(
                        ValidationMessages.OnboardRestaurantOwnerEmailsMinLength));

            var exception =
                await Assert.ThrowsExceptionAsync<ValidationException>(
                    () => _adminService.OnboardRestaurantOwnerAsync(
                        restaurantId,
                        request));

            Assert.AreEqual(
                ValidationMessages.OnboardRestaurantOwnerEmailsMinLength,
                exception.Message);

            _restaurantRepositoryMock.Verify(
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
        public async Task OnboardRestaurantOwnerAsync_RestaurantNotFound_ThrowsValidationException()
        {
            var restaurantId = 1L;

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
                        restaurantId,
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
                        restaurantId,
                        request));

            Assert.AreEqual(
                ValidationMessages.RestaurantNotExists,
                exception.Message);

            _authRepositoryMock.Verify(
                repository =>
                    repository.GetUserByEmailAsync(
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task OnboardRestaurantOwnerAsync_DuplicateEmail_ThrowsValidationException()
        {
            var restaurantId = 1L;

            var request = new OnboardRestaurantOwnerRequest
            {
                Emails = new List<string>
                {
                    "owner@example.com",
                    "owner@example.com"
                }
            };

            var restaurant = new Restaurant
            {
                Id = restaurantId
            };

            _restaurantRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAsync(
                        restaurantId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(restaurant);

            var owner = new User
            {
                Id = 2L,
                Email = "owner@example.com",
                IsActive = true,
                Role = (int)UserRole.Owner
            };

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetUserByEmailAsync(
                        "owner@example.com",
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(owner);

            var exception =
                await Assert.ThrowsExceptionAsync<ValidationException>(
                    () => _adminService.OnboardRestaurantOwnerAsync(
                        restaurantId,
                        request));

            Assert.AreEqual(
                ValidationMessages.DuplicateOwnerEmail,
                exception.Message);

            _restaurantOwnerRepositoryMock.Verify(
                repository =>
                    repository.Add(
                        It.IsAny<RestaurantOwner>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task OnboardRestaurantOwnerAsync_UserNotFound_ThrowsValidationException()
        {
            var restaurantId = 1L;

            var request = new OnboardRestaurantOwnerRequest
            {
                Emails = new List<string>
                {
                    "owner@example.com"
                }
            };

            var restaurant = new Restaurant
            {
                Id = restaurantId
            };

            _restaurantRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAsync(
                        restaurantId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(restaurant);

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetUserByEmailAsync(
                        "owner@example.com",
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            _userValidatorMock
                .Setup(validator =>
                    validator.IsUserNullOrDeactivated(
                        null,
                        ValidationMessages.UserNotFound))
                .Throws(
                    new ValidationException(
                        ValidationMessages.UserNotFound));

            var exception =
                await Assert.ThrowsExceptionAsync<ValidationException>(
                    () => _adminService.OnboardRestaurantOwnerAsync(
                        restaurantId,
                        request));

            Assert.AreEqual(
                ValidationMessages.UserNotFound,
                exception.Message);

            _restaurantOwnerRepositoryMock.Verify(
                repository =>
                    repository.Add(
                        It.IsAny<RestaurantOwner>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task OnboardRestaurantOwnerAsync_UserAlreadyAdmin_ThrowsValidationException()
        {
            var restaurantId = 1L;

            var request = new OnboardRestaurantOwnerRequest
            {
                Emails = new List<string>
                {
                    "admin@example.com"
                }
            };

            var restaurant = new Restaurant
            {
                Id = restaurantId
            };

            var user = new User
            {
                Id = 2L,
                Email = "admin@example.com",
                IsActive = true,
                Role = (int)UserRole.Admin
            };

            _restaurantRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAsync(
                        restaurantId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(restaurant);

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetUserByEmailAsync(
                        "admin@example.com",
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _restaurantValidatorMock
                .Setup(validator =>
                    validator.ValidateAdminRole(user.Role))
                .Throws(
                    new ValidationException(
                        ValidationMessages.InvalidRestaurantOwner));

            var exception =
                await Assert.ThrowsExceptionAsync<ValidationException>(
                    () => _adminService.OnboardRestaurantOwnerAsync(
                        restaurantId,
                        request));

            Assert.AreEqual(
                ValidationMessages.InvalidRestaurantOwner,
                exception.Message);

            _restaurantOwnerRepositoryMock.Verify(
                repository =>
                    repository.Add(
                        It.IsAny<RestaurantOwner>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task OnboardRestaurantOwnerAsync_ExistingRelationship_ThrowsValidationException()
        {
            var restaurantId = 1L;

            var request = new OnboardRestaurantOwnerRequest
            {
                Emails = new List<string>
                {
                    "owner@example.com"
                }
            };

            var restaurant = new Restaurant
            {
                Id = restaurantId
            };

            var user = new User
            {
                Id = 2L,
                Email = "owner@example.com",
                IsActive = true,
                Role = (int)UserRole.Owner
            };

            var existingRelationship = new RestaurantOwner
            {
                RestaurantId = restaurantId,
                UserId = user.Id
            };

            _restaurantRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAsync(
                        restaurantId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(restaurant);

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetUserByEmailAsync(
                        "owner@example.com",
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _restaurantOwnerRepositoryMock
                .Setup(repository =>
                    repository.GetOwnerWithRestaurantIdAsync(
                        restaurantId,
                        user.Id,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingRelationship);

            _restaurantValidatorMock
                .Setup(validator =>
                    validator.ValidateOwnerRelationshipDoesNotExist(
                        existingRelationship))
                .Throws(
                    new ValidationException(
                        ValidationMessages.AlreadyOwner));

            var exception =
                await Assert.ThrowsExceptionAsync<ValidationException>(
                    () => _adminService.OnboardRestaurantOwnerAsync(
                        restaurantId,
                        request));

            Assert.AreEqual(
                ValidationMessages.AlreadyOwner,
                exception.Message);

            _restaurantOwnerRepositoryMock.Verify(
                repository =>
                    repository.Add(
                        It.IsAny<RestaurantOwner>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task OnboardRestaurantOwnerAsync_ValidRequest_OnboardsOwnerAndReturnsResult()
        {
            var restaurantId = 1L;

            var request = new OnboardRestaurantOwnerRequest
            {
                Emails = new List<string>
                {
                    "owner@example.com"
                }
            };

            var restaurant = new Restaurant
            {
                Id = restaurantId
            };

            var user = new User
            {
                Id = 2L,
                Name = "Test Owner",
                Email = "owner@example.com",
                IsActive = true,
                Role = (int)UserRole.Owner
            };

            _restaurantRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAsync(
                        restaurantId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(restaurant);

            _authRepositoryMock
                .Setup(repository =>
                    repository.GetUserByEmailAsync(
                        "owner@example.com",
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _restaurantOwnerRepositoryMock
                .Setup(repository =>
                    repository.GetOwnerWithRestaurantIdAsync(
                        restaurantId,
                        user.Id,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync((RestaurantOwner)null);

            RestaurantOwner createdOwner = null;

            _restaurantOwnerRepositoryMock
                .Setup(repository =>
                    repository.Add(
                        It.IsAny<RestaurantOwner>()))
                .Callback<RestaurantOwner>(
                    restaurantOwner =>
                        createdOwner = restaurantOwner)
                .Returns(Task.CompletedTask);

            var result =
                await _adminService.OnboardRestaurantOwnerAsync(
                    restaurantId,
                    request);

            Assert.IsNotNull(result);

            Assert.AreEqual(
                restaurantId,
                result.RestaurantId);

            Assert.AreEqual(
                SuccessMessages.ownersOnboardedSuccessful,
                result.Message);

            Assert.IsNotNull(result.Owners);
            Assert.AreEqual(1, result.Owners.Count);

            Assert.AreEqual(
                (int)UserRole.Owner,
                user.Role);

            Assert.IsNotNull(createdOwner);

            _restaurantValidatorMock.Verify(
                validator =>
                    validator.IsOwnersEmailEmpty(request.Emails),
                Times.Once);

            _restaurantValidatorMock.Verify(
                validator =>
                    validator.ValidateRestaurantExists(restaurant),
                Times.Once);

            _restaurantValidatorMock.Verify(
                validator =>
                    validator.ValidateEmail("owner@example.com"),
                Times.Once);

            _restaurantValidatorMock.Verify(
                validator =>
                    validator.ValidateAdminRole(user.Role),
                Times.Once);

            _restaurantValidatorMock.Verify(
                validator =>
                    validator.ValidateOwnerRelationshipDoesNotExist(null),
                Times.Once);

            _restaurantOwnerRepositoryMock.Verify(
                repository =>
                    repository.Add(
                        It.IsAny<RestaurantOwner>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
