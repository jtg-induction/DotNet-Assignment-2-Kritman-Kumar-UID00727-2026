using RestaurantServer.Constants;
using RestaurantServer.Exceptions;
using RestaurantServer.Models;
using RestaurantServer.Validators.Interfaces;
using System;

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

        /// <summary>
        /// Validates that the refresh token input is not null, empty, or whitespace.
        /// </summary>
        /// <param name="refreshToken">
        /// The refresh token value provided by the client.
        /// </param>
        /// <exception cref="ValidationException">
        /// Thrown when the refresh token input is missing or empty.
        /// </exception>
        public void ValidateRefreshTokenInput(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ValidationException(
                    ValidationMessages.InvalidRefreshToken);
            }
        }

        /// <summary>
        /// Validates that the specified refresh token exists.
        /// </summary>
        /// <param name="refreshToken">
        /// The refresh token to validate.
        /// </param>
        /// <exception cref="ValidationException">
        /// Thrown when the refresh token does not exist.
        /// </exception>
        public void ValidateRefreshToken(RefreshToken refreshToken)
        {
            if (refreshToken == null)
            {
                throw new ValidationException(
                    ValidationMessages.InvalidRefreshToken);
            }
        }

        /// <summary>
        /// Validates that the specified refresh token has not been revoked.
        /// </summary>
        /// <param name="refreshToken">
        /// The refresh token whose revocation status is being validated.
        /// </param>
        /// <exception cref="ValidationException">
        /// Thrown when the refresh token has been revoked.
        /// </exception>
        public void ValidateRefreshTokenIsNotRevoked(
            RefreshToken refreshToken)
        {
            if (refreshToken.IsRevoked)
            {
                throw new ValidationException(
                    ValidationMessages.InvalidRefreshToken);
            }
        }

        /// <summary>
        /// Validates that the specified refresh token has not expired.
        /// </summary>
        /// <param name="refreshToken">
        /// The refresh token whose expiration status is being validated.
        /// </param>
        /// <exception cref="ValidationException">
        /// Thrown when the refresh token has expired.
        /// </exception>
        public void ValidateRefreshTokenIsNotExpired(
            RefreshToken refreshToken)
        {
            if (refreshToken.ExpiresAt <= DateTime.UtcNow)
            {
                throw new ValidationException(
                    ValidationMessages.InvalidRefreshToken);
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

        /// <summary>
        /// Performs all required validations for a refresh token.
        /// </summary>
        /// <param name="refreshToken">
        /// The refresh token to validate.
        /// </param>
        /// <exception cref="ValidationException">
        /// Thrown when the refresh token does not exist, has been revoked,
        /// or has expired.
        /// </exception>
        public void ValidateRefreshTokenIsValid(RefreshToken refreshToken)
        {
            ValidateRefreshToken(refreshToken);
            ValidateRefreshTokenIsNotRevoked(refreshToken);
            ValidateRefreshTokenIsNotExpired(refreshToken);
        }
    }
}
