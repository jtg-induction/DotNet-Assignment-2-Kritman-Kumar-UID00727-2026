using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using RestaurantServer.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order> GetOrderWithItemsByIdNoTrackingAsync(long orderId, bool disableTracking = false, CancellationToken cancellationToken = default);
        Task<Order> GetOrderForUpdateAsync(long orderId, CancellationToken cancellationToken = default);
        Task<(int TotalRecords, List<OrderResponse> Orders)> GetFilteredOrdersNoTrackingAsync(long ownerId, OrderQueryParameters orderQueryParameters,
               CancellationToken cancellationToken = default);
    }
}
