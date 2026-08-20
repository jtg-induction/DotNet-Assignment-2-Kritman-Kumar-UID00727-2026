using Microsoft.Ajax.Utilities;
using RestaurantServer.Constants;
using RestaurantServer.Enums;
using RestaurantServer.Exceptions;
using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.Validators.Interfaces;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


namespace RestaurantServer.Validators.Implementations
{
    /// <summary>
    /// Provides validation logic for restaurant entities and owner relationships.
    /// </summary>
    public class RestaurantValidator : IRestaurantValidator
    {

        private readonly IRestaurantRepository _restaurantRepository;
        private CancellationToken cancellationToken;

        public RestaurantValidator(IRestaurantRepository restaurantRepository)
        {
            _restaurantRepository = restaurantRepository;
        }

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

        public async Task ValidateMobileNumber(string mobileNumber)
        {
            mobileNumber = mobileNumber?.Trim();

            var mobileExists = await _restaurantRepository
                .ExistsByMobileNumberAsync(mobileNumber, cancellationToken);

            if (mobileExists)
            {
                throw new ValidationException(ValidationMessages.RestaurantMobileNumberAlreadyExists);
            }
        }

        public void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                    !Regex.IsMatch(email.Trim(), ValidationConstants.EmailRegex))
            {
                throw new ValidationException(ValidationMessages.InvalidEmail);
            }
        }

        public void ValidateAdminRole(int role)
        {
            if (role == (int)UserRole.Admin)
            {
                throw new ValidationException(ValidationMessages.InvalidRestaurantOwner);
            }
        }

        public void IsOwnersEmailEmpty(List<string> emails)
        {
            if(0 == emails.Count)
            {
                throw new ValidationException(ValidationMessages.OnboardRestaurantOwnerEmailsMinLength);
            }
        }

    }
}
