using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;

namespace RestaurantServer.Repositories.Implementations
{
    public class RestaurantRepository : Repository<Restaurant>, IRestaurantRepository
    {
        public RestaurantRepository(ApplicationDbContext context)
            : base(context)
        {

        }
    }
}
