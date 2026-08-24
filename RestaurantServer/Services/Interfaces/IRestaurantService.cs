using RestaurantServer.DTOs.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Services.Interfaces
{
    public interface IRestaurantService
    {
        Task<RestaurantListResponse> GetRestaurantsAsync(
            int page, int pageSize,
            CancellationToken cancellationToken = default);

        Task<RestaurantItemListResponse> GetRestaurantItemsAsync(
            long restaurantId,
            int page, int pageSize,
            CancellationToken cancellationToken = default);
    }
}
