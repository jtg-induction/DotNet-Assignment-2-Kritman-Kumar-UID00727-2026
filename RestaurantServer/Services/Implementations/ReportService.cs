using RestaurantServer.DTOs.Requests;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.Services.Interfaces;
using RestaurantServer.Validators.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IReportValidator _reportValidator;
        private readonly IUserSessionService _userSessionService;
        private readonly IReportRenderer _reportRenderer;

        public ReportService(
            IReportRepository reportRepository,
            IReportValidator reportValidator,
            IUserSessionService userSessionService,
            IReportRenderer reportRenderer)
        {
            _reportRepository = reportRepository;
            _reportValidator = reportValidator;
            _userSessionService = userSessionService;
            _reportRenderer = reportRenderer;
        }

        /// <summary>
        /// Generates the top ordered items report for the
        /// authenticated restaurant owner.
        /// </summary>
        /// <param name="request">
        /// The report query parameters.
        /// </param>
        /// <param name="cancellationToken">
        /// A token used to cancel the operation.
        /// </param>
        /// <returns>
        /// The generated PDF report as a byte array.
        /// </returns>
        public async Task<byte[]> GetTopOrderedItemsReportAsync(
            TopOrderedItemsRequest request,
            CancellationToken cancellationToken = default)
        {
            request = request ?? new TopOrderedItemsRequest();

            _reportValidator.ValidateTopOrderedItemsRequest(request);

            var ownerId = _userSessionService.GetUserId().Value;

            var reportData = await _reportRepository
                .GetTopOrderedItemsAsync(ownerId, request, cancellationToken);

            return _reportRenderer.RenderTopOrderedItemsReport(
                reportData);
        }

        /// <summary>
        /// Generates the frequently bought together report
        /// for the authenticated restaurant owner.
        /// </summary>
        /// <param name="restaurantId">
        /// The identifier of the restaurant.
        /// </param>
        /// <param name="request">
        /// The report query parameters.
        /// </param>
        /// <param name="cancellationToken">
        /// A token used to cancel the operation.
        /// </param>
        /// <returns>
        /// The generated PDF report as a byte array.
        /// </returns>
        public async Task<byte[]> GetFrequentlyBoughtTogetherReportAsync(
            long restaurantId,
            FrequentlyBoughtTogetherRequest request,
            CancellationToken cancellationToken = default)
        {
            request = request ?? new FrequentlyBoughtTogetherRequest();

            _reportValidator.ValidateFrequentlyBoughtTogetherRequest(restaurantId, request);

            var ownerId = _userSessionService.GetUserId().Value;

            var reportData = await _reportRepository.GetFrequentlyBoughtTogetherAsync(
                ownerId, restaurantId, request, cancellationToken);

            return _reportRenderer.RenderFrequentlyBoughtTogetherReport(reportData);
        }
    }
}
