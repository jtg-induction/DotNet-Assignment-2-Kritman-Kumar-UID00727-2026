using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RestaurantServer.Constants;

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
        [MaxLength(ValidationConstants.NameMaxLength)]
        public string RestaurantName { get; set; }

        [MaxLength(ValidationConstants.DescriptionMaxLength)]
        public string Description { get; set; }

        [MaxLength(ValidationConstants.MobileNumberMaxLength)]
        public string MobileNumber { get; set; }

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
        public long CreatedBy { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        [Required]
        public long UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        // Navigation
        public virtual User CreatedByUser { get; set; }

        public virtual User UpdatedByUser { get; set; }

        public virtual ICollection<Item> Items { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        public virtual ICollection<RestaurantOwner> RestaurantOwners { get; set; }
    }
}
