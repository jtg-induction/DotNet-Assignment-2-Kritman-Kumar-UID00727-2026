using RestaurantServer.Enums;
using System.ComponentModel.DataAnnotations;

namespace RestaurantServer.DTOs.Requests
{
    public class UpdateOrderStatusRequest
    {
        [Required]
        public OrderStatus Status { get; set; }
    }
}
