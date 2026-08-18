using System;
using System.ComponentModel.DataAnnotations;

namespace RestaurantServer.Models
{
    public class RestaurantOwner
    {

        public RestaurantOwner() 
        { 
        
        }

        public RestaurantOwner(long restaurantId, long userId)
        {
            RestaurantId = restaurantId;
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        [Key]
        public long Id { get; set; }

        [Required]
        public long RestaurantId { get; set; }

        [Required]
        public long UserId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }
        // Navigation
        public virtual Restaurant Restaurant { get; set; }
        public virtual User User { get; set; }
    }
}
