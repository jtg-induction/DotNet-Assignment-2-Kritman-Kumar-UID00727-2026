using RestaurantServer.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantServer.DTOs.Requests
{
    public class CreateOrderRequest
    {
        [Required]
        public List<OrderItemRequest> Items { get; set; }

        [Required(ErrorMessage = ValidationMessages.AddressLine1Required)]
        [MaxLength(ValidationConstants.AddressMaxLength)]
        public string AddressLine1 { get; set; }

        [MaxLength(ValidationConstants.AddressMaxLength)]
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
    }
}
