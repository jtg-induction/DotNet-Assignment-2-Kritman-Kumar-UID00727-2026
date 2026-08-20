using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using System.Data.Entity;
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
    }
}
