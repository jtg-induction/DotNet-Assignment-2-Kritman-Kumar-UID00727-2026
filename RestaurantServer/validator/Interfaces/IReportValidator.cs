using RestaurantServer.DTOs.Requests;

namespace RestaurantServer.Validators.Interfaces
{
    public interface IReportValidator
    {
        void ValidateTopOrderedItemsRequest(TopOrderedItemsRequest request);

        void ValidateFrequentlyBoughtTogetherRequest(
            long restaurantId, FrequentlyBoughtTogetherRequest request);
    }
}
