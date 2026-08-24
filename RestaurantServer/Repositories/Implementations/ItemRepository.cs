using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Implementations
{
    public class ItemRepository : Repository<Item>, IItemRepository
    {
        public ItemRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<List<Item>> GetAvailableItemsByRestaurantIdAsync(
            long restaurantId, int page, int pageSize,
            CancellationToken cancellationToken = default)
        {
            return await _context.Items
                .Where(item => item.RestaurantId == restaurantId && !item.IsDeleted)
                .OrderBy(item => item.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountAvailableItemsByRestaurantIdAsync(
            long restaurantId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Items
                .CountAsync(item => item.RestaurantId == restaurantId && !item.IsDeleted, cancellationToken);
        }
    }
}
