using RestaurantServer.Constants;
using System.ComponentModel.DataAnnotations;

public class LoginRequest
{
    [Required(ErrorMessage = ValidationMessages.EmailRequired)]
    [EmailAddress(ErrorMessage = ValidationMessages.InvalidEmail)]
    [MaxLength(
        ValidationConstants.EmailMaxLength,
        ErrorMessage = ValidationMessages.EmailMaxLength)]
    public string Email { get; set; }

    [Required(ErrorMessage = ValidationMessages.PasswordRequired)]
    [MinLength(
        ValidationConstants.PasswordMinLength,
        ErrorMessage = ValidationMessages.PasswordMinLength)]
    [MaxLength(
        ValidationConstants.PasswordMaxLength,
        ErrorMessage = ValidationMessages.PasswordMaxLength)]
    public string Password { get; set; }
}
