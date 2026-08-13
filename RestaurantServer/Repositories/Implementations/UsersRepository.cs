using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;

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
    }
}
