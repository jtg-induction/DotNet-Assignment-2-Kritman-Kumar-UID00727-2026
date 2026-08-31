using RestaurantServer.DTOs.Requests;
using RestaurantServer.Enums;
using RestaurantServer.Filters;
using RestaurantServer.Services.Interfaces;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace RestaurantServer.Controllers
{
    [RoutePrefix("api/reports")]
    public class ReportController : ApiController
    {
        private readonly IReportService _reportService;

        public ReportController(
            IReportService reportService)
        {
            _reportService = reportService;
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
        /// A downloadable PDF report.
        /// </returns>
        [HttpGet]
        [Route("top-ordered-items")]
        [CustomAuthorize(UserRole.Owner)]
        public async Task<HttpResponseMessage> GetTopOrderedItems(
            [FromUri] TopOrderedItemsRequest request,
            CancellationToken cancellationToken = default)
        {
            var report = await _reportService
                .GetTopOrderedItemsReportAsync(
                    request, cancellationToken);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(report)
            };

            response.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/pdf");

            response.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "TopOrderedItems.pdf"
                };

            return response;
        }

        /// <summary>
        /// Generates the frequently bought together report
        /// for the specified restaurant.
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
        /// A downloadable PDF report.
        /// </returns>
        [HttpGet]
        [Route("restaurants/{restaurantId}/frequently-bought-together")]
        [CustomAuthorize(UserRole.Owner)]
        public async Task<HttpResponseMessage>
            GetFrequentlyBoughtTogether(long restaurantId,
                [FromUri] FrequentlyBoughtTogetherRequest request,
                CancellationToken cancellationToken = default)
        {
            var report = await _reportService
                .GetFrequentlyBoughtTogetherReportAsync(restaurantId,
                    request, cancellationToken);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(report)
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

            response.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "FrequentlyBoughtTogether.pdf"
                };

            return response;
        }
    }
}
