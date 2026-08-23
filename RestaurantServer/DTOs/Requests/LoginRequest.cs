using RestaurantServer.Constants;
using System.ComponentModel.DataAnnotations;

public class LoginRequest
{
    [Required(ErrorMessage = ValidationMessages.EmailRequired)]
    [MaxLength(ValidationConstants.EmailMaxLength, ErrorMessage = ValidationMessages.EmailMaxLength)]
    [RegularExpression(ValidationConstants.EmailRegex, ErrorMessage = ValidationMessages.InvalidEmail)]
    public string Email { get; set; }

    [Required(ErrorMessage = ValidationMessages.PasswordRequired)]
    [MinLength(ValidationConstants.PasswordMinLength, ErrorMessage = ValidationMessages.PasswordMinLength)]
    [MaxLength(ValidationConstants.PasswordMaxLength, ErrorMessage = ValidationMessages.PasswordMaxLength)]
    [RegularExpression(ValidationConstants.PasswordRegex, ErrorMessage = ValidationMessages.InvalidPasswordFormat)]
    public string Password { get; set; }
}
