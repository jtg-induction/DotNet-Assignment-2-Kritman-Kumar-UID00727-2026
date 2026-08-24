using RestaurantServer.Models;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Interfaces
{
    public interface IUsersRepository : IRepository<User>
    {
        bool IsMobileNumberExists(string mobileNumber, long userId);
        Task<User> GetUserForUpdateAsync(long userId, CancellationToken cancellationToken = default);
    }
}
