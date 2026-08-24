using RestaurantServer.Models;

namespace RestaurantServer.DTOs.Responses
{
    public class OrderItemDto
    {
        public OrderItemDto()
        {
        }

        public OrderItemDto(OrderItem orderItem)
        {
            Id = orderItem.Id;
            ItemId = orderItem.ItemId;
            Name = orderItem.Name;
            PriceAtPurchase = orderItem.PriceAtPurchase;
            Quantity = orderItem.Quantity;

            TotalPrice = orderItem.PriceAtPurchase * orderItem.Quantity;
        }

        public long Id { get; set; }
        public long ItemId { get; set; }
        public string Name { get; set; }
        public decimal PriceAtPurchase { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
