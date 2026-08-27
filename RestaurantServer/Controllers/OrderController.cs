using RestaurantServer.DTOs.Requests;
using RestaurantServer.Enums;
using RestaurantServer.Filters;
using RestaurantServer.Services.Interfaces;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace RestaurantServer.Controllers
{
    [RoutePrefix("api/orders")]
    public class OrderController : ApiController
    {
        private readonly IOrderService _orderService;

        public OrderController(
            IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [Route("{orderId:long}")]
        [CustomAuthorize(UserRole.Customer, UserRole.Owner, UserRole.Admin)]
        public async Task<IHttpActionResult> GetOrderDetails(
            long orderId,
            CancellationToken cancellationToken = default)
        {
            var response = await _orderService.GetOrderDetailsAsync(
                orderId, cancellationToken);

            return Ok(response);
        }

        [HttpPost]
        [Route("{orderId:long}/cancel")]
        [CustomAuthorize(UserRole.Customer, UserRole.Owner, UserRole.Admin)]
        public async Task<IHttpActionResult> CancelOrder(
            long orderId,
            CancellationToken cancellationToken = default)
        {

            var response = await _orderService.CancelOrderAsync(
                orderId, cancellationToken);

            return Ok(response);
        }

        /// <summary>
        /// Gets the orders for the current restaurant owner using the specified filters and pagination.
        /// </summary>
        /// <param name="orderQueryParameters">The filters, sorting, and pagination parameters.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>The filtered and paginated orders.</returns>
        [HttpGet]
        [Route("")]
        [CustomAuthorize(UserRole.Owner)]
        public async Task<IHttpActionResult> GetOrders([FromUri] OrderQueryParameters orderQueryParameters,
            CancellationToken cancellationToken = default)
        {
            var response = await _orderService.FilterOrdersAsync(orderQueryParameters, cancellationToken);

            return Content(HttpStatusCode.OK, response);
        }
    }
}
