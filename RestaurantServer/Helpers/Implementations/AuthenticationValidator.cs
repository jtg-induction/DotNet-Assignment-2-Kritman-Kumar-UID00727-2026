using RestaurantServer.Constants;
using RestaurantServer.Exceptions;
using RestaurantServer.Models;
using RestaurantServer.Validators.Interfaces;

namespace RestaurantServer.Validators.Implementations
{
    public class AuthenticationValidator : IAuthenticationValidator
    {
        public void ValidateUserIsActive(User user)
        {
            if (!user.IsActive)
            {
                throw new ValidationException(
                    ValidationMessages.UserInactive);
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
