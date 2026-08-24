using System.Collections.Generic;

namespace RestaurantServer.DTOs.Responses
{
    public class RestaurantItemListResponse
    {
        public RestaurantItemListResponse()
        {
            Items = new List<ItemDto>();
        }

        public RestaurantItemListResponse(string message, long restaurantId, List<ItemDto> items, PaginationResponse pagination)
        {
            Message = message;
            RestaurantID = restaurantId;
            Items = items;
            Pagination = pagination;
        }

        public string Message { get; set; }
        public long RestaurantID { get; set; }
        public List<ItemDto> Items { get; set; }
        public PaginationResponse Pagination { get; set; }
    }
}
