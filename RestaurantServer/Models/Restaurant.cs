using RestaurantServer.Constants;
using RestaurantServer.DTOs.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public Restaurant(CreateRestaurantRequest request, long createdBy)
        {
            var now = DateTime.UtcNow;

            RestaurantName = request.RestaurantName.Trim();
            Description = request.Description?.Trim();
            MobileNumber = request.MobileNumber?.Trim();
            AddressLine1 = request.AddressLine1.Trim();
            AddressLine2 = request.AddressLine2?.Trim();
            City = request.City.Trim();
            PostalCode = request.PostalCode.Trim();
            Country = request.Country.Trim();

            CreatedAt = now;
            CreatedBy = createdBy;
            UpdatedAt = now;
            UpdatedBy = createdBy;
            IsDeleted = false;
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
