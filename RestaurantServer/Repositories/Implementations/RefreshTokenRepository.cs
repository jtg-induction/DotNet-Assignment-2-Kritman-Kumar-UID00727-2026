using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;


namespace RestaurantServer.Repositories.Implementations
{
    public class RefreshTokenRepository
        : Repository<RefreshToken>, IRefreshTokenRepository
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="RefreshTokenRepository"/> class.
        /// </summary>
        /// <param name="context">
        /// The database context used to access refresh token data.
        /// </param>
        public RefreshTokenRepository(ApplicationDbContext context)
            : base(context)
        {

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
        public async Task<RefreshToken> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(refreshToken =>
                    refreshToken.Token == token, cancellationToken);
        }

        /// <summary>
        /// Revokes all active refresh tokens associated with a user.
        /// </summary>
        /// <param name="userId">
        /// The unique identifier of the user whose refresh tokens
        /// should be revoked.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous revoke operation.
        /// </returns>
        public async Task RevokeAllByUserIdAsync(long userId, CancellationToken cancellationToken = default)
        {
            var refreshTokens = await _context.RefreshTokens
                .Where(refreshToken =>
                    refreshToken.UserId == userId &&
                    !refreshToken.IsRevoked)
                .ToListAsync(cancellationToken);

            foreach (var refreshToken in refreshTokens)
            {
                refreshToken.IsRevoked = true;
            }
        }
    }
}
