using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Implementations
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly ApplicationDbContext _context;

        public RestaurantRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(Restaurant restaurant, CancellationToken cancellationToken = default)
        {
            _context.Set<Restaurant>().Add(restaurant);

            return Task.CompletedTask;
        }

        public async Task<Restaurant> GetByIdAsync(long restaurantId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Restaurant>().FirstOrDefaultAsync(restaurant => restaurant.Id == restaurantId, cancellationToken);
        }
    }
}
