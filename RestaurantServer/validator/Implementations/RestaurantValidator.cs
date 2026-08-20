using RestaurantServer.Constants; 
using RestaurantServer.Exceptions;
using RestaurantServer.Models; 
using RestaurantServer.Validators.Interfaces; 


namespace RestaurantServer.Validators.Implementations
{
    /// <summary>
    /// Provides validation logic for restaurant entities and owner relationships.
    /// </summary>
    public class RestaurantValidator : IRestaurantValidator
    {
        /// <summary>
        /// Validates that a restaurant exists and has not been deleted.
        /// </summary>
        /// <param name="restaurant">The restaurant entity model to check.</param>
        /// <throws cref="ValidationException">Thrown when the restaurant is null or when its <see cref="Restaurant.IsDeleted"/> status is true.</throws>
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

        /// <summary>
        /// Validates that an owner relationship does not already exist for the restaurant.
        /// </summary>
        /// <param name="restaurantOwner">The restaurant owner entity model to check.</param>
        /// <throws cref="ValidationException">Thrown when the restaurant owner record is not null, indicating a relationship already exists.</throws>
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
