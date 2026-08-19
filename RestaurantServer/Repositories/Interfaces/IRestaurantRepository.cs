using RestaurantServer.Models;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Interfaces
{
    public interface IRestaurantRepository
    {
        Task AddAsync(Restaurant restaurant, CancellationToken cancellationToken = default);

        Task<Restaurant> GetByIdAsync(long restaurantId, CancellationToken cancellationToken = default);
    }
}
