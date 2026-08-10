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
    [RoutePrefix("account")]
    public class AccountController : ApiController
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

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

            var response =
                await _accountService.UpdateAccountAsync(
                    userId,
                    request);

            return Ok(response);
        }

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

            if (!long.TryParse(
                userIdClaim.Value,
                out var userId))
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


        [HttpGet]
        [Route("admin-test")]
        [CustomAuthorize(UserRole.Admin)]
        public IHttpActionResult AdminTest()
        {
            return Ok(new
            {
                Message = "Admin authorization successful."
            });
        }

    }
}
