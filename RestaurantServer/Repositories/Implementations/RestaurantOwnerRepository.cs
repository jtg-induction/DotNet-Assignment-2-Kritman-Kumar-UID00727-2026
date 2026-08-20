using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Implementations
{
    public class RestaurantOwnerRepository : Repository<RestaurantOwner>,IRestaurantOwnerRepository
    {
        public RestaurantOwnerRepository(ApplicationDbContext context)
            : base(context)
        {

        }

        public async Task<RestaurantOwner> GetOwnerWithRestaurantIdAsync(long restaurantId, long userId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<RestaurantOwner>()
                .FirstOrDefaultAsync(restaurantOwner => 
                restaurantOwner.RestaurantId == restaurantId && restaurantOwner.UserId == userId, cancellationToken);
        }
    }
}
