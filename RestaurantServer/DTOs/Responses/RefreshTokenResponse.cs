using RestaurantServer.Enums;

namespace RestaurantServer.DTOs.Responses
{
    public class RefreshTokenResponse
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public long UserId { get; set; }

        public string Name { get; set; }

        public UserRole Role { get; set; }

        public string Message { get; set; }
    }
}
