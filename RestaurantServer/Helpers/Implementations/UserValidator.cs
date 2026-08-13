using RestaurantServer.Constants;
using RestaurantServer.Exceptions;
using RestaurantServer.Models;
using RestaurantServer.Validators.Interfaces;

namespace RestaurantServer.Validators.Implementations
{
    public class UserValidator : IUserValidator
    {
        public void ValidateUserExists(User user)
        {
            if (user == null)
            {
                throw new ValidationException(
                    ValidationMessages.UserNotFound);
            }
        }
    }
}
