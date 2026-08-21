using RestaurantServer.Models;

namespace RestaurantServer.DTOs.Responses
{
    public class OwnerDto
    {
        public OwnerDto(RestaurantOwner restaurantOwner)
        {
            UserId = restaurantOwner.UserId;
            UserName = restaurantOwner.User.Name;
            Email = restaurantOwner.User.Email;
            Role = restaurantOwner.User.Role;
        }

        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
    }
}
