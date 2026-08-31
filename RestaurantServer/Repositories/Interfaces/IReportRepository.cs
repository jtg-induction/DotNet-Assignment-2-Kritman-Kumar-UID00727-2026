using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Interfaces
{
    public interface IReportRepository
    {
        Task<List<TopOrderedItemResponse>> GetTopOrderedItemsAsync(
            long ownerId, TopOrderedItemsRequest request,
            CancellationToken cancellationToken = default);

        Task<List<FrequentlyBoughtTogetherResponse>>
            GetFrequentlyBoughtTogetherAsync(
                long ownerId, long restaurantId,
                FrequentlyBoughtTogetherRequest request,
                CancellationToken cancellationToken = default);
    }
}
