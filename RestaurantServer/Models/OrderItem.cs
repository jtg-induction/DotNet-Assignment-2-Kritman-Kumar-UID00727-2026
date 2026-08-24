using System;
using System.ComponentModel.DataAnnotations;
using RestaurantServer.Constants;

namespace RestaurantServer.Models
{
    public class OrderItem
    {
        public OrderItem() { 
        
        }

        public OrderItem(Item item, int quantity) {
            ItemId = item.Id;
            Name = item.Name;
            PriceAtPurchase = item.Price;
            Quantity = quantity;
        }

        [Key]
        public long Id { get; set; }
        
        [Required]
        [MaxLength(ValidationConstants.NameMaxLength)]
        public string Name { get; set; }

        [Required]
        [Range(typeof(decimal), "0", ValidationConstants.DecimalMax)]
        public decimal PriceAtPurchase { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public long ItemId { get; set; }

        [Required]
        public long OrderId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        // Navigation
        public virtual Item Item { get; set; }
        public virtual Order Order { get; set; }
    }
}
