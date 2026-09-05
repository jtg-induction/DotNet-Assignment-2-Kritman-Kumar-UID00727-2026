using RestaurantServer.DTOs.Requests;
using RestaurantServer.Enums;
using RestaurantServer.Filters;
using RestaurantServer.Services.Interfaces;
using RestaurantServer.Validators.Interfaces;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace RestaurantServer.Controllers
{
    /// <summary>
    /// Provides API endpoints for managing customer orders.
    /// </summary>
    [RoutePrefix("api/orders")]
    public class OrderController : ApiController
    {
        private readonly IOrderService _orderService;
        private readonly IRequestValidator _requestValidator;

        public OrderController(
            IOrderService orderService,
            IRequestValidator requestValidator)
        {
            _orderService = orderService;
            _requestValidator = requestValidator;
        }

        /// <summary>
        /// Retrieves the details of a specific order.
        /// </summary>
        /// <param name="orderId">The unique identifier of the order.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>
        /// An HTTP 200 response containing the details of the specified order.
        /// </returns>
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

        /// <summary>
        /// Cancels a specific order.
        /// </summary>
        /// <param name="orderId">The unique identifier of the order to cancel.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>
        /// An HTTP 200 response containing the result of the cancellation operation.
        /// </returns>
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
        /// Places a new order for a restaurant.
        /// </summary>
        /// <param name="request">The request containing the restaurant and order details.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>
        /// An HTTP 201 response containing the newly created order.
        /// </returns>
        [HttpPost]
        [Route("")]
        [CustomAuthorize(UserRole.Customer, UserRole.Owner, UserRole.Admin)]
        public async Task<IHttpActionResult> PlaceOrder(CreateOrderRequest request,
            CancellationToken cancellationToken = default)
        {
            _requestValidator.IsRequestNull(request);

            var response = await _orderService.PlaceOrderAsync(request.RestaurantId, request, cancellationToken);

            return Content(HttpStatusCode.Created, response);
        }
    }
}
