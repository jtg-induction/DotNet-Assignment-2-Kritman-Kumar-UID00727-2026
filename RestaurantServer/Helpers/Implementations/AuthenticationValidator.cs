using RestaurantServer.Constants;
using RestaurantServer.Exceptions;
using RestaurantServer.Models;
using RestaurantServer.Validators.Interfaces;
using System;

namespace RestaurantServer.Validators.Implementations
{
    public class AuthenticationValidator : IAuthenticationValidator
    {
        public void ValidateUser(User user)
        {
            if (user == null)
            {
                throw new ValidationException(
                    ValidationMessages.InvalidCredentials);
            }
        }

        public void ValidateUserIsActive(User user)
        {
            if (!user.IsActive)
            {
                throw new ValidationException(
                    ValidationMessages.UserInactive);
            }
        }

        public void ValidatePassword(bool isValid)
        {
            if (!isValid)
            {
                throw new ValidationException(
                    ValidationMessages.InvalidCredentials);
            }
        }

        public void ValidateRefreshTokenInput(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ValidationException(
                    ValidationMessages.InvalidRefreshToken);
            }
        }

        public void ValidateRefreshToken(RefreshToken refreshToken)
        {
            if (refreshToken == null)
            {
                throw new ValidationException(
                    ValidationMessages.InvalidRefreshToken);
            }
        }

        public void ValidateRefreshTokenIsNotRevoked(
            RefreshToken refreshToken)
        {
            if (refreshToken.IsRevoked)
            {
                throw new ValidationException(
                    ValidationMessages.InvalidRefreshToken);
            }
        }

        public void ValidateRefreshTokenIsNotExpired(
            RefreshToken refreshToken)
        {
            if (refreshToken.ExpiresAt <= DateTime.UtcNow)
            {
                throw new ValidationException(
                    ValidationMessages.InvalidRefreshToken);
            }
        }

        public void ValidateRefreshTokenUser(User user)
        {
            if (user == null || !user.IsActive)
            {
                throw new ValidationException(
                    ValidationMessages.InvalidRefreshToken);
            }
        }
    }
}
