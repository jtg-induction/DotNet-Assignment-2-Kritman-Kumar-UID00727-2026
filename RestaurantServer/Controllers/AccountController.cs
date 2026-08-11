using RestaurantServer.DTOs.Requests;
using RestaurantServer.Services.Interfaces;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using RestaurantServer.Enums;
using RestaurantServer.Filters;

namespace RestaurantServer.Controllers
{
    /// <summary>
    /// Provides endpoints for managing the authenticated user's account.
    /// </summary>
    [RoutePrefix("account")]
    public class AccountController : ApiController
    {
        private readonly IAccountService _accountService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="accountService">
        /// The account service used to perform account-related operations.
        /// </param>
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Updates the account details of the currently authenticated user.
        /// </summary>
        /// <param name="request">
        /// The account details to be updated.
        /// </param>
        /// <returns>
        /// An HTTP 200 response containing the updated account information.
        /// Returns HTTP 401 if the authenticated user's identity cannot be determined.
        /// </returns>
        [HttpPut]
        [Route("update")]
        [CustomAuthorize(UserRole.Customer, UserRole.Owner, UserRole.Admin)]
        public async Task<IHttpActionResult> UpdateAccount(
            UpdateAccountRequest request)
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

            if (!long.TryParse(
                userIdClaim.Value,
                out var userId))
            {
                return Unauthorized();
            }

            var response = await _accountService.UpdateAccountAsync(userId, request);

            return Ok(response);
        }

        /// <summary>
        /// Deactivates the account of the currently authenticated user.
        /// </summary>
        /// <returns>
        /// An HTTP 200 response containing the user's ID and deactivation message.
        /// Returns HTTP 401 if the authenticated user's identity cannot be determined.
        /// </returns>
        [HttpPut]
        [Route("deactivate")]
        public async Task<IHttpActionResult> DeactivateAccount()
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

            if (!long.TryParse(userIdClaim.Value,out var userId))
            {
                return Unauthorized();
            }

            var response =
                await _accountService.DeactivateAccountAsync(userId);

            return Ok(new
            {
                UserId = userId,
                Message = response
            });
        }
    }
}
