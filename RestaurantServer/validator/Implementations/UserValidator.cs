using RestaurantServer.Constants;
using RestaurantServer.Exceptions;
using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.Validators.Interfaces;

namespace RestaurantServer.Validators.Implementations
{
    /// <summary>
    /// Provides validation methods for user-related operations.
    /// </summary>
    public class UserValidator : IUserValidator
    {

        private readonly IUsersRepository _userReposeroty;

        public UserValidator(IUsersRepository usersRepository)
        {
            _userReposeroty = usersRepository;
        }



        /// <summary>
        /// Validates that the specified user exists.
        /// </summary>
        /// <param name="user">
        /// The user to validate.
        /// </param>
        /// <exception cref="ValidationException">
        /// Thrown when the specified user does not exist.
        /// </exception>
        public void ValidateUserExists(User user)
        {
            if (user == null)
            {
                throw new ValidationException(
                    ValidationMessages.UserNotFound);
            }
        }

        /// <summary>
        /// Validates that the user ID from the request matches the authenticated user's ID.
        /// </summary>
        /// <param name="requestedUserId">
        /// The user ID provided in the request URL.
        /// </param>
        /// <param name="authenticatedUserId">
        /// The user ID obtained from the authenticated user's JWT claims.
        /// </param>
        /// <exception cref="ValidationException">
        /// Thrown when the requested user ID does not match the authenticated user's ID.
        /// </exception>
        public void ValidateUserId(
            long requestedUserId,
            long authenticatedUserId)
        {
            if (requestedUserId != authenticatedUserId)
            {
                throw new ValidationException(
                    ValidationMessages.NotAuthorized);
            }
        }

        /// <summary>
        /// Validates that the specified user exists and has an active account.
        /// </summary>
        /// <param name="user">
        /// The user to validate.
        /// </param>
        /// <exception cref="ValidationException">
        /// Thrown when the user does not exist or the user's account is inactive.
        /// </exception>
        public void IsUserNullOrDeactivated(User user)
        {
            if (user == null || !user.IsActive)
            {
                throw new ValidationException(
                    ValidationMessages.InvalidRefreshToken);
            }
        }

        public void ValidateMobileNumberIsUnique(string mobileNumber, long userId)
        {
            if (string.IsNullOrWhiteSpace(mobileNumber))
            {
                return;
            }

            if (_userReposeroty.IsMobileNumberExists(mobileNumber, userId))
            {
                throw new ValidationException(
                    ValidationMessages.MobileNumberAlreadyExists);
            }
        }
    }
}
