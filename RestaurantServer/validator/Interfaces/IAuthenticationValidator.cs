using RestaurantServer.Models;

namespace RestaurantServer.Validators.Interfaces
{
    public interface IAuthenticationValidator
    {
        void ValidateUser(User user);

        void ValidateUserIsActive(User user);

        void ValidatePassword(bool isValid);
    }
}
