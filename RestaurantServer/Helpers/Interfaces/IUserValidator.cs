using RestaurantServer.Models;

namespace RestaurantServer.Validators.Interfaces
{
    public interface IUserValidator
    {
        void ValidateUserExists(User user);
    }
}
