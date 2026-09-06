using RestaurantServer.DTOs.Responses;
using System.Collections.Generic;

namespace RestaurantServer.Services.Interfaces
{
    public interface IReportRenderer
    {
        byte[] RenderTopOrderedItemsReport(List<TopOrderedItemResponse> data);

        byte[] RenderFrequentlyBoughtTogetherReport(
            List<FrequentlyBoughtTogetherResponse> data);
    }
}
