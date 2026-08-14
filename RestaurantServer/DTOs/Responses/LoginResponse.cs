using RestaurantServer.Enums;
using RestaurantServer.Models;


namespace RestaurantServer.DTOs.Responses
{
    public class LoginResponse
    {

        public LoginResponse()
        {
        }

        public LoginResponse(User user, string accessToken, string message)
        {
            AccessToken = accessToken;
            UserId = user.Id;
            Name = user.Name;
            Role = (UserRole)user.Role;
            Message = message;
        }

        public string AccessToken {  get; set; }
        public long UserId {  get; set; }
        public string Name {  get; set; }
        public UserRole Role { get; set; }
        public string Message {  get; set; } 
    }
}
