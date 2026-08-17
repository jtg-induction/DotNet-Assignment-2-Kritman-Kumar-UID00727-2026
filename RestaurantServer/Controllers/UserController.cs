using RestaurantServer.DTOs.Requests;
using RestaurantServer.Enums;
using RestaurantServer.Filters;
using RestaurantServer.Services.Interfaces;
using RestaurantServer.Validators.Interfaces;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace RestaurantServer.Controllers
{
    /// <summary>
    /// Provides endpoints for managing the authenticated user's account.
    /// </summary>
    [RoutePrefix("api/users")]
    public class UserController : ApiController
    {
        private readonly IUserUpdateService _userUpdateService;
        private readonly IUserValidator _userValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userUpdateService">
        /// The service used to update and deactivate user accounts.
        /// </param>
        public UserController(IUserUpdateService userUpdateService, IUserValidator userValidator)
        {
            _userUpdateService = userUpdateService;
            _userValidator = userValidator;
        }

        /// <summary>
        /// Updates the account details of the currently authenticated user.
        /// </summary>
        /// <param name="request">
        /// The account details to be updated.
        /// </param>
        /// <param name="cancellationToken">
        /// The token used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// An HTTP 200 response containing the updated account information.
        /// Returns HTTP 401 if the authenticated user's identity cannot be determined
        /// or the user ID claim is invalid.
        /// </returns>
        [HttpPatch]
        [Route("update")]
        [CustomAuthorize(UserRole.Customer, UserRole.Owner, UserRole.Admin)]
        public async Task<IHttpActionResult> UpdateAccount(UpdateAccountRequest request, CancellationToken cancellationToken = default)
        {
            var claimsPrincipal = User as ClaimsPrincipal;

            var userIdClaim = claimsPrincipal.Claims
                .FirstOrDefault(
                    claim => claim.Type == ClaimTypes.NameIdentifier);

            if (!long.TryParse(
                userIdClaim.Value,
                out var userId))
            {
                return Unauthorized();
            }

            var response = await _userUpdateService.UpdateAccountAsync(userId, request, cancellationToken);

            return Ok(response);
        }

        /// <summary>
        /// Deactivates the account of the currently authenticated user.
        /// </summary>
        /// <param name="cancellationToken">
        /// The token used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// An HTTP 200 response containing the user's ID and deactivation message.
        /// Returns HTTP 401 if the authenticated user's identity cannot be determined,
        /// the user ID claim is missing, or the user ID claim is invalid.
        /// </returns>
        [HttpPatch]
        [Route("deactivate")]
        [CustomAuthorize(UserRole.Customer, UserRole.Owner, UserRole.Admin)]
        public async Task<IHttpActionResult> DeactivateAccount(CancellationToken cancellationToken = default)
        {
            var claimsPrincipal = User as ClaimsPrincipal;

            if (claimsPrincipal == null)
            {
                return Unauthorized();
            }

            var userIdClaim = claimsPrincipal.Claims
                .FirstOrDefault(
                    claim => claim.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            if (!long.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized();
            }

            var response = await _userUpdateService.DeactivateAccountAsync(userId, cancellationToken);

            return Ok(new
            {
                UserId = userId,
                Message = response
            });
        }
    }
}
