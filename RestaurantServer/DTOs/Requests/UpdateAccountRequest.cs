using RestaurantServer.Constants;
using System.ComponentModel.DataAnnotations;

namespace RestaurantServer.DTOs.Requests
{
    public class UpdateAccountRequest
    {
        [Required(ErrorMessage = ValidationMessages.NameRequired)]
        [MaxLength(ValidationConstants.NameMaxLength, ErrorMessage = ValidationMessages.NameMaxLength)]
        public string Name { get; set; }

        [Required(ErrorMessage = ValidationMessages.MobileNumberRequired)]
        [RegularExpression(ValidationConstants.MobileNumberRgex, ErrorMessage = ValidationMessages.InvalidMobileNumber)]
        public string MobileNumber { get; set; }
    }
}
