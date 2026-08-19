using RestaurantServer.Constants;
using System.ComponentModel.DataAnnotations;

namespace RestaurantServer.DTOs.Requests
{
    public class CreateRestaurantRequest
    {
        [Required]
        [MaxLength(ValidationConstants.NameMaxLength)]
        public string RestaurantName { get; set; }

        [MaxLength(ValidationConstants.DescriptionMaxLength)]
        public string Description { get; set; }

        [MaxLength(ValidationConstants.MobileNumberMaxLength)]
        [RegularExpression(ValidationConstants.MobileNumberRgex, ErrorMessage = ValidationMessages.InvalidMobileNumber)]
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
    }
}
