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

        /// <summary>
        /// Retrieves a paginated list of available (non-deleted) items for the specified restaurant.
        /// </summary>
        /// <param name="restaurantId">The unique identifier of the restaurant.</param>
        /// <param name="page">The page number to retrieve. Must be greater than zero.</param>
        /// <param name="pageSize">The number of items to retrieve per page.</param>
        /// <param name="disableTracking">
        /// Indicates whether Entity Framework Core should disable change tracking for the returned entities.
        /// Set to <c>true</c> for read-only scenarios.
        /// </param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A list of available items for the specified restaurant and page.</returns>
        public async Task<List<Item>> GetAvailableItemsByRestaurantIdAsync(
            long restaurantId,
            int page,
            int pageSize,
            bool disableTracking = false,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Items
                .Where(item => item.RestaurantId == restaurantId && !item.IsDeleted);

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            return await query
                .OrderBy(item => item.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Counts the number of available (non-deleted) items for the specified restaurant.
        /// </summary>
        /// <param name="restaurantId">The unique identifier of the restaurant.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The number of available items for the specified restaurant.</returns>
        public async Task<int> CountAvailableItemsByRestaurantIdAsync(
            long restaurantId, 
            CancellationToken cancellationToken = default)
        {
            var query = _context.Items
                .Where(item => item.RestaurantId == restaurantId && !item.IsDeleted);

            return await query.CountAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves the specified items for update while applying SQL row-level update locks.
        /// Items are processed in ascending order of their IDs to help maintain a consistent
        /// locking order and reduce the risk of deadlocks during concurrent updates.
        /// </summary>
        /// <param name="itemIds">The list of item IDs to retrieve and lock for update.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>
        /// A list of items that were found and successfully retrieved with update locks.
        /// Items that do not exist are excluded from the result.
        /// </returns>
        public async Task<List<Item>> GetItemsForUpdateAsync(
            List<long> itemIds,
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
