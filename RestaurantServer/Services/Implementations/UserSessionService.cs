using RestaurantServer.Services.Interfaces;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace RestaurantServer.Services
{
    public class UserSessionService : IUserSessionService
    {
        public long? GetUserId()
        {
            var claimsPrincipal =
                HttpContext.Current?.User as ClaimsPrincipal;

            if (claimsPrincipal == null)
            {
                return null;
            }

            if (claimsPrincipal.Identity == null ||
                !claimsPrincipal.Identity.IsAuthenticated)
            {
                return null;
            }

            var userIdClaim = claimsPrincipal.Claims
                .FirstOrDefault(
                    claim => claim.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null ||
                string.IsNullOrWhiteSpace(userIdClaim.Value))
            {
                return null;
            }

            if (!long.TryParse(
                    userIdClaim.Value,
                    out var userId))
            {
                return null;
            }

            return userId;
        }
    }
}
