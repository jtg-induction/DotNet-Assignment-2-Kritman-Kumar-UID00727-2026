using System.Collections.Generic;

namespace RestaurantServer.DTOs.Responses
{
    public class RestaurantItemListResponse
    {
        public RestaurantItemListResponse()
        {
            Items = new List<ItemDto>();
        }

        public RestaurantItemListResponse(string message, long restaurantId, List<ItemDto> items, PaginatedResponse pagination)
        {
            Message = message;
            RestaurantID = restaurantId;
            Items = items;
            Pagination = pagination;
        }

        public string Message { get; set; }
        public long RestaurantID { get; set; }
        public List<ItemDto> Items { get; set; }
        public PaginatedResponse Pagination { get; set; }
    }
}
