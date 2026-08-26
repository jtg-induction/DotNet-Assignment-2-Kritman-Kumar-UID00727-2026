using RestaurantServer.Constants;
using RestaurantServer.Enums;
using RestaurantServer.Exceptions;
using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.Validators.Interfaces;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace RestaurantServer.Validators.Implementations
{
    /// <summary>
    /// Provides validation logic for restaurant entities and owner relationships.
    /// </summary>
    public class RestaurantValidator : IRestaurantValidator
    {

        private readonly IRestaurantRepository _restaurantRepository;

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
                throw new ValidationException(ValidationMessages.OwnerRelationshipAlreadyExists);
            }
        }

        /// <summary>
        /// Validates the mobile number to check whether the mobile number exists or not. If it already exists then throw an exception: RestaurantMobileNumberAlreadyExists
        /// </summary>
        /// <param name="mobileNumber">string new restaurant mobile number</param>
        /// <exception cref="ValidationException">thrown when mobile number allready exists</exception>
        public async Task ValidateMobileNumber(string mobileNumber)
        {
            mobileNumber = mobileNumber?.Trim();

            var mobileExists = await _restaurantRepository
                .ExistsByMobileNumberAsync(mobileNumber);

            if (mobileExists)
            {
                throw new ValidationException(ValidationMessages.RestaurantMobileNumberAlreadyExists);
            }
        }

        /// <summary>
        /// Validates the email using Regex.
        /// </summary>
        /// <param name="email">owner email</param>
        /// <exception cref="ValidationException">throws exception when email is empty or when email is invlaid.</exception>
        public void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                    !Regex.IsMatch(email.Trim(), RegexConstants.EmailRegex))
            {
                throw new ValidationException(ValidationMessages.InvalidEmail);
            }
        }


        /// <summary>
        /// validate user role Allow only customer and owner.
        /// </summary>
        /// <param name="role"> intiger user role only from ENUM USER_ROLE</param>
        /// <exception cref="ValidationException"> thows exception when user role is addmin </exception>
        public void ValidateAdminRole(int role)
        {
            if (role == (int)UserRole.Admin)
            {
                throw new ValidationException(ValidationMessages.InvalidRestaurantOwner);
            }
        }


        /// <summary>
        ///  validate the emails count.
        /// </summary>
        /// <param name="emails"> list of string </param>
        /// <exception cref="ValidationException">throws exception when emails count is 0</exception>
        public void IsOwnersEmailEmpty(List<string> emails)
        {
            if (0 == emails.Count)
            {
                throw new ValidationException(ValidationMessages.OnboardRestaurantOwnerEmailsMinLength);
            }
        }

    }
}
