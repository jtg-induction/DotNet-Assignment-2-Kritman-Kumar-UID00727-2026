using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantServer.Models
{
    public class Item
    {
        public Item()
        {
            OrderItems = new HashSet<OrderItem>();
        }
        [Key]
        public long Id { get; set; }
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
        public decimal Price { get; set; }
        public long RestaurantId { get; set; }
        public int Stock { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public virtual Restaurant Restaurant { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
