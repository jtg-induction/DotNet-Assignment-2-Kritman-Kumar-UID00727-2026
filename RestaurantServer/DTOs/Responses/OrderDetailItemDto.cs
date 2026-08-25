using RestaurantServer.Models;

namespace RestaurantServer.DTOs.Responses
{
    public class OrderDetailItemDto
    {
        public OrderDetailItemDto()
        {
        }

        public OrderDetailItemDto(OrderItem orderItem)
        {
            Id = orderItem.Id;
            ItemId = orderItem.ItemId;
            Name = orderItem.Name;
            PriceAtPurchase = orderItem.PriceAtPurchase;
            Quantity = orderItem.Quantity;
            Subtotal = orderItem.PriceAtPurchase * orderItem.Quantity;
        }

        public long Id { get; set; }
        public long ItemId { get; set; }
        public string Name { get; set; }
        public decimal PriceAtPurchase { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }
}
