using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using RestaurantServer.Enums;
using RestaurantServer.Repositories.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Implementations
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the top ordered items across all restaurants owned
        /// by the specified restaurant owner.
        /// </summary>
        /// <param name="ownerId">
        /// The identifier of the restaurant owner.
        /// </param>
        /// <param name="request">
        /// The report query parameters.
        /// </param>
        /// <param name="cancellationToken">
        /// A token used to cancel the operation.
        /// </param>
        /// <returns>
        /// A list of the top ordered items grouped by restaurant and item.
        /// </returns>
        public async Task<List<TopOrderedItemResponse>> GetTopOrderedItemsAsync(
            long ownerId, TopOrderedItemsRequest request,
            CancellationToken cancellationToken = default)
        {
            var query = _context.OrderItems.AsNoTracking()
                .Where(orderItem =>
                    orderItem.Order.Status == (int)OrderStatus.Delivered &&
                    orderItem.Order.Restaurant.RestaurantOwners
                    .Any(owner => owner.UserId == ownerId));

            if (request.ExcludeItemIds != null &&
                request.ExcludeItemIds.Any())
            {
                query = query.Where(orderItem =>
                    !request.ExcludeItemIds.Contains(orderItem.ItemId));
            }

            return await query
                .GroupBy(orderItem => new
                {
                    ItemId = orderItem.ItemId,
                    ItemName = orderItem.Item.Name,
                    RestaurantId = orderItem.Item.RestaurantId,
                    RestaurantName = orderItem.Item.Restaurant.RestaurantName
                })
                .Select(group => new TopOrderedItemResponse
                {
                    ItemId = group.Key.ItemId,
                    ItemName = group.Key.ItemName,
                    RestaurantId = group.Key.RestaurantId,
                    RestaurantName = group.Key.RestaurantName,
                    TotalQuantityOrdered = group.Sum(
                    orderItem => orderItem.Quantity)
                })
                .OrderByDescending(result => result.TotalQuantityOrdered)
                .Take(request.TopItems)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the items that are most frequently bought together
        /// within the specified restaurant.
        /// </summary>
        /// <param name="ownerId">
        /// The identifier of the restaurant owner.
        /// </param>
        /// <param name="restaurantId">
        /// The identifier of the restaurant.
        /// </param>
        /// <param name="request">
        /// The frequently bought together report query parameters.
        /// </param>
        /// <param name="cancellationToken">
        /// A token used to cancel the operation.
        /// </param>
        /// <returns>
        /// A list of item pairs ordered by how frequently
        /// they were bought together.
        /// </returns>
        public async Task<List<FrequentlyBoughtTogetherResponse>>
            GetFrequentlyBoughtTogetherAsync(
                long ownerId, long restaurantId,
                FrequentlyBoughtTogetherRequest request,
                CancellationToken cancellationToken = default)
        {
            var query = _context.OrderItems
                .AsNoTracking()
                .Where(orderItem =>
                    orderItem.Order.Status == (int)OrderStatus.Delivered &&
                    orderItem.Order.RestaurantId == restaurantId &&
                    orderItem.Order.Restaurant.RestaurantOwners
                    .Any(owner => owner.UserId == ownerId));

            var itemPairs = query.SelectMany(
                firstItem => firstItem.Order.OrderItems,
                (firstItem, secondItem) => new
                {
                    ItemA = firstItem,
                    ItemB = secondItem
                });

            itemPairs = itemPairs.Where(pair =>
                pair.ItemA.ItemId < pair.ItemB.ItemId);

            return await itemPairs
                .GroupBy(pair => new
                {
                    Item1Id = pair.ItemA.ItemId,
                    Item1Name = pair.ItemA.Item.Name,
                    Item2Id = pair.ItemB.ItemId,
                    Item2Name = pair.ItemB.Item.Name,
                    RestaurantId = pair.ItemA.Order.RestaurantId,
                    RestaurantName =
                        pair.ItemA.Order.Restaurant.RestaurantName
                })
                .Select(group => new FrequentlyBoughtTogetherResponse
                {
                    Item1Id = group.Key.Item1Id,
                    Item1Name = group.Key.Item1Name,
                    Item2Id = group.Key.Item2Id,
                    Item2Name = group.Key.Item2Name,
                    RestaurantId = group.Key.RestaurantId,
                    RestaurantName = group.Key.RestaurantName,
                    TotalTimesBoughtTogether = group.Count()
                })
                .OrderByDescending(result =>
                    result.TotalTimesBoughtTogether)
                .ThenBy(result => result.Item1Id)
                .ThenBy(result => result.Item2Id)
                .Take(request.TopPairs)
                .ToListAsync(cancellationToken);
        }
    }
}
