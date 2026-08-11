namespace RestaurantServer.DTOs.Responses
{
    public class LoginResult
    {
        public LoginResponse Response { get; set; }
        public string RefreshToken { get; set; }
    }
}
