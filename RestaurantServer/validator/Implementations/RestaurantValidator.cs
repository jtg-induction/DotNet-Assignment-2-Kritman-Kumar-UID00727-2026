using RestaurantServer.Constants;
using RestaurantServer.Exceptions;
using RestaurantServer.Models;
using RestaurantServer.Validators.Interfaces;

namespace RestaurantServer.Validators.Implementations
{
    public class RestaurantValidator : IRestaurantValidator
    {
        public void ValidateRestaurantExists(
            Restaurant restaurant)
        {
            if (restaurant == null)
            {
                throw new ValidationException(ValidationMessages.RestaurantNotExists);
            }

            if (restaurant.IsDeleted)
            {
                throw new ValidationException(ValidationMessages.RestaurantNotavailable);
            }
        }

        public void ValidateUserCanBeOwner(
            User user)
        {
            if (user == null)
            {
                throw new ValidationException(ValidationMessages.UserNotFound);
            }

            if (!user.IsActive)
            {
                throw new ValidationException(ValidationMessages.UserInactive);
            }
        }

        public void ValidateOwnerRelationshipDoesNotExist(
            RestaurantOwner restaurantOwner)
        {
            if (restaurantOwner != null)
            {
                throw new ValidationException(ValidationMessages.AlreadyOwner);
            }
        }
    }
}
