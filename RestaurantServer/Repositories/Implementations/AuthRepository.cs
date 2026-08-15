using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Implementations
{
    public class AuthRepository : Repository<User>, IAuthRepository
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="AuthRepository"/> class.
        /// </summary>
        /// <param name="context">
        /// The database context used to access user data.
        /// </param>
        public AuthRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">
        /// The email address of the user.
        /// </param>
        /// <returns>
        /// The matching user if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
        }
    }
}
