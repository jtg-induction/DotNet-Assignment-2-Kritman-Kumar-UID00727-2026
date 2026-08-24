using RestaurantServer.Constants;
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
    [RoutePrefix("api/restaurants")]
    public class RestaurantController : ApiController
    {
        private readonly IRestaurantService _restaurantService;
        private readonly IOrderService _orderService;
        private readonly IUserSessionService _currentUserService;
        private readonly IRequestValidator _requestValidator;

        public RestaurantController(
            IRestaurantService restaurantService,
            IOrderService orderService,
            IUserSessionService currentUserService,
            IRequestValidator requestValidator)
        {
            _restaurantService = restaurantService;
            _orderService = orderService;
            _currentUserService = currentUserService;
            _requestValidator = requestValidator;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAllRestaurants(
            int page = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var response = await _restaurantService.GetRestaurantsAsync(page, pageSize, cancellationToken);

            return Ok(response);
        }

        [HttpGet]
        [Route("{restaurantId:long}/items")]
        public async Task<IHttpActionResult> GetRestaurantItems(
            long restaurantId,
            int page = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var response = await _restaurantService.GetRestaurantItemsAsync(restaurantId, page, pageSize, cancellationToken);

            return Ok(response);
        }

        [HttpPost]
        [Route("{restaurantId:long}/orders")]
        [CustomAuthorize(UserRole.Customer, UserRole.Owner, UserRole.Admin)]
        public async Task<IHttpActionResult> PlaceOrder(
            long restaurantId,
            CreateOrderRequest request,
            CancellationToken cancellationToken = default)
        {
            _requestValidator.IsRequestNull(request);

            var userId = _currentUserService.GetUserId();

            var response = await _orderService.PlaceOrderAsync(
                userId.Value, restaurantId, request,
                cancellationToken);

            return Content(HttpStatusCode.Created, response);
        }
    }
}
