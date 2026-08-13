using RestaurantServer.Models;

namespace RestaurantServer.Validators.Interfaces
{
    public interface IAuthenticationValidator
    {
        void ValidateUserIsActive(User user);
        void ValidateRefreshTokenIsNotRevoked(RefreshToken refreshToken);
        void ValidateRefreshTokenUser(User user);
    }
}
