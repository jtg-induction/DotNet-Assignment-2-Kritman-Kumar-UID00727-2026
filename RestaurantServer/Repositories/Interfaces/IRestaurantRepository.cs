using RestaurantServer.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Interfaces
{
    public interface IRestaurantRepository : IRepository<Restaurant>
    {
        Task<bool> ExistsByMobileNumberAsync(string mobileNumber, CancellationToken cancellationToken = default);
        Task<List<Restaurant>> GetAvailableRestaurantsAsync(int page, int pageSize, bool disableTracking = false, CancellationToken cancellationToken = default);
        Task<int> CountAvailableRestaurantsAsync(CancellationToken cancellationToken = default);
    }
}
