using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Implementations
{
    public class RestaurantOwnerRepository
        : Repository<RestaurantOwner>, IRestaurantOwnerRepository
    {
        public RestaurantOwnerRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Provides the Restaurant Owners by RestaurantId and UserId
        /// </summary>
        /// <param name="restaurantId">restaurantId of current restaurant</param>
        /// <param name="userIds">onboarding restaurant owners userId</param>
        /// <returns>returns List of Restaurant Owner</returns>
        public async Task<List<RestaurantOwner>> GetOwnersByRestaurantAndUserIdsAsync(
            long restaurantId, List<long> userIds, bool disableTracking = false, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<RestaurantOwner>()
                .Where(owner => owner.RestaurantId == restaurantId &&
                userIds.Contains(owner.UserId));

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Determines whether the specified user is an owner of the specified restaurant.
        /// </summary>
        /// <param name="restaurantId">The unique identifier of the restaurant.</param>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>
        /// <c>true</c> if the user is an owner of the restaurant; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> IsOwnerAsync(
            long restaurantId, long userId, CancellationToken cancellationToken = default)
        {
            return await _context.RestaurantOwners
                .AnyAsync(ro => ro.RestaurantId == restaurantId && ro.UserId == userId, cancellationToken);
        }
    }
}
