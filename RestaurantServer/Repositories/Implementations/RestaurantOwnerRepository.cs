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
            long restaurantId, IEnumerable<long> userIds, CancellationToken cancellationToken = default)
        {
            var ids = userIds.ToList();

            return await _context.Set<RestaurantOwner>()
                .Where(restaurant =>
                    restaurant.RestaurantId == restaurantId && ids.Contains(restaurant.UserId))
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsOwnerAsync(
            long restaurantId, long userId, CancellationToken cancellationToken = default)
        {
            return await _context.RestaurantOwners
                .AnyAsync(ro => ro.RestaurantId == restaurantId && ro.UserId == userId, cancellationToken);
        }
    }
}
