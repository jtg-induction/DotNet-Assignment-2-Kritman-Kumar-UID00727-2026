using RestaurantServer.Models;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
        : IRepository<RefreshToken>
    {
        Task<RefreshToken> GetByTokenAsync(string token);
        Task RevokeAllByUserIdAsync(long userId);
    }
}
