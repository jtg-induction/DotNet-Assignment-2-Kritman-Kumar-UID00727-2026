using RestaurantServer.Enums;
using RestaurantServer.Models;

namespace RestaurantServer.DTOs.Responses
{
    public class UpdateOrderStatusResponse
    {
        public UpdateOrderStatusResponse(Order order, string message)
        {
            OrderId = order.Id;
            Status = order.Status;
            StatusName = ((OrderStatus)order.Status).ToString();
            Message = message;
        }

        public long OrderId { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string Message { get; set; }
    }
}
