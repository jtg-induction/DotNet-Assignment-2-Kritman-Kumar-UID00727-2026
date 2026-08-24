using System.Collections.Generic;

namespace RestaurantServer.DTOs.Responses
{
    public class RestaurantListResponse
    {
        public RestaurantListResponse()
        {
            Restaurants = new List<RestaurantDto>();
        }

        public RestaurantListResponse(string message, List<RestaurantDto> restaurants, PaginationResponse pagination)
        {
            Message = message;
            Restaurants = restaurants;
            Pagination = pagination;
        }

        public string Message { get; set; }
        public List<RestaurantDto> Restaurants { get; set; }
        public PaginationResponse Pagination { get; set; }
    }
}
