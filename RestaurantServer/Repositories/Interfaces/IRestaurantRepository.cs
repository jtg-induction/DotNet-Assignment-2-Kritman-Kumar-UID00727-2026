using RestaurantServer.Models;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Interfaces
{
    public interface IRestaurantRepository: IRepository<Restaurant>
    {
        Task<bool> ExistsByMobileNumberAsync(string mobileNumber, CancellationToken cancellationToken = default);
    }
}
