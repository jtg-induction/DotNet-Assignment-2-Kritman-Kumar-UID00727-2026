using RestaurantServer.Constants;
using System.ComponentModel.DataAnnotations;

namespace RestaurantServer.DTOs.Requests
{
    public class OrderItemRequest
    {
        [Required(ErrorMessage = ValidationMessages.ItemIdRequired)]
        [Range(1, long.MaxValue)]
        public long ItemId { get; set; }

        [Required(ErrorMessage = ValidationMessages.QuantityRequired)]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
