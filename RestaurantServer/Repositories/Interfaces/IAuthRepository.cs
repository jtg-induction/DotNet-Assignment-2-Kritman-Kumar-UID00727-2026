using System.Threading.Tasks;
using RestaurantServer.Models;

namespace RestaurantServer.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(long userId);
        void AddUser(User user);
        Task SaveAsync();
    }
}
