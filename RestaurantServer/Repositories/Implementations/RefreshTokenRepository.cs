using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using System.Data.Entity;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Implementations
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public RefreshTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(refreshToken =>
                    refreshToken.Token == token);
        }

        public async Task AddAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            await Task.CompletedTask;
        }

        public void Update(RefreshToken refreshToken)
        {
            _context.Entry(refreshToken).State = EntityState.Modified;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
