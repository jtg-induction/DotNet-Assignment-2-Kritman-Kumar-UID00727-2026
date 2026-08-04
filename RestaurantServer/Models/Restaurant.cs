using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantServer.Models
{
    public class Restaurant
    {
        public Restaurant()
        {
            Items = new HashSet<Item>();
            Orders = new HashSet<Order>();
            RestaurantOwners = new HashSet<RestaurantOwner>();
        }

        [Key]
        public long Id { get; set; }
        [Required]
        [MaxLength(150)]
        public string RestaurantName { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        [MaxLength(20)]
        public string MobileNumber { get; set; }
        [MaxLength(200)]
        public string AddressLine1 { get; set; }
        [MaxLength(200)]
        public string AddressLine2 { get; set; }
        [MaxLength(100)]
        public string City { get; set; }
        [MaxLength(20)]
        public string PostalCode { get; set; }
        [MaxLength(100)]
        public string Country { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        // Navigation
        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<RestaurantOwner> RestaurantOwners { get; set; }
    }
}
