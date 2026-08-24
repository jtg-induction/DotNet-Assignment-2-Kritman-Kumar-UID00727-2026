using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Services.Interfaces
{
    public interface IOrderService
    {
        Task<CreateOrderResponse> PlaceOrderAsync(
            long userId,
            long restaurantId,
            CreateOrderRequest request,
            CancellationToken cancellationToken = default);
    }
}
