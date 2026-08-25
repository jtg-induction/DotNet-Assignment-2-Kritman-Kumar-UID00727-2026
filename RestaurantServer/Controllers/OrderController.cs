using RestaurantServer.Enums;
using RestaurantServer.Filters;
using RestaurantServer.Services.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace RestaurantServer.Controllers
{
    [RoutePrefix("api/orders")]
    public class OrderController : ApiController
    {
        private readonly IOrderService _orderService;
        private readonly IUserSessionService _currentUserService;

        public OrderController(
            IOrderService orderService,
            IUserSessionService currentUserService)
        {
            _orderService = orderService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        [Route("{orderId:long}")]
        [CustomAuthorize(UserRole.Customer, UserRole.Owner, UserRole.Admin)]
        public async Task<IHttpActionResult> GetOrderDetails(
            long orderId,
            CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService.GetUserId();

            var response = await _orderService.GetOrderDetailsAsync(
                orderId,
                userId.Value,
                cancellationToken);

            return Ok(response);
        }

        [HttpPost]
        [Route("{orderId:long}/cancel")]
        [CustomAuthorize(UserRole.Customer, UserRole.Owner, UserRole.Admin)]
        public async Task<IHttpActionResult> CancelOrder(
            long orderId,
            CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService.GetUserId();

            var response = await _orderService.CancelOrderAsync(
                orderId,
                userId.Value,
                cancellationToken);

            return Ok(response);
        }
    }
}
