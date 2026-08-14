using RestaurantServer.Constants;
using RestaurantServer.Models;

namespace RestaurantServer.DTOs.Responses
{
    public class SignupResponse
    {

        public SignupResponse()
        {
        }

        public SignupResponse(User user)
        {
            UserId = user.Id;
            Name = user.Name;
            Email = user.Email;
            Message = SuccessMessages.UserRegistered;
        }

        public long UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
    }
}
