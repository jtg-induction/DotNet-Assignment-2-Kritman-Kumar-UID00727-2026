using RestaurantServer.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Interfaces
{
    public interface IUsersRepository : IRepository<User>
    {
        bool IsMobileNumberExists(string mobileNumber, long userId);

        Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);

        Task<List<User>> GetUsersByEmailsAsync(
            List<string> emails, CancellationToken cancellationToken = default);

        Task<User> GetUserForUpdateAsync(long userId, CancellationToken cancellationToken = default);
    }
}
