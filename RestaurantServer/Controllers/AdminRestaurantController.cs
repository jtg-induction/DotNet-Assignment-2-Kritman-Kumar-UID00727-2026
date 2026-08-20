using RestaurantServer.DTOs.Requests;
using RestaurantServer.Enums;
using RestaurantServer.Filters;
using RestaurantServer.Services.Implementations;
using RestaurantServer.Services.Interfaces;
using RestaurantServer.Validators.Interfaces;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace RestaurantServer.Controllers
{
    [RoutePrefix("api/admin")]
    [CustomAuthorize(UserRole.Admin)]
    public class AdminRestaurantController : ApiController
    {
        private readonly AdminService _restaurantService;
        private readonly IRequestValidator _requestValidator;
        private readonly IUserSessionService _currentUserService;

        public AdminRestaurantController(
            AdminService restaurantService,
            IRequestValidator requestValidator,
            IUserSessionService currentUserService)
        {
            _restaurantService = restaurantService;
            _requestValidator = requestValidator;
            _currentUserService = currentUserService;
        }

        [HttpPost]
        [Route("restaurants")]
        public async Task<IHttpActionResult> CreateRestaurantAsync(CreateRestaurantRequest request, CancellationToken cancellationToken = default)
        {
            _requestValidator.IsRequestNull(request);

            var createdBy = _currentUserService.GetUserId();

            if (!createdBy.HasValue)
            {
                return Unauthorized();
            }

            var response = await _restaurantService.CreateRestaurantAsync(request, createdBy.Value, cancellationToken);

            return Content(HttpStatusCode.Created, response);
        }

        [HttpPost]
        [Route("restaurants/{restaurantId:long}/owners")]
        public async Task<IHttpActionResult> OnboardRestaurantOwnerAsync(
            long restaurantId,
            OnboardRestaurantOwnerRequest request,
            CancellationToken cancellationToken = default)
        {
            _requestValidator.IsRequestNull(request);

            var response = await _restaurantService.OnboardRestaurantOwnerAsync(restaurantId, request, cancellationToken);

            return Content(HttpStatusCode.Created, response);
        }
    }
}
