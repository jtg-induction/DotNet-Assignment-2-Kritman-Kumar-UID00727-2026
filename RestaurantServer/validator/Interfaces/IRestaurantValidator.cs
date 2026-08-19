using RestaurantServer.Models;

namespace RestaurantServer.Validators.Interfaces
{
    public interface IRestaurantValidator
    {
        void ValidateRestaurantExists(Restaurant restaurant);

        void ValidateUserCanBeOwner(User user);

        void ValidateOwnerRelationshipDoesNotExist(RestaurantOwner restaurantOwner);
    }
}
