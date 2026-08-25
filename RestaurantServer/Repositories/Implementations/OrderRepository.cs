using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Implementations
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<Order> GetOrderWithItemsByIdAsync(long orderId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(order => order.OrderItems)
                .FirstOrDefaultAsync(order => order.Id == orderId, cancellationToken);
        }

        public async Task<Order> GetOrderForUpdateAsync(long orderId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .SqlQuery("SELECT * FROM Orders WITH (UPDLOCK, ROWLOCK) WHERE Id = @p0", orderId)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
