using System.ComponentModel.DataAnnotations;
using RestaurantServer.Constants;

namespace RestaurantServer.DTOs.Requests
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = ValidationMessages.RefreshTokenRequired)]
        public string RefreshToken { get; set; }
    }
}
