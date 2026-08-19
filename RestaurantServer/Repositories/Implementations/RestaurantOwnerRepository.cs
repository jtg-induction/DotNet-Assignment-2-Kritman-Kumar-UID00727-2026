using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Implementations
{
    public class RestaurantOwnerRepository : IRestaurantOwnerRepository
    {
        private readonly ApplicationDbContext _context;

        public RestaurantOwnerRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RestaurantOwner> GetAsync(long restaurantId, long userId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<RestaurantOwner>()
                .FirstOrDefaultAsync(restaurantOwner => restaurantOwner.RestaurantId == restaurantId && restaurantOwner.UserId == userId, cancellationToken);
        }

        public Task AddAsync(RestaurantOwner restaurantOwner, CancellationToken cancellationToken = default)
        {
            _context.Set<RestaurantOwner>().Add(restaurantOwner);

            return Task.CompletedTask;
        }
    }
}
