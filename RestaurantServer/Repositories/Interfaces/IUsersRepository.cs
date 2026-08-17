using RestaurantServer.Models;

namespace RestaurantServer.Repositories.Interfaces
{
    public interface IUsersRepository : IRepository<User>
    {
        bool IsMobileNumberExists(string mobileNumber, long userId);
    }
}
