using RestaurantServer.Services.Interfaces;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace RestaurantServer.Services
{
    /// <summary>
    /// Provides methods to access data from the current user session context.
    /// </summary>
    public class UserSessionService : IUserSessionService
    {
        /// <summary>
        /// Retrieves the unique identifier of the currently authenticated user from the HTTP context claims.
        /// </summary>
        /// <returns>
        /// The unique user ID as a <see cref="long"/> if the user is authenticated and the claim exists; 
        /// otherwise, <c>null</c>.
        /// </returns>
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
