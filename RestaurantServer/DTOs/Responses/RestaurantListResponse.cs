using System.Collections.Generic;

namespace RestaurantServer.DTOs.Responses
{
    public class RestaurantListResponse
    {
        public RestaurantListResponse()
        {
            Restaurants = new List<RestaurantDto>();
        }

        public RestaurantListResponse( List<RestaurantDto> restaurants, PaginatedResponse pagination)
        { 
            Restaurants = restaurants;
            Pagination = pagination;
        }
         
        public List<RestaurantDto> Restaurants { get; set; }
        public PaginatedResponse Pagination { get; set; }
    }
}
