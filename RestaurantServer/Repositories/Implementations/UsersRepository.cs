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

        public bool IsMobileNumberExists(string mobileNumber, long userId)
        {
            return _context.Users.Any(user => user.MobileNumber == mobileNumber && user.Id != userId);
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

        /// <summary>
        /// returns List of users of given owners emails
        /// </summary>
        /// <param name="emails"> IEnumerable of emails</param>
        /// <returns>List of users</returns>
        public async Task<List<User>> GetUsersByEmailsAsync(
           IEnumerable<string> emails,
           CancellationToken cancellationToken = default)
        {
            var emailList = emails
                .Select(email => email.Trim())
                .ToList();

            return await _context.Users
                .Where(user => emailList.Contains(user.Email))
                .ToListAsync(cancellationToken);
        }
    }
}
