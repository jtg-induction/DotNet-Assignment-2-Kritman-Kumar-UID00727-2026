using RestaurantServer.Models;

namespace RestaurantServer.DTOs.Responses
{
    public class OnboardRestaurantOwnerResponse
    {

        public OnboardRestaurantOwnerResponse()
        {

        }

        public OnboardRestaurantOwnerResponse(
           RestaurantOwner restaurantOwner)
        {
            RestaurantId = restaurantOwner.RestaurantId;
            UserId = restaurantOwner.UserId;
            UserName = restaurantOwner.User.Name;
            Email = restaurantOwner.User.Email;
            Role = restaurantOwner.User.Role;
        }

        public long RestaurantId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
    }
}
