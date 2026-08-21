using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestaurantServer.Constants;
using RestaurantServer.DTOs.Requests;
using RestaurantServer.Enums;
using RestaurantServer.Exceptions;
using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.Services.Implementations;
using RestaurantServer.Validators.Interfaces;
using System;
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
                .Setup(x =>
                    x.SaveChangesAsync(
                        It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _restaurantRepositoryMock
                .Setup(x =>
                    x.Add(It.IsAny<Restaurant>()))
                .Returns(Task.CompletedTask);

            _restaurantOwnerRepositoryMock
                .Setup(x =>
                    x.Add(It.IsAny<RestaurantOwner>()))
                .Returns(Task.CompletedTask);

            _adminService = new AdminService(
                _restaurantRepositoryMock.Object,
                _restaurantOwnerRepositoryMock.Object,
                _authRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _restaurantValidatorMock.Object,
                _userValidatorMock.Object);
        }

        [TestMethod]
        public async Task CreateRestaurantAsync_UserNotFound_ThrowsValidationException()
        {
            var createdBy = 1L;

            var request = new CreateRestaurantRequest
            {
                RestaurantName = "Test Restaurant",
                Description = "Test Description",
                MobileNumber = "9876543210",
                AddressLine1 = "123 Main Street",
                AddressLine2 = "Near Mall",
                City = "Dehradun",
                Country = "India",
                PostalCode = "248001",
                OwnerEmail = "owner@example.com"
            };

            _authRepositoryMock
                .Setup(x =>
                    x.GetUserByEmailAsync(
                        request.OwnerEmail,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            _userValidatorMock
                .Setup(x =>
                    x.IsUserNullOrDeactivated(
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

            _authRepositoryMock.Verify(
                x => x.GetUserByEmailAsync(
                    request.OwnerEmail,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _userValidatorMock.Verify(
                x => x.IsUserNullOrDeactivated(
                    null,
                    ValidationMessages.UserNotFound),
                Times.Once);

            _restaurantValidatorMock.Verify(
                x => x.ValidateAdminRole(
                    It.IsAny<int>()),
                Times.Never);

            _restaurantRepositoryMock.Verify(
                x => x.Add(
                    It.IsAny<Restaurant>()),
                Times.Never);

            _restaurantOwnerRepositoryMock.Verify(
                x => x.Add(
                    It.IsAny<RestaurantOwner>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task CreateRestaurantAsync_AdminRoleValidationFails_ThrowsValidationException()
        {
            var createdBy = 1L;

            var request = new CreateRestaurantRequest
            {
                RestaurantName = "Test Restaurant",
                Description = "Test Description",
                MobileNumber = "9876543210",
                AddressLine1 = "123 Main Street",
                AddressLine2 = "Near Mall",
                City = "Dehradun",
                Country = "India",
                PostalCode = "248001",
                OwnerEmail = "admin@example.com"
            };

            var owner = new User
            {
                Id = 2L,
                Name = "Admin User",
                Email = request.OwnerEmail,
                IsActive = true,
                Role = (int)UserRole.Admin
            };

            _authRepositoryMock
                .Setup(x =>
                    x.GetUserByEmailAsync(
                        request.OwnerEmail,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(owner);

            _restaurantValidatorMock
                .Setup(x =>
                    x.ValidateAdminRole(owner.Role))
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

            _userValidatorMock.Verify(
                x => x.IsUserNullOrDeactivated(
                    owner,
                    ValidationMessages.UserNotFound),
                Times.Once);

            _restaurantValidatorMock.Verify(
                x => x.ValidateAdminRole(
                    (int)UserRole.Admin),
                Times.Once);

            _restaurantValidatorMock.Verify(
                x => x.ValidateMobileNumber(
                    It.IsAny<string>()),
                Times.Never);

            _restaurantRepositoryMock.Verify(
                x => x.Add(
                    It.IsAny<Restaurant>()),
                Times.Never);

            _restaurantOwnerRepositoryMock.Verify(
                x => x.Add(
                    It.IsAny<RestaurantOwner>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task CreateRestaurantAsync_MobileNumberExists_ThrowsValidationException()
        {
            var createdBy = 1L;

            var request = new CreateRestaurantRequest
            {
                RestaurantName = "Test Restaurant",
                Description = "Test Description",
                MobileNumber = "9876543210",
                AddressLine1 = "123 Main Street",
                AddressLine2 = "Near Mall",
                City = "Dehradun",
                Country = "India",
                PostalCode = "248001",
                OwnerEmail = "owner@example.com"
            };

            var owner = new User
            {
                Id = 2L,
                Name = "Test Owner",
                Email = request.OwnerEmail,
                IsActive = true,
                Role = (int)UserRole.Customer
            };

            _authRepositoryMock
                .Setup(x =>
                    x.GetUserByEmailAsync(
                        request.OwnerEmail,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(owner);

            _restaurantValidatorMock
                .Setup(x =>
                    x.ValidateMobileNumber(
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

            _restaurantValidatorMock.Verify(
                x => x.ValidateAdminRole(
                    (int)UserRole.Customer),
                Times.Once);

            _restaurantValidatorMock.Verify(
                x => x.ValidateMobileNumber(
                    request.MobileNumber),
                Times.Once);

            _restaurantRepositoryMock.Verify(
                x => x.Add(
                    It.IsAny<Restaurant>()),
                Times.Never);

            _restaurantOwnerRepositoryMock.Verify(
                x => x.Add(
                    It.IsAny<RestaurantOwner>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(
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
                Name = "Test Owner",
                Email = request.OwnerEmail,
                IsActive = true,
                Role = (int)UserRole.Customer
            };

            _authRepositoryMock
                .Setup(x =>
                    x.GetUserByEmailAsync(
                        request.OwnerEmail,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(owner);

            _restaurantValidatorMock
                .Setup(x =>
                    x.ValidateMobileNumber(
                        request.MobileNumber))
                .Returns(Task.CompletedTask);

            Restaurant createdRestaurant = null;
            RestaurantOwner createdRestaurantOwner = null;

            _restaurantRepositoryMock
                .Setup(x =>
                    x.Add(It.IsAny<Restaurant>()))
                .Callback<Restaurant>(restaurant =>
                {
                    createdRestaurant = restaurant;

                    restaurant.Id = 100L;
                })
                .Returns(Task.CompletedTask);

            _restaurantOwnerRepositoryMock
                .Setup(x =>
                    x.Add(It.IsAny<RestaurantOwner>()))
                .Callback<RestaurantOwner>(restaurantOwner =>
                {
                    createdRestaurantOwner = restaurantOwner;
                })
                .Returns(Task.CompletedTask);

            var result =
                await _adminService.CreateRestaurantAsync(
                    request,
                    createdBy);

            Assert.IsNotNull(result);
            Assert.IsNotNull(createdRestaurant);
            Assert.IsNotNull(createdRestaurantOwner);

            Assert.AreEqual(
                100L,
                createdRestaurant.Id);

            Assert.AreEqual(
                request.RestaurantName,
                createdRestaurant.RestaurantName);

            Assert.AreEqual(
                request.Description,
                createdRestaurant.Description);

            Assert.AreEqual(
                request.MobileNumber,
                createdRestaurant.MobileNumber);

            Assert.AreEqual(
                request.AddressLine1,
                createdRestaurant.AddressLine1);

            Assert.AreEqual(
                request.AddressLine2,
                createdRestaurant.AddressLine2);

            Assert.AreEqual(
                request.City,
                createdRestaurant.City);

            Assert.AreEqual(
                request.PostalCode,
                createdRestaurant.PostalCode);

            Assert.AreEqual(
                request.Country,
                createdRestaurant.Country);

            Assert.AreEqual(
                createdBy,
                createdRestaurant.CreatedBy);

            Assert.AreEqual(
                createdBy,
                createdRestaurant.UpdatedBy);

            Assert.IsFalse(
                createdRestaurant.IsDeleted);

            Assert.IsNotNull(
                createdRestaurantOwner.Restaurant);

            Assert.IsNotNull(
                createdRestaurantOwner.User);

            Assert.AreSame(
                createdRestaurant,
                createdRestaurantOwner.Restaurant);

            Assert.AreSame(
                owner,
                createdRestaurantOwner.User);

            Assert.AreEqual(
                (int)UserRole.Owner,
                owner.Role);

            Assert.AreEqual(
                createdRestaurant.Id,
                result.Id);

            Assert.AreEqual(
                request.RestaurantName,
                result.RestaurantName);

            Assert.AreEqual(
                request.Description,
                result.Description);

            Assert.AreEqual(
                request.MobileNumber,
                result.MobileNumber);

            Assert.AreEqual(
                request.AddressLine1,
                result.AddressLine1);

            Assert.AreEqual(
                request.AddressLine2,
                result.AddressLine2);

            Assert.AreEqual(
                request.City,
                result.City);

            Assert.AreEqual(
                request.PostalCode,
                result.PostalCode);

            Assert.AreEqual(
                request.Country,
                result.Country);

            _authRepositoryMock.Verify(
                x => x.GetUserByEmailAsync(
                    request.OwnerEmail,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _userValidatorMock.Verify(
                x => x.IsUserNullOrDeactivated(
                    owner,
                    ValidationMessages.UserNotFound),
                Times.Once);

            _restaurantValidatorMock.Verify(
                x => x.ValidateAdminRole(
                    (int)UserRole.Customer),
                Times.Once);

            _restaurantValidatorMock.Verify(
                x => x.ValidateMobileNumber(
                    request.MobileNumber),
                Times.Once);

            _restaurantRepositoryMock.Verify(
                x => x.Add(
                    It.IsAny<Restaurant>()),
                Times.Once);

            _restaurantOwnerRepositoryMock.Verify(
                x => x.Add(
                    It.IsAny<RestaurantOwner>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [TestMethod]
        public async Task OnboardRestaurantOwnerAsync_EmptyEmails_ThrowsValidationException()
        {
            var restaurantId = 1L;

            var request = new OnboardRestaurantOwnerRequest
            {
                Emails = new List<string>()
            };

            _restaurantValidatorMock
                .Setup(x =>
                    x.IsOwnersEmailEmpty(request.Emails))
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

            _restaurantValidatorMock.Verify(
                x => x.IsOwnersEmailEmpty(
                    request.Emails),
                Times.Once);

            _restaurantRepositoryMock.Verify(
                x => x.GetByIdAsync(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _authRepositoryMock.Verify(
                x => x.GetUsersByEmailsAsync(
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _restaurantOwnerRepositoryMock.Verify(
                x => x.GetOwnersByRestaurantAndUserIdsAsync(
                    It.IsAny<long>(),
                    It.IsAny<IEnumerable<long>>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(
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
                .Setup(x =>
                    x.GetByIdAsync(
                        restaurantId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync((Restaurant)null);

            _restaurantValidatorMock
                .Setup(x =>
                    x.ValidateRestaurantExists(null))
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

            _restaurantValidatorMock.Verify(
                x => x.IsOwnersEmailEmpty(
                    request.Emails),
                Times.Once);

            _restaurantRepositoryMock.Verify(
                x => x.GetByIdAsync(
                    restaurantId,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _restaurantValidatorMock.Verify(
                x => x.ValidateRestaurantExists(null),
                Times.Once);

            _restaurantValidatorMock.Verify(
                x => x.ValidateEmail(
                    It.IsAny<string>()),
                Times.Never);

            _authRepositoryMock.Verify(
                x => x.GetUsersByEmailsAsync(
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _restaurantOwnerRepositoryMock.Verify(
                x => x.Add(
                    It.IsAny<RestaurantOwner>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(
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
                Id = restaurantId,
                RestaurantName = "Test Restaurant"
            };

            _restaurantRepositoryMock
                .Setup(x =>
                    x.GetByIdAsync(
                        restaurantId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(restaurant);

            var exception =
                await Assert.ThrowsExceptionAsync<ValidationException>(
                    () => _adminService.OnboardRestaurantOwnerAsync(
                        restaurantId,
                        request));

            Assert.AreEqual(
                ValidationMessages.DuplicateOwnerEmail,
                exception.Message);

            _restaurantValidatorMock.Verify(
                x => x.IsOwnersEmailEmpty(
                    request.Emails),
                Times.Once);

            _restaurantValidatorMock.Verify(
                x => x.ValidateRestaurantExists(
                    restaurant),
                Times.Once);

            _restaurantValidatorMock.Verify(
                x => x.ValidateEmail(
                    "owner@example.com"),
                Times.Exactly(2));

            _authRepositoryMock.Verify(
                x => x.GetUsersByEmailsAsync(
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _restaurantOwnerRepositoryMock.Verify(
                x => x.GetOwnersByRestaurantAndUserIdsAsync(
                    It.IsAny<long>(),
                    It.IsAny<IEnumerable<long>>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _restaurantOwnerRepositoryMock.Verify(
                x => x.Add(
                    It.IsAny<RestaurantOwner>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(
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
                .Setup(x =>
                    x.GetByIdAsync(
                        restaurantId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(restaurant);

            _authRepositoryMock
                .Setup(x =>
                    x.GetUsersByEmailsAsync(
                        It.IsAny<IEnumerable<string>>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User>());

            var exception =
                await Assert.ThrowsExceptionAsync<ValidationException>(
                    () => _adminService.OnboardRestaurantOwnerAsync(
                        restaurantId,
                        request));

            Assert.AreEqual(
                ValidationMessages.UserNotFound,
                exception.Message);

            _restaurantValidatorMock.Verify(
                x => x.ValidateEmail(
                    "owner@example.com"),
                Times.Once);

            _authRepositoryMock.Verify(
                x => x.GetUsersByEmailsAsync(
                    It.Is<IEnumerable<string>>(emails =>
                        emails.Count() == 1 &&
                        emails.Contains("owner@example.com")),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _userValidatorMock.Verify(
                x => x.IsUserNullOrDeactivated(
                    It.IsAny<User>(),
                    ValidationMessages.UserNotFound),
                Times.Never);

            _restaurantOwnerRepositoryMock.Verify(
                x => x.GetOwnersByRestaurantAndUserIdsAsync(
                    It.IsAny<long>(),
                    It.IsAny<IEnumerable<long>>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _restaurantOwnerRepositoryMock.Verify(
                x => x.Add(
                    It.IsAny<RestaurantOwner>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(
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
                Name = "Admin User",
                Email = "admin@example.com",
                IsActive = true,
                Role = (int)UserRole.Admin
            };

            _restaurantRepositoryMock
                .Setup(x =>
                    x.GetByIdAsync(
                        restaurantId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(restaurant);

            _authRepositoryMock
                .Setup(x =>
                    x.GetUsersByEmailsAsync(
                        It.IsAny<IEnumerable<string>>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User>
                {
                    user
                });

            _restaurantValidatorMock
                .Setup(x =>
                    x.ValidateAdminRole(
                        user.Role))
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

            _userValidatorMock.Verify(
                x => x.IsUserNullOrDeactivated(
                    user,
                    ValidationMessages.UserNotFound),
                Times.Once);

            _restaurantValidatorMock.Verify(
                x => x.ValidateAdminRole(
                    (int)UserRole.Admin),
                Times.Once);

            _restaurantOwnerRepositoryMock.Verify(
                x => x.GetOwnersByRestaurantAndUserIdsAsync(
                    It.IsAny<long>(),
                    It.IsAny<IEnumerable<long>>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _restaurantOwnerRepositoryMock.Verify(
                x => x.Add(
                    It.IsAny<RestaurantOwner>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(
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
                Name = "Test Owner",
                Email = "owner@example.com",
                IsActive = true,
                Role = (int)UserRole.Owner
            };

            var existingOwner = new RestaurantOwner
            {
                Id = 10L,
                RestaurantId = restaurantId,
                UserId = user.Id,
                Restaurant = restaurant,
                User = user
            };

            _restaurantRepositoryMock
                .Setup(x =>
                    x.GetByIdAsync(
                        restaurantId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(restaurant);

            _authRepositoryMock
                .Setup(x =>
                    x.GetUsersByEmailsAsync(
                        It.IsAny<IEnumerable<string>>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User>
                {
                    user
                });

            _restaurantOwnerRepositoryMock
                .Setup(x =>
                    x.GetOwnersByRestaurantAndUserIdsAsync(
                        restaurantId,
                        It.IsAny<IEnumerable<long>>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RestaurantOwner>
                {
                    existingOwner
                });

            var exception =
                await Assert.ThrowsExceptionAsync<ValidationException>(
                    () => _adminService.OnboardRestaurantOwnerAsync(
                        restaurantId,
                        request));

            Assert.AreEqual(
                ValidationMessages.OwnerRelationshipAlreadyExists,
                exception.Message);

            _restaurantOwnerRepositoryMock.Verify(
                x => x.GetOwnersByRestaurantAndUserIdsAsync(
                    restaurantId,
                    It.Is<IEnumerable<long>>(ids =>
                        ids.Contains(user.Id)),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _restaurantValidatorMock.Verify(
                x => x.ValidateOwnerRelationshipDoesNotExist(
                    It.IsAny<RestaurantOwner>()),
                Times.Never);

            _restaurantOwnerRepositoryMock.Verify(
                x => x.Add(
                    It.IsAny<RestaurantOwner>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task OnboardRestaurantOwnerAsync_MultipleValidOwners_CreatesAllOwners()
        {
            var restaurantId = 1L;

            var request = new OnboardRestaurantOwnerRequest
            {
                Emails = new List<string>
                {
                    "owner1@example.com",
                    "owner2@example.com"
                }
            };

            var restaurant = new Restaurant
            {
                Id = restaurantId,
                RestaurantName = "Test Restaurant"
            };

            var user1 = new User
            {
                Id = 2L,
                Name = "Owner One",
                Email = "owner1@example.com",
                IsActive = true,
                Role = (int)UserRole.Customer
            };

            var user2 = new User
            {
                Id = 3L,
                Name = "Owner Two",
                Email = "owner2@example.com",
                IsActive = true,
                Role = (int)UserRole.Customer
            };

            _restaurantRepositoryMock
                .Setup(x =>
                    x.GetByIdAsync(
                        restaurantId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(restaurant);

            _authRepositoryMock
                .Setup(x =>
                    x.GetUsersByEmailsAsync(
                        It.IsAny<IEnumerable<string>>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User>
                {
                    user1,
                    user2
                });

            _restaurantOwnerRepositoryMock
                .Setup(x =>
                    x.GetOwnersByRestaurantAndUserIdsAsync(
                        restaurantId,
                        It.IsAny<IEnumerable<long>>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RestaurantOwner>());

            var createdOwners =
                new List<RestaurantOwner>();

            _restaurantOwnerRepositoryMock
                .Setup(x =>
                    x.Add(
                        It.IsAny<RestaurantOwner>()))
                .Callback<RestaurantOwner>(owner =>
                {
                    createdOwners.Add(owner);
                })
                .Returns(Task.CompletedTask);

            var result =
                await _adminService.OnboardRestaurantOwnerAsync(
                    restaurantId,
                    request);

            Assert.IsNotNull(result);

            Assert.AreEqual(
                2,
                result.Owners.Count);

            Assert.AreEqual(
                2,
                createdOwners.Count);

            Assert.AreEqual(
                (int)UserRole.Owner,
                user1.Role);

            Assert.AreEqual(
                (int)UserRole.Owner,
                user2.Role);

            Assert.AreEqual(
                "owner1@example.com",
                result.Owners[0].Email);

            Assert.AreEqual(
                "owner2@example.com",
                result.Owners[1].Email);

            _restaurantOwnerRepositoryMock.Verify(
                x => x.Add(
                    It.IsAny<RestaurantOwner>()),
                Times.Exactly(2));

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }


        [TestMethod]
        public async Task OnboardRestaurantOwnerAsync_InactiveUser_ThrowsValidationException()
        {
            var restaurantId = 1L;

            var request = new OnboardRestaurantOwnerRequest
            {
                Emails = new List<string>
                {
                    "inactive@example.com"
                }
            };

            var restaurant = new Restaurant
            {
                Id = restaurantId
            };

            var user = new User
            {
                Id = 2L,
                Name = "Inactive User",
                Email = "inactive@example.com",
                IsActive = false,
                Role = (int)UserRole.Customer
            };

            _restaurantRepositoryMock
                .Setup(x =>
                    x.GetByIdAsync(
                        restaurantId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(restaurant);

            _authRepositoryMock
                .Setup(x =>
                    x.GetUsersByEmailsAsync(
                        It.IsAny<IEnumerable<string>>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User>
                {
                    user
                });

            _userValidatorMock
                .Setup(x =>
                    x.IsUserNullOrDeactivated(
                        user,
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

            _userValidatorMock.Verify(
                x => x.IsUserNullOrDeactivated(
                    user,
                    ValidationMessages.UserNotFound),
                Times.Once);

            _restaurantValidatorMock.Verify(
                x => x.ValidateAdminRole(
                    It.IsAny<int>()),
                Times.Never);

            _restaurantOwnerRepositoryMock.Verify(
                x => x.GetOwnersByRestaurantAndUserIdsAsync(
                    It.IsAny<long>(),
                    It.IsAny<IEnumerable<long>>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _restaurantOwnerRepositoryMock.Verify(
                x => x.Add(
                    It.IsAny<RestaurantOwner>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task OnboardRestaurantOwnerAsync_InvalidEmail_ThrowsValidationException()
        {
            var restaurantId = 1L;

            var request = new OnboardRestaurantOwnerRequest
            {
                Emails = new List<string>
                {
                    "invalid-email"
                }
            };

            var restaurant = new Restaurant
            {
                Id = restaurantId
            };

            _restaurantRepositoryMock
                .Setup(x =>
                    x.GetByIdAsync(
                        restaurantId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(restaurant);

            _restaurantValidatorMock
                .Setup(x =>
                    x.ValidateEmail(
                        "invalid-email"))
                .Throws(
                    new ValidationException(
                        ValidationMessages.InvalidEmail));

            var exception =
                await Assert.ThrowsExceptionAsync<ValidationException>(
                    () => _adminService.OnboardRestaurantOwnerAsync(
                        restaurantId,
                        request));

            Assert.AreEqual(
                ValidationMessages.InvalidEmail,
                exception.Message);

            _restaurantValidatorMock.Verify(
                x => x.ValidateEmail(
                    "invalid-email"),
                Times.Once);

            _authRepositoryMock.Verify(
                x => x.GetUsersByEmailsAsync(
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _restaurantOwnerRepositoryMock.Verify(
                x => x.Add(
                    It.IsAny<RestaurantOwner>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task OnboardRestaurantOwnerAsync_EmailWithSpaces_UsesNormalizedEmail()
        {
            var restaurantId = 1L;

            var request = new OnboardRestaurantOwnerRequest
            {
                Emails = new List<string>
                {
                    "  owner@example.com  "
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
                Role = (int)UserRole.Customer
            };

            _restaurantRepositoryMock
                .Setup(x =>
                    x.GetByIdAsync(
                        restaurantId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(restaurant);

            _authRepositoryMock
                .Setup(x =>
                    x.GetUsersByEmailsAsync(
                        It.IsAny<IEnumerable<string>>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User>
                {
                    user
                });

            _restaurantOwnerRepositoryMock
                .Setup(x =>
                    x.GetOwnersByRestaurantAndUserIdsAsync(
                        restaurantId,
                        It.IsAny<IEnumerable<long>>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RestaurantOwner>());

            var result =
                await _adminService.OnboardRestaurantOwnerAsync(
                    restaurantId,
                    request);

            Assert.IsNotNull(result);

            Assert.AreEqual(
                1,
                result.Owners.Count);

            Assert.AreEqual(
                "owner@example.com",
                result.Owners[0].Email);

            _authRepositoryMock.Verify(
                x => x.GetUsersByEmailsAsync(
                    It.Is<IEnumerable<string>>(emails =>
                        emails.Count() == 1 &&
                        emails.First() == "owner@example.com"),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [TestMethod]
        public async Task CreateRestaurantAsync_PassesCancellationTokenToRepositories()
        {
            var createdBy = 1L;
            var cancellationToken =
                new CancellationTokenSource().Token;

            var request = new CreateRestaurantRequest
            {
                RestaurantName = "Test Restaurant",
                Description = "Test Description",
                MobileNumber = "9876543210",
                AddressLine1 = "123 Main Street",
                AddressLine2 = "Near Mall",
                City = "Dehradun",
                Country = "India",
                PostalCode = "248001",
                OwnerEmail = "owner@example.com"
            };

            var owner = new User
            {
                Id = 2L,
                Name = "Test Owner",
                Email = request.OwnerEmail,
                IsActive = true,
                Role = (int)UserRole.Customer
            };

            _authRepositoryMock
                .Setup(x =>
                    x.GetUserByEmailAsync(
                        request.OwnerEmail,
                        cancellationToken))
                .ReturnsAsync(owner);

            _restaurantValidatorMock
                .Setup(x =>
                    x.ValidateMobileNumber(
                        request.MobileNumber))
                .Returns(Task.CompletedTask);

            await _adminService.CreateRestaurantAsync(
                request,
                createdBy,
                cancellationToken);

            _authRepositoryMock.Verify(
                x => x.GetUserByEmailAsync(
                    request.OwnerEmail,
                    cancellationToken),
                Times.Once);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(
                    cancellationToken),
                Times.Once);
        }

        [TestMethod]
        public async Task OnboardRestaurantOwnerAsync_PassesCancellationTokenToRepositories()
        {
            var restaurantId = 1L;

            var cancellationToken =
                new CancellationTokenSource().Token;

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
                Role = (int)UserRole.Customer
            };

            _restaurantRepositoryMock
                .Setup(x =>
                    x.GetByIdAsync(
                        restaurantId,
                        cancellationToken))
                .ReturnsAsync(restaurant);

            _authRepositoryMock
                .Setup(x =>
                    x.GetUsersByEmailsAsync(
                        It.IsAny<IEnumerable<string>>(),
                        cancellationToken))
                .ReturnsAsync(new List<User>
                {
                    user
                });

            _restaurantOwnerRepositoryMock
                .Setup(x =>
                    x.GetOwnersByRestaurantAndUserIdsAsync(
                        restaurantId,
                        It.IsAny<IEnumerable<long>>(),
                        cancellationToken))
                .ReturnsAsync(new List<RestaurantOwner>());

            await _adminService.OnboardRestaurantOwnerAsync(
                restaurantId,
                request,
                cancellationToken);

            _restaurantRepositoryMock.Verify(
                x => x.GetByIdAsync(
                    restaurantId,
                    cancellationToken),
                Times.Once);

            _authRepositoryMock.Verify(
                x => x.GetUsersByEmailsAsync(
                    It.IsAny<IEnumerable<string>>(),
                    cancellationToken),
                Times.Once);

            _restaurantOwnerRepositoryMock.Verify(
                x => x.GetOwnersByRestaurantAndUserIdsAsync(
                    restaurantId,
                    It.IsAny<IEnumerable<long>>(),
                    cancellationToken),
                Times.Once);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(
                    cancellationToken),
                Times.Once);
        }
    }
}
