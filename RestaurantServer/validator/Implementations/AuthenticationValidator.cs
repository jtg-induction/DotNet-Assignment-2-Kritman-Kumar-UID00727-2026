using RestaurantServer.Constants;
using RestaurantServer.Exceptions;
using RestaurantServer.Models;
using RestaurantServer.Validators.Interfaces;

namespace RestaurantServer.Validators.Implementations
{
    public class AuthenticationValidator : IAuthenticationValidator
    {
        /// <summary>
        /// Validates that the specified user exists.
        /// </summary>
        /// <param name="user">
        /// The user to validate.
        /// </param>
        /// <exception cref="ValidationException">
        /// Thrown when the user is null.
        /// </exception>
        public void ValidateUser(User user)
        {
            if (user == null)
            {
                throw new ValidationException(
                    ValidationMessages.InvalidCredentials);
            }
        }

        /// <summary>
        /// Validates that the specified user's account is active.
        /// </summary>
        /// <param name="user">
        /// The user whose account status is being validated.
        /// </param>
        /// <exception cref="ValidationException">
        /// Thrown when the user's account is inactive.
        /// </exception>
        public void ValidateUserIsActive(User user)
        {
            if (!user.IsActive)
            {
                throw new ValidationException(
                    ValidationMessages.UserInactive);
            }
        }

        /// <summary>
        /// Validates the result of a password verification operation.
        /// </summary>
        /// <param name="isValid">
        /// Indicates whether the provided password matches the stored password.
        /// </param>
        /// <exception cref="ValidationException">
        /// Thrown when the password is invalid.
        /// </exception>
        public void ValidatePassword(bool isValid)
        {
            if (!isValid)
            {
                throw new ValidationException(
                    ValidationMessages.InvalidCredentials);
            }
        }
    }
}
