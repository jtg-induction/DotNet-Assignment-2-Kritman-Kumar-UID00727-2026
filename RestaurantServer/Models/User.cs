using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
 

namespace RestaurantServer.Models
{
    public class User
    {
        public User()
        {
            Orders = new HashSet<Order>();
            RestaurantOwners = new HashSet<RestaurantOwner>();
        }
        [Key]
        public long Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [MaxLength(150)]
        public string Email { get; set; }
        [Required]
        [MaxLength(500)]
        public string PasswordHash { get; set; }
        public decimal Balance { get; set; }
        [Required]
          
        public int Role { get; set; }
        public bool IsActive { get; set; }
        [MaxLength(20)]
        public string MobileNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        // Navigation
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<RestaurantOwner> RestaurantOwners { get; set; }
    }
}
