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
    /// <summary>
    /// Provides administrative endpoints to manage restaurants and onboard their owners.
    /// </summary>
    [RoutePrefix("api/admin")]
    [CustomAuthorize(UserRole.Admin)]
    public class AdminRestaurantController : ApiController
    {
        private readonly AdminService _restaurantService;
        private readonly IRequestValidator _requestValidator;
        private readonly IUserSessionService _currentUserService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminRestaurantController"/> class.
        /// </summary>
        /// <param name="restaurantService">The service handling restaurant administration business logic.</param>
        /// <param name="requestValidator">The validator checking incoming request payloads.</param>
        /// <param name="currentUserService">The service retrieving current user session data.</param>
        public AdminRestaurantController(
            AdminService restaurantService,
            IRequestValidator requestValidator,
            IUserSessionService currentUserService)
        {
            _restaurantService = restaurantService;
            _requestValidator = requestValidator;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Creates a new restaurant profile.
        /// </summary>
        /// <param name="request">The data payload containing details of the restaurant to create.</param>
        /// <param name="cancellationToken">The token used to monitor for request cancellation requests.</param>
        /// <returns>An <see cref="IHttpActionResult"/> yielding a 201 Created status with the new restaurant details, or 401 Unauthorized if the admin session is invalid.</returns>
        [HttpPost]
        [Route("restaurants")]
        public async Task<IHttpActionResult> CreateRestaurantAsync(CreateRestaurantRequest request, CancellationToken cancellationToken = default)
        {
            _requestValidator.IsRequestNull(request);

            var createdBy = _currentUserService.GetUserId();

            var response = await _restaurantService.CreateRestaurantAsync(request, createdBy.Value, cancellationToken);

            return Content(HttpStatusCode.Created, response);
        }

        /// <summary>
        /// Onboards an owner and assigns them to a specific restaurant.
        /// </summary>
        /// <param name="restaurantId">The unique long identifier of the target restaurant.</param>
        /// <param name="request">The data payload containing the owner onboarding information.</param>
        /// <param name="cancellationToken">The token used to monitor for request cancellation requests.</param>
        /// <returns>An <see cref="IHttpActionResult"/> yielding a 201 Created status with the onboarded owner details.</returns>
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
