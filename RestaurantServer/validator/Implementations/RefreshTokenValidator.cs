using RestaurantServer.Constants;
using RestaurantServer.Exceptions;
using RestaurantServer.Models;
using RestaurantServer.validator.Interfaces;
using System;

namespace RestaurantServer.Validators.Implementations
{
    public class RefreshTokenValidator : IRefreshTokenValidator
    {
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
                    ErrorMessages.InvalidRefreshToken);
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
                    ErrorMessages.InvalidRefreshToken);
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
                    ErrorMessages.InvalidRefreshToken);
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
                    ErrorMessages.InvalidRefreshToken);
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
