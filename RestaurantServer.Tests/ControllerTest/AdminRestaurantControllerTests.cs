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
    /// Provides administrative endpoints to manage restaurants and onboard their owners.
    /// </summary>
    [RoutePrefix("api/admin")]
    [CustomAuthorize(UserRole.Admin)]
    public class AdminRestaurantController : ApiController
    {
        private readonly IAdminService _restaurantService;
        private readonly IRequestValidator _requestValidator;

        public AdminRestaurantController(
            IAdminService restaurantService,
            IRequestValidator requestValidator)
        {
            _restaurantService = restaurantService;
            _requestValidator = requestValidator;
        }

        [HttpPost]
        [Route("restaurants")]
        public async Task<IHttpActionResult> CreateRestaurantAsync(
            CreateRestaurantRequest request,
            CancellationToken cancellationToken = default)
        {
            _requestValidator.IsRequestNull(request);

            var response =
                await _restaurantService.CreateRestaurantAsync(
                    request,
                    cancellationToken);

            return Content(
                HttpStatusCode.Created,
                response);
        }

        [HttpPost]
        [Route("restaurants/{restaurantId:long}/owners")]
        public async Task<IHttpActionResult> OnboardRestaurantOwnerAsync(
            long restaurantId,
            OnboardRestaurantOwnerRequest request,
            CancellationToken cancellationToken = default)
        {
            _requestValidator.IsRequestNull(request);

            var response =
                await _restaurantService.OnboardRestaurantOwnerAsync(
                    restaurantId,
                    request,
                    cancellationToken);

            return Content(
                HttpStatusCode.Created,
                response);
        }
    }
}
