using RestaurantServer.Constants;
using RestaurantServer.Models;

namespace RestaurantServer.DTOs.Responses
{
    public class UpdateUserResponse
    {
        public long UserId {  get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNumber {  get; set; }
        public string Message {  get; set; }

        public UpdateUserResponse(User user)
        {
            UserId = user.Id;
            Name = user.Name;
            Email = user.Email;
            MobileNumber = user.MobileNumber;
            Message = SuccessMessages.AccountUpdateSuccessful;
        }
    }
}
