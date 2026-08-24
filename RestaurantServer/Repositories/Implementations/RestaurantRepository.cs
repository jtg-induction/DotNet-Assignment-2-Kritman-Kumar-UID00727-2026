using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Implementations
{
    public class RestaurantRepository : Repository<Restaurant>, IRestaurantRepository
    {
        public RestaurantRepository(ApplicationDbContext context)
            : base(context)
        {

        }

        public async Task<bool> ExistsByMobileNumberAsync(string mobileNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Restaurants
                .AnyAsync(
                    restaurant => restaurant.MobileNumber == mobileNumber && !restaurant.IsDeleted,
                    cancellationToken);
        }

        public async Task<List<Restaurant>> GetAvailableRestaurantsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.Restaurants
                .Where(restaurant => !restaurant.IsDeleted)
                .OrderBy(restaurant => restaurant.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountAvailableRestaurantsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Restaurants
                .CountAsync(restaurant => !restaurant.IsDeleted, cancellationToken);
        }
    }
}
