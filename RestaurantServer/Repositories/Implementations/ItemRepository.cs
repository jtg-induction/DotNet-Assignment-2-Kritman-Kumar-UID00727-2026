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

        public async Task<List<Item>> GetItemsForUpdateAsync(
            IEnumerable<long> itemIds,
            CancellationToken cancellationToken = default)
        {
            var sortedIds = itemIds.OrderBy(id => id).Distinct().ToList();
            var items = new List<Item>();

            foreach (var itemId in sortedIds)
            {
                var item = await _context.Items
                    .SqlQuery("SELECT * FROM Items WITH (UPDLOCK, ROWLOCK) WHERE Id = @p0", itemId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (item != null)
                {
                    items.Add(item);
                }
            }

            return items;
        }
    }
}
