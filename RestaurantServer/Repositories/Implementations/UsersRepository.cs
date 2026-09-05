using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Implementations
{
    public class UsersRepository : Repository<User>, IUsersRepository
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="UsersRepository"/> class.
        /// </summary>
        /// <param name="context">
        /// The database context used to access user data.
        /// </param>
        public UsersRepository(ApplicationDbContext context)
            : base(context)
        {

        }

        /// <summary>
        /// Checks whether a mobile number is already in use by any user other than the specified user.
        /// </summary>
        /// <param name="mobileNumber">The mobile number string to check for duplicates.</param>
        /// <param name="userId">The unique identifier of the user to exclude from the check (typically the current user).</param>
        /// <returns><c>true</c> if the mobile number already exists for another user; otherwise, <c>false</c>.</returns>
        public bool IsMobileNumberExists(string mobileNumber, long userId)
        {
            return _context.Users.Any(user => user.MobileNumber == mobileNumber && user.Id != userId);
        }


        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve.</param>
        /// <param name="disableTracking">
        /// Indicates whether Entity Framework should disable change tracking for the returned user.
        /// Set to <c>true</c> when the user is only needed for read-only purposes.
        /// </param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>
        /// The user matching the specified email address, or <c>null</c> if no matching user is found.
        /// </returns>
        public async Task<User> GetUserByEmailAsync(
            string email, bool disableTracking = false,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Users.AsQueryable();

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            return await query
                .FirstOrDefaultAsync(user => user.Email == email, cancellationToken);


        }

        /// <summary>
        /// returns List of users of given owners emails
        /// </summary>
        /// <param name="emails"> IEnumerable of emails</param>
        /// <returns>List of users</returns>
        public async Task<List<User>> GetUsersByEmailsAsync(
           List<string> emails, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Where(user => emails.Contains(user.Email))
                .ToListAsync(cancellationToken);
        }

        public async Task<User> GetUserForUpdateAsync(long userId, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .SqlQuery("SELECT * FROM Users WITH (UPDLOCK, ROWLOCK) WHERE Id = @p0", userId)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
