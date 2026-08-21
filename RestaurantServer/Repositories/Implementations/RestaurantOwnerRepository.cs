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

        public async Task<List<RestaurantOwner>> GetOwnersByRestaurantAndUserIdsAsync(
            long restaurantId, IEnumerable<long> userIds, CancellationToken cancellationToken = default)
        {
            var ids = userIds.ToList();

            return await _context.Set<RestaurantOwner>()
                .Where(restaurant =>
                    restaurant.RestaurantId == restaurantId && ids.Contains(restaurant.UserId))
                .ToListAsync(cancellationToken);
        }
    }
}
