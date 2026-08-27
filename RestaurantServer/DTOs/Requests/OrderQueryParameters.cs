using RestaurantServer.DTOs.Enums;
using RestaurantServer.Enums;

namespace RestaurantServer.DTOs.Requests
{
    public class OrderQueryParameters
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public OrderStatus? Status { get; set; }
        public string SearchQuery { get; set; }
        public long? OrderId { get; set; }
        public OrderSortBy SortBy { get; set; } = OrderSortBy.CreatedAt;
        public bool IsDescending { get; set; } = true;
    }
}
