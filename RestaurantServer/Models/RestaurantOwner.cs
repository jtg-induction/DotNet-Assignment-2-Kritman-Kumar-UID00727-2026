using System;
using System.ComponentModel.DataAnnotations;

namespace RestaurantServer.Models
{
    public class RestaurantOwner
    {
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
