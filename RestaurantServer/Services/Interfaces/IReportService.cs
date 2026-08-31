using RestaurantServer.DTOs.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Services.Interfaces
{
    public interface IReportService
    {
        Task<byte[]> GetTopOrderedItemsReportAsync(
            TopOrderedItemsRequest request,
            CancellationToken cancellationToken = default);

        Task<byte[]> GetFrequentlyBoughtTogetherReportAsync(
            long restaurantId,
            FrequentlyBoughtTogetherRequest request,
            CancellationToken cancellationToken = default);
    }
}
