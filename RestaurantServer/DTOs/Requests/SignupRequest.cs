using System.ComponentModel.DataAnnotations;
using RestaurantServer.Constants;

namespace RestaurantServer.DTOs.Requests
{
    public class SignupRequest
    {
        [Required(ErrorMessage = ValidationMessages.NameRequired)]
        [MaxLength(ValidationConstants.NameMaxLength, ErrorMessage = ValidationMessages.NameMaxLength)]
        public string Name { get; set; }

        [Required(ErrorMessage = ValidationMessages.EmailRequired)]
        [MaxLength(ValidationConstants.EmailMaxLength, ErrorMessage = ValidationMessages.EmailMaxLength)]
        public string Email { get; set; }

        [Required(ErrorMessage = ValidationMessages.PasswordRequired)]
        [MinLength(ValidationConstants.PasswordMinLength, ErrorMessage = ValidationMessages.PasswordMinLength)]
        [MaxLength(ValidationConstants.PasswordMaxLength, ErrorMessage = ValidationMessages.PasswordMaxLength)]
        [RegularExpression(RegexConstants.PasswordRegex, ErrorMessage = ValidationMessages.InvalidPasswordFormat)]
        public string Password { get; set; }
    }
}
