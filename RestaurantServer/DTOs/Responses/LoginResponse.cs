using RestaurantServer.Enums; 

namespace RestaurantServer.DTOs.Responses
{
    public class LoginResponse
    {
        public string AccessToken {  get; set; }
        public long UserId {  get; set; }
        public string Name {  get; set; }
        public UserRole Role { get; set; }
        public string Message {  get; set; } 
    }
}
