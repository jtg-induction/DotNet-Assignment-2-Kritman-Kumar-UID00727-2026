using RestaurantServer.Constants;
using RestaurantServer.Enums;
using RestaurantServer.Exceptions;
using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.Validators.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


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
