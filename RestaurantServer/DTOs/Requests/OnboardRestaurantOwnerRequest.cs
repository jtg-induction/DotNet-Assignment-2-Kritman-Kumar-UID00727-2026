using RestaurantServer.Constants;
using System.ComponentModel.DataAnnotations;

namespace RestaurantServer.DTOs.Requests
{
    public class OnboardRestaurantOwnerRequest
    {
        [Required(ErrorMessage = ValidationMessages.EmailRequired)]
        [MaxLength(ValidationConstants.EmailMaxLength, ErrorMessage = ValidationMessages.EmailMaxLength)]
        [RegularExpression(ValidationConstants.EmailRegex, ErrorMessage = ValidationMessages.InvalidEmail)]
        public string Email { get; set; }
    }
}
