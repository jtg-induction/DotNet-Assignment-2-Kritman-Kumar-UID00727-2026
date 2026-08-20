using RestaurantServer.Models;

namespace RestaurantServer.Validators.Interfaces
{
    public interface IRestaurantValidator
    {
        void ValidateRestaurantExists(Restaurant restaurant);

        void ValidateOwnerRelationshipDoesNotExist(RestaurantOwner restaurantOwner);
    }
}
