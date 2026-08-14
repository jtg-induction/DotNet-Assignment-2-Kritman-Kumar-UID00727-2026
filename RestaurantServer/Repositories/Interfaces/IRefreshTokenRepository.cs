using RestaurantServer.Models;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
        : IRepository<RefreshToken>
    {
        Task<RefreshToken> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
        Task RevokeAllByUserIdAsync(long userId, CancellationToken cancellationToken = default);
    }
}
