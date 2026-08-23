using RestaurantServer.Models;

namespace RestaurantServer.validator.Interfaces
{
    public interface IRefreshTokenValidator
    {
        void ValidateRefreshTokenInput(string refreshToken);

        void ValidateRefreshToken(RefreshToken refreshToken);

        void ValidateRefreshTokenIsNotRevoked(RefreshToken refreshToken);

        void ValidateRefreshTokenIsNotExpired(RefreshToken refreshToken);

        void ValidateRefreshTokenIsValid(RefreshToken refreshToken);
    }
}
