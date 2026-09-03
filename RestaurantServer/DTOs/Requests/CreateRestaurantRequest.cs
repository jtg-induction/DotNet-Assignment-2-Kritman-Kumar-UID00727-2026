using RestaurantServer.Constants;
using RestaurantServer.ModelStateValidator;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantServer.DTOs.Requests
{
    public class CreateRestaurantRequest
    {
        [Required(ErrorMessage = ValidationMessages.RestaurantNameRequired)]
        [MaxLength(ValidationConstants.NameMaxLength)]
        public string RestaurantName { get; set; }

        [MaxLength(ValidationConstants.DescriptionMaxLength)]
        public string Description { get; set; }

        [Required(ErrorMessage = ValidationMessages.MobileNumberRequired)]
        [MaxLength(ValidationConstants.MobileNumberMaxLength)]
        [RegularExpression(Regex.MobileNumberRgex, ErrorMessage = ErrorMessages.InvalidMobileNumber)]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = ValidationMessages.AddressLine1Required)]
        [MaxLength(ValidationConstants.AddressMaxLength)]
        public string AddressLine1 { get; set; }
         
        [MaxLength(ValidationConstants.AddressMaxLength)]
        [Required(ErrorMessage = ValidationMessages.AddressLine2Required)]
        public string AddressLine2 { get; set; }

        [Required(ErrorMessage = ValidationMessages.CityRequired)]
        [MaxLength(ValidationConstants.CityMaxLength)]
        public string City { get; set; }

        [Required(ErrorMessage = ValidationMessages.PostalCodeRequired)]
        [MaxLength(ValidationConstants.PostalCodeMaxLength)]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = ValidationMessages.CountryRequired)]
        [MaxLength(ValidationConstants.CountryMaxLength)]
        public string Country { get; set; }

        [Required(ErrorMessage = ValidationMessages.EmailRequired)]
        [EmailListAttribute]
        [CollectionMinLength]
        public List<string> OwnersEmails { get; set; }
    }
}
