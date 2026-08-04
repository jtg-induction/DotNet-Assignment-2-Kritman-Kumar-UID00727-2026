using System;
using System.ComponentModel.DataAnnotations;

namespace RestaurantServer.Models
{
    public class RestaurantOwner
    {
        [Key]
        public long Id { get; set; }
        public long RestaurantId { get; set; }
        public long UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        // Navigation
        public virtual Restaurant Restaurant { get; set; }
        public virtual User User { get; set; }
    }
}
