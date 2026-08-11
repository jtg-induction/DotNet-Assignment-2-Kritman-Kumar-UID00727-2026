using System.Data.Entity;
using System.Threading.Tasks;
using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;

namespace RestaurantServer.Repositories.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="AuthRepository"/> class.
        /// </summary>
        /// <param name="context">
        /// The database context used to access user data.
        /// </param>
        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
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
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(user => user.Email == email);
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
        /// Adds a new user to the database context.
        /// </summary>
        /// <param name="user">
        /// The user entity to add.
        /// </param>
        public void AddUser(User user)
        {
            _context.Users.Add(user);
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
