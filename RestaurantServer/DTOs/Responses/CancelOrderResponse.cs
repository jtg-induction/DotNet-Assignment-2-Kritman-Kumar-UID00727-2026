using RestaurantServer.Enums;
using RestaurantServer.Models;

namespace RestaurantServer.DTOs.Responses
{
    public class CancelOrderResponse
    {
        public long OrderId { get; set; }
        public string Status { get; set; } 

        public CancelOrderResponse()
        {
        }

        public CancelOrderResponse(Order order)
        {
            OrderId = order.Id; 
            Status = ((OrderStatus)order.Status).ToString(); 
        }
    }
}
