using RestaurantServer.Models;

namespace RestaurantServer.DTOs.Responses
{
    public class ItemDto
    {
        public ItemDto()
        {
        }

        public ItemDto(Item item)
        {
            Id = item.Id;
            Name = item.Name;
            Price = item.Price;
            Stock = item.Stock;
            RestaurantId = item.RestaurantId;
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public long RestaurantId { get; set; }
    }
}
