using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Services.Interfaces
{
    public interface IAdminService
    {
        Task<CreateRestaurantResponse> CreateRestaurantAsync(
            CreateRestaurantRequest request,
            CancellationToken cancellationToken = default);

        Task<OnboardRestaurantResponses> OnboardRestaurantOwnerAsync(long restaurantId, OnboardRestaurantOwnerRequest request,
            CancellationToken cancellationToken = default);
    }
}
