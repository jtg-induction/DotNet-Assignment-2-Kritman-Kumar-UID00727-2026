using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestaurantServer.DTOs.Requests;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.Services.Implementations;
using RestaurantServer.Services.Interfaces;
using RestaurantServer.Validators.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Tests
{
    [TestClass]
    public class ReportServiceTests
    {
        private Mock<IReportRepository> _reportRepositoryMock;
        private Mock<IReportValidator> _reportValidatorMock;
        private Mock<IUserSessionService> _userSessionServiceMock;
        private Mock<IReportRenderer> _reportRendererMock;

        private ReportService _reportService;

        [TestInitialize]
        public void Setup()
        {
            _reportRepositoryMock = new Mock<IReportRepository>();
            _reportValidatorMock = new Mock<IReportValidator>();
            _userSessionServiceMock = new Mock<IUserSessionService>();
            _reportRendererMock = new Mock<IReportRenderer>();

            _userSessionServiceMock
                .Setup(x => x.GetUserId())
                .Returns(1);

            _reportService = new ReportService(
                _reportRepositoryMock.Object,
                _reportValidatorMock.Object,
                _userSessionServiceMock.Object,
                _reportRendererMock.Object);
        }

        [TestMethod]
        public async Task GetTopOrderedItemsReportAsync_ValidRequest_ShouldReturnReport()
        {
            var request = new TopOrderedItemsRequest();
            var reportData = new List<TopOrderedItemResponse>();
            var expectedReport = new byte[] { 1, 2, 3 };

            _reportRepositoryMock
                .Setup(x => x.GetTopOrderedItemsAsync(
                    1,
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(reportData);

            _reportRendererMock
                .Setup(x => x.RenderTopOrderedItemsReport(reportData))
                .Returns(expectedReport);

            var result = await _reportService.GetTopOrderedItemsReportAsync(request);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedReport, result);

            _reportValidatorMock.Verify(
                x => x.ValidateTopOrderedItemsRequest(request),
                Times.Once);
        }

        [TestMethod]
        public async Task GetFrequentlyBoughtTogetherReportAsync_ValidRequest_ShouldReturnReport()
        {
            var request = new FrequentlyBoughtTogetherRequest();
            var reportData = new List<FrequentlyBoughtTogetherResponse>();
            var expectedReport = new byte[] { 1, 2, 3 };

            _reportRepositoryMock
                .Setup(x => x.GetFrequentlyBoughtTogetherAsync(
                    1,
                    1,
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(reportData);

            _reportRendererMock
                .Setup(x => x.RenderFrequentlyBoughtTogetherReport(reportData))
                .Returns(expectedReport);

            var result = await _reportService
                .GetFrequentlyBoughtTogetherReportAsync(1, request);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedReport, result);

            _reportValidatorMock.Verify(
                x => x.ValidateFrequentlyBoughtTogetherRequest(1, request),
                Times.Once);
        }
    }
}