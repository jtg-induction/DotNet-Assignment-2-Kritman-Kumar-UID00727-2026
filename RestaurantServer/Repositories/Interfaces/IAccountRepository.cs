using RestaurantServer.Models;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<User> GetUserByIdAsync(long userId);

        Task SaveAsync();
    }
}
