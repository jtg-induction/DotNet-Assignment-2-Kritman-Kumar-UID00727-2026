using RestaurantServer.Constants;
using RestaurantServer.Enums;
using RestaurantServer.Models;

namespace RestaurantServer.DTOs.Responses
{
    public class CancelOrderResponse
    {
        public long OrderId { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string Message { get; set; }

        public CancelOrderResponse()
        {
        }

        public CancelOrderResponse(Order order, string message = SuccessMessages.OrderCancelledSuccessfully)
        {
            OrderId = order.Id;
            Status = order.Status;
            StatusName = ((OrderStatus)order.Status).ToString();
            Message = message;
        }
    }
}
