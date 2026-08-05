using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RestaurantServer.Constants;

namespace RestaurantServer.Models
{
    public class Order
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }
        [Key]
        public long Id { get; set; }

        [Required]
        public long RestaurantId { get; set; }

        [Required]
        public long UserId { get; set; }

        [Required]
        public int Status { get; set; }
            
        [Range(typeof(decimal), "0", ValidationConstants.DecimalMax)]
        public decimal TotalPrice { get; set; }

        [Required]
        [MaxLength(ValidationConstants.AddressMaxLength)]
        public string AddressLine1 { get; set; }
        
        [MaxLength(ValidationConstants.AddressMaxLength)]
        public string AddressLine2 { get; set; }

        [Required]
        [MaxLength(ValidationConstants.CityMaxLength)]
        public string City { get; set; }

        [Required]
        [MaxLength(ValidationConstants.PostalCodeMaxLength)]
        public string PostalCode { get; set; }

        [Required]
        [MaxLength(ValidationConstants.CountryMaxLength)]
        public string Country { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }
        // Navigation
        public virtual User User { get; set; }
        public virtual Restaurant Restaurant { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
