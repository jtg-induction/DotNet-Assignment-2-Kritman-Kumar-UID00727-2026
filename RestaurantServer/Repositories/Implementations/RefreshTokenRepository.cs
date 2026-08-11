using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Implementations
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="RefreshTokenRepository"/> class.
        /// </summary>
        /// <param name="context">
        /// The database context used to access refresh token data.
        /// </param>
        public RefreshTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a refresh token by its token value.
        /// </summary>
        /// <param name="token">
        /// The refresh token value to search for.
        /// </param>
        /// <returns>
        /// The matching refresh token if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<RefreshToken> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(refreshToken =>
                    refreshToken.Token == token);
        }

        /// <summary>
        /// Adds a new refresh token to the database context.
        /// </summary>
        /// <param name="refreshToken">
        /// The refresh token entity to add.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        public async Task AddAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Marks an existing refresh token as modified in the database context.
        /// </summary>
        /// <param name="refreshToken">
        /// The refresh token entity to update.
        /// </param>
        public void Update(RefreshToken refreshToken)
        {
            _context.Entry(refreshToken).State = EntityState.Modified;
        }

        /// <summary>
        /// Revokes all active refresh tokens associated with a user.
        /// </summary>
        /// <param name="userId">
        /// The unique identifier of the user whose refresh tokens should be revoked.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous revoke operation.
        /// </returns>
        public async Task RevokeAllByUserIdAsync(long userId)
        {
            var refreshTokens = await _context.RefreshTokens
                .Where(refreshToken =>
                    refreshToken.UserId == userId &&
                    !refreshToken.IsRevoked)
                .ToListAsync();

            foreach (var refreshToken in refreshTokens)
            {
                refreshToken.IsRevoked = true;
                refreshToken.UpdatedAt = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Persists the pending changes in the database context.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous save operation.
        /// </returns>
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
