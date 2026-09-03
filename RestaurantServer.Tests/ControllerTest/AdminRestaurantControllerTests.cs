using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestaurantServer.Constants;
using RestaurantServer.Controllers;
using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using RestaurantServer.Exceptions;
using RestaurantServer.Services.Interfaces;
using RestaurantServer.Validators.Interfaces;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace RestaurantServer.Tests
{
    [TestClass]
    public class AdminRestaurantControllerTests
    {
        private Mock<IAdminService> _adminServiceMock;
        private Mock<IRequestValidator> _requestValidatorMock;
        private Mock<IUserSessionService> _currentUserServiceMock;

        private AdminRestaurantController _controller;

        [TestInitialize]
        public void Setup()
        {
            _adminServiceMock = new Mock<IAdminService>();
            _requestValidatorMock = new Mock<IRequestValidator>();
            _currentUserServiceMock = new Mock<IUserSessionService>();

            _controller = new AdminRestaurantController(
                _adminServiceMock.Object,
                _requestValidatorMock.Object,
                _currentUserServiceMock.Object);
        }

        [TestMethod]
        public async Task CreateRestaurantAsync_ValidRequest_ReturnsCreated()
        {
            var request = new CreateRestaurantRequest();

            var createdBy = 1L;

            var response = new CreateRestaurantResponse();

            _currentUserServiceMock
                .Setup(service =>
                    service.GetUserId())
                .Returns(createdBy);

            _adminServiceMock
                .Setup(service =>
                    service.CreateRestaurantAsync(
                        request,
                        createdBy,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _controller.CreateRestaurantAsync(
                request,
                CancellationToken.None);

            Assert.IsNotNull(result);

            var contentResult = result as NegotiatedContentResult<CreateRestaurantResponse>;

            Assert.IsNotNull(contentResult);

            Assert.AreEqual(
                HttpStatusCode.Created,
                contentResult.StatusCode);

            Assert.AreSame(
                response,
                contentResult.Content);

            _requestValidatorMock.Verify(
                validator =>
                    validator.IsRequestNull(request),
                Times.Once);

            _currentUserServiceMock.Verify(
                service =>
                    service.GetUserId(),
                Times.Once);

            _adminServiceMock.Verify(
                service =>
                    service.CreateRestaurantAsync(
                        request,
                        createdBy,
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [TestMethod]
        public async Task CreateRestaurantAsync_RequestIsNull_ThrowsValidationException()
        {
            CreateRestaurantRequest request = null;

            _requestValidatorMock
                .Setup(validator =>
                    validator.IsRequestNull(request))
                .Throws(
                    new ValidationException(
                        ValidationMessages.EmptyRequest));

            var exception =
                await Assert.ThrowsExceptionAsync<ValidationException>(
                    () => _controller.CreateRestaurantAsync(
                        request,
                        CancellationToken.None));

            Assert.AreEqual(
                ValidationMessages.EmptyRequest,
                exception.Message);

            _requestValidatorMock.Verify(
                validator =>
                    validator.IsRequestNull(request),
                Times.Once);

            _currentUserServiceMock.Verify(
                service =>
                    service.GetUserId(),
                Times.Never);

            _adminServiceMock.Verify(
                service =>
                    service.CreateRestaurantAsync(
                        It.IsAny<CreateRestaurantRequest>(),
                        It.IsAny<long>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task OnboardRestaurantOwnerAsync_ValidRequest_ReturnsCreated()
        {
            var restaurantId = 10L;

            var request = new OnboardRestaurantOwnerRequest();

            var response = new OnboardRestaurantResponses();

            _adminServiceMock
                .Setup(service =>
                    service.OnboardRestaurantOwnerAsync(
                        restaurantId,
                        request,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result =
                await _controller.OnboardRestaurantOwnerAsync(
                    restaurantId,
                    request,
                    CancellationToken.None);

            Assert.IsNotNull(result);

            var contentResult =
                result as NegotiatedContentResult<OnboardRestaurantResponses>;

            Assert.IsNotNull(contentResult);

            Assert.AreEqual(
                HttpStatusCode.Created,
                contentResult.StatusCode);

            Assert.AreSame(
                response,
                contentResult.Content);

            _requestValidatorMock.Verify(
                validator =>
                    validator.IsRequestNull(request),
                Times.Once);

            _adminServiceMock.Verify(
                service =>
                    service.OnboardRestaurantOwnerAsync(
                        restaurantId,
                        request,
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [TestMethod]
        public async Task OnboardRestaurantOwnerAsync_RequestIsNull_ThrowsValidationException()
        {
            var restaurantId = 10L;

            OnboardRestaurantOwnerRequest request = null;

            _requestValidatorMock
                .Setup(validator =>
                    validator.IsRequestNull(request))
                .Throws(
                    new ValidationException(
                        ValidationMessages.EmptyRequest));

            var exception =
                await Assert.ThrowsExceptionAsync<ValidationException>(
                    () => _controller.OnboardRestaurantOwnerAsync(
                        restaurantId,
                        request,
                        CancellationToken.None));

            Assert.AreEqual(
                ValidationMessages.EmptyRequest,
                exception.Message);

            _requestValidatorMock.Verify(
                validator =>
                    validator.IsRequestNull(request),
                Times.Once);

            _adminServiceMock.Verify(
                service =>
                    service.OnboardRestaurantOwnerAsync(
                        It.IsAny<long>(),
                        It.IsAny<OnboardRestaurantOwnerRequest>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
