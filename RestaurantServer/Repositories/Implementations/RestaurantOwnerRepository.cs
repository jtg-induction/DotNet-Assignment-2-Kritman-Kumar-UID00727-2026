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
    }
}
