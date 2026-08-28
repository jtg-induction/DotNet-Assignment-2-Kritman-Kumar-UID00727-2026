using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Services.Interfaces
{
    public interface IOrderService
    {
        Task<CreateOrderResponse> PlaceOrderAsync(
            long restaurantId, CreateOrderRequest request,
            CancellationToken cancellationToken = default);

        Task<OrderDetailsResponse> GetOrderDetailsAsync(long orderId, 
            CancellationToken cancellationToken = default);

        Task<CancelOrderResponse> CancelOrderAsync(long orderId, 
            CancellationToken cancellationToken = default);
        Task<FilterOrdersResponse> FilterOrdersAsync(OrderQueryParameters orderQueryParameters, 
            CancellationToken cancellationToken = default);
        Task<UpdateOrderStatusResponse> UpdateOrderStatusAsync(long orderId,
            UpdateOrderStatusRequest request, CancellationToken cancellationToken = default);
    }
}
