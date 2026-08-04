using System;
using System.ComponentModel.DataAnnotations;

namespace RestaurantServer.Models
{
    public class OrderItem
    {

        [Key]
        public long Id { get; set; }
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
        public decimal PriceAtPurchase { get; set; }
        public int Quantity { get; set; }
        public long ItemId { get; set; }
        public long OrderId { get; set; }
        public DateTime CreatedAt { get; set; }
        // Navigation
        public virtual Item Item { get; set; }
        public virtual Order Order { get; set; }
    }
}