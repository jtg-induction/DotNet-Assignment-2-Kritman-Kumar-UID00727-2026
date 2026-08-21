using RestaurantServer.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Interfaces
{
    public interface IAuthRepository : IRepository<User>
    {
        Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);

        Task<List<User>> GetUsersByEmailsAsync(
            IEnumerable<string> emails, CancellationToken cancellationToken = default);
    }
}
