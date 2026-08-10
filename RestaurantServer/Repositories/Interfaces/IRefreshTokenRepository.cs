using RestaurantServer.Models;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetByTokenAsync(string token);

        Task AddAsync(RefreshToken refreshToken);

        void Update(RefreshToken refreshToken);

        Task RevokeAllByUserIdAsync(long userId);

        Task SaveAsync();
    }
}