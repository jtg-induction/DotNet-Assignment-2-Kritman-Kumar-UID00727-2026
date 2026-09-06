using RestaurantServer.Constants;
using System.Collections.Generic;

namespace RestaurantServer.DTOs.Responses
{
    public class FilterOrdersResponse
    {
        public FilterOrdersResponse(PaginatedResponse paginationResponse, List<OrderResponse> orders)
        {
            Pagination = paginationResponse;
            Message = SuccessMessages.OrdersRetrieved;
            Orders = orders;
        }
        public PaginatedResponse Pagination { get; set; }
        public string Message { get; set; }
        public List<OrderResponse> Orders { get; set; }
    }
}
