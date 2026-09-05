using RestaurantServer.Models;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order> GetOrderWithItemsByIdAsync(long orderId, bool disableTracking = false, CancellationToken cancellationToken = default);
        Task<Order> GetOrderForUpdateAsync(long orderId, CancellationToken cancellationToken = default);
    }
}
