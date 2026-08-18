using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Services.Interfaces
{
    public interface IRestaurantService
    {
        Task<CreateRestaurantResponse> CreateRestaurantAsync(
            CreateRestaurantRequest request,
            long createdBy,
            CancellationToken cancellationToken = default);

        Task<OnboardRestaurantOwnerResponse> OnboardRestaurantOwnerAsync(
            long restaurantId,
            OnboardRestaurantOwnerRequest request,
            CancellationToken cancellationToken = default);
    }
}
