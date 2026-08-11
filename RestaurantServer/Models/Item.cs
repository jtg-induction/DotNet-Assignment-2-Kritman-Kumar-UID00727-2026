using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RestaurantServer.Constants;

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
        [MaxLength(ValidationConstants.NameMaxLength)]
        public string Name { get; set; }

        [Required]
        [Range(typeof(decimal), "0", ValidationConstants.DecimalMax)]
        public decimal Price { get; set; }

        [Required]
        [Index("IX_Item_RestaurantId")]
        public long RestaurantId { get; set; }

        [Required]
        [Range(0,int.MaxValue)]
        public int Stock { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public virtual Restaurant Restaurant { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
