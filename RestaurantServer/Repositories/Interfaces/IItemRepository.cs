using RestaurantServer.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Interfaces
{
    public interface IItemRepository : IRepository<Item>
    {
        Task<List<Item>> GetAvailableItemsByRestaurantIdAsync(
            long restaurantId, int page, int pageSize,
            CancellationToken cancellationToken = default);

        Task<int> CountAvailableItemsByRestaurantIdAsync(
            long restaurantId,
            CancellationToken cancellationToken = default);
    }
}
