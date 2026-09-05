using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using System.Data.Entity;
using System.Linq;
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

        /// <summary>
        /// Asynchronously retrieves an order by its identifier, including its associated order items.
        /// </summary>
        /// <param name="orderId">
        /// The unique identifier of the order to retrieve.
        /// </param>
        /// <param name="disableTracking">
        /// A value indicating whether Entity Framework tracking should be disabled.
        /// When set to <see langword="true"/>, <see cref="DbExtensions.AsNoTracking{T}(System.Linq.IQueryable{T})" />
        /// is applied to the query. This is recommended when the returned order and its items
        /// are intended for read-only purposes and do not need to be tracked for updates.
        /// </param>
        /// <param name="cancellationToken">
        /// A token to observe while waiting for the asynchronous operation to complete.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the
        /// matching order with its associated order items, or <see langword="null"/> if no
        /// order with the specified identifier exists.
        /// </returns>
        public async Task<Order> GetOrderWithItemsByIdAsync(long orderId,
            bool disableTracking = false, CancellationToken cancellationToken = default)
        {
            IQueryable<Order> query = _context.Orders
                .Include(order => order.OrderItems);

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(
                order => order.Id == orderId,
                cancellationToken);
        }

        public async Task<Order> GetOrderForUpdateAsync(long orderId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .SqlQuery("SELECT * FROM Orders WITH (UPDLOCK, ROWLOCK) WHERE Id = @p0", orderId)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
