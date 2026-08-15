using RestaurantServer.Models;

namespace RestaurantServer.Validators.Interfaces
{
    public interface IAuthenticationValidator
    {
        void ValidateUser(User user);

        void ValidateUserIsActive(User user);

        void ValidatePassword(bool isValid);

        void ValidateRefreshTokenInput(string refreshToken);

        void ValidateRefreshToken(RefreshToken refreshToken);

        void ValidateRefreshTokenIsNotRevoked(RefreshToken refreshToken);

        void ValidateRefreshTokenIsNotExpired(RefreshToken refreshToken);

        void IsUserNullOrDeactivated(User user);

        void ValidateRefreshTokenIsValid(RefreshToken refreshToken); 
    }
}
