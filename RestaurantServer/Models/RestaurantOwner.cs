using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantServer.Models
{
    public class RestaurantOwner
    {

        public RestaurantOwner()
        {

        }

        public RestaurantOwner(Restaurant restaurant, User user)
        {
            Restaurant = restaurant;
            User = user;
        }

        [Key]
        public long Id { get; set; }

        [Required]
        [Index("IX_RestaurantOwner_RestaurantId_UserId", 1, IsUnique = true)]
        public long RestaurantId { get; set; }

        [Required]
        [Index("IX_RestaurantOwner_RestaurantId_UserId", 2, IsUnique = true)]
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
