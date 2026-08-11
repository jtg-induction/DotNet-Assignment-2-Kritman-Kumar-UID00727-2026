using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using System.Data.Entity;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Implementations
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="AccountRepository"/> class.
        /// </summary>
        /// <param name="context">
        /// The database context used to access account data.
        /// </param>
        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="userId">
        /// The unique identifier of the user.
        /// </param>
        /// <returns>
        /// The matching user if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<User> GetUserByIdAsync(long userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(user => user.Id == userId);
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
