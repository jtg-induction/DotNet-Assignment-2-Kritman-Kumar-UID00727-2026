using RestaurantServer.Models;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order> GetOrderWithItemsByIdAsync(long orderId, CancellationToken cancellationToken = default);
    }
}
