using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;

namespace RestaurantServer.Repositories.Implementations
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}
