using RestaurantServer.Models;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Interfaces
{
    public interface IAuthRepository : IRepository<User>
    {
        Task<User> GetUserByEmailAsync(string email);
    }
}
