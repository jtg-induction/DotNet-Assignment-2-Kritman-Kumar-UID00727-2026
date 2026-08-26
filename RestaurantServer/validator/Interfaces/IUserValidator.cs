using RestaurantServer.Constants;
using RestaurantServer.Models;

namespace RestaurantServer.Validators.Interfaces
{
    public interface IUserValidator
    {
        void ValidateUserExists(User user);
        void ValidateUserId(long requestedUserId, long authenticatedUserId);
        void IsUserNullOrDeactivated(User user, string message = ValidationMessages.InvalidRefreshToken);
        void ValidateMobileNumberIsUnique(string mobileNumber, long userId);
        void ValidateUserBalance(User user, decimal totalPrice);

    }
}
