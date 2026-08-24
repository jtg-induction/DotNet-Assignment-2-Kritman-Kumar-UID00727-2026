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
    }
}
