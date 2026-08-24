using RestaurantServer.Services.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace RestaurantServer.Controllers
{
    [RoutePrefix("api/restaurants")]
    public class RestaurantController : ApiController
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
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
    }
}
