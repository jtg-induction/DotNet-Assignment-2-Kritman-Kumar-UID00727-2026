using RestaurantServer.DTOs.Enums;
using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Implementations
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Asynchronously retrieves an order by its identifier, including its associated order items.
        /// </summary>
        /// <param name="orderId">
        /// The unique identifier of the order to retrieve.
        /// </param>
        /// <param name="disableTracking">
        /// A value indicating whether Entity Framework tracking should be disabled.
        /// When set to <see langword="true"/>, <see cref="DbExtensions.AsNoTracking{T}(System.Linq.IQueryable{T})" />
        /// is applied to the query. This is recommended when the returned order and its items
        /// are intended for read-only purposes and do not need to be tracked for updates.
        /// </param>
        /// <param name="cancellationToken">
        /// A token to observe while waiting for the asynchronous operation to complete.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the
        /// matching order with its associated order items, or <see langword="null"/> if no
        /// order with the specified identifier exists.
        /// </returns>
        public async Task<Order> GetOrderWithItemsByIdNoTrackingAsync(long orderId,
            bool disableTracking = false, CancellationToken cancellationToken = default)
        {
            IQueryable<Order> query = _context.Orders
                .Include(order => order.OrderItems);

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(
                order => order.Id == orderId,
                cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieves an order by its identifier for update while applying
        /// SQL row-level update locks to prevent concurrent modifications.
        /// </summary>
        /// <param name="orderId">
        /// The unique identifier of the order to retrieve.
        /// </param>
        /// <param name="cancellationToken">
        /// A token to observe while waiting for the asynchronous operation to complete.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// the matching order if found; otherwise, <see langword="null"/>.
        /// </returns>
        public async Task<Order> GetOrderForUpdateAsync(long orderId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .SqlQuery("SELECT * FROM Orders WITH (UPDLOCK, ROWLOCK) WHERE Id = @p0", orderId)
                .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the orders matching the specified filters, sorting, and pagination settings.
        /// </summary>
        /// <param name="ownerId">The ID of the restaurant owner.</param>
        /// <param name="orderQueryParameters">The filters, sorting, and pagination parameters.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The total number of matching orders and the orders for the requested page.</returns>
        public async Task<(int TotalRecords, List<OrderResponse> Orders)> GetFilteredOrdersNoTrackingAsync(
            long ownerId, OrderQueryParameters orderQueryParameters, CancellationToken cancellationToken = default)
        {
            var query = _context.Orders.AsNoTracking()
                .Where(order => order.Restaurant.RestaurantOwners
                .Any(owner => owner.UserId == ownerId));

            if (orderQueryParameters.OrderId.HasValue)
            {
                var searchOrder = await query
                    .Where(order => order.Id == orderQueryParameters.OrderId.Value)
                    .Select(order => new OrderResponse
                    {
                        OrderId = order.Id,
                        RestaurantName = order.Restaurant.RestaurantName,
                        Status = (order.Status).ToString(),
                        TotalPrice = order.TotalPrice,
                        CreatedAt = order.CreatedAt,
                        AddressLine1 = order.AddressLine1,
                        AddressLine2 = order.AddressLine2,
                        City = order.City,
                        PostalCode = order.PostalCode,
                        Country = order.Country
                    })
                    .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

                if (searchOrder == null)
                {
                    return (0, new List<OrderResponse>());
                }

                return (1, new List<OrderResponse> { searchOrder });
            }

            if (orderQueryParameters.Status.HasValue)
            {
                var status = (int)orderQueryParameters.Status.Value;

                query = query.Where(order => order.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(orderQueryParameters.SearchQuery))
            {
                var searchQuery = orderQueryParameters.SearchQuery.Trim();

                query = query.Where(order =>
                    order.User.Name.Contains(searchQuery) ||
                    order.AddressLine1.Contains(searchQuery) ||
                    order.AddressLine2.Contains(searchQuery) ||
                    order.City.Contains(searchQuery) ||
                    order.PostalCode.Contains(searchQuery) ||
                    order.Country.Contains(searchQuery));
            }

            var totalRecords = await query.CountAsync(cancellationToken)
                            .ConfigureAwait(false); ;

            switch (orderQueryParameters.SortBy)
            {
                case OrderSortBy.TotalPrice:
                    query = orderQueryParameters.IsDescending
                        ? query.OrderByDescending(order => order.TotalPrice)
                        : query.OrderBy(order => order.TotalPrice);
                    break;

                case OrderSortBy.CreatedAt:
                    query = orderQueryParameters.IsDescending
                        ? query.OrderByDescending(order => order.CreatedAt)
                        : query.OrderBy(order => order.CreatedAt);
                    break;

                case OrderSortBy.UpdatedAt:
                    query = orderQueryParameters.IsDescending
                        ? query.OrderByDescending(order => order.UpdatedAt)
                        : query.OrderBy(order => order.UpdatedAt);
                    break;

                case OrderSortBy.OrderId:
                    query = orderQueryParameters.IsDescending
                        ? query.OrderByDescending(order => order.Id)
                        : query.OrderBy(order => order.Id);
                    break;

                case OrderSortBy.Name:
                    query = orderQueryParameters.IsDescending
                        ? query.OrderByDescending(order => order.User.Name)
                        : query.OrderBy(order => order.User.Name);
                    break;

                default:
                    query = query.OrderByDescending(order => order.CreatedAt);
                    break;
            }

            var responses = await query
                .Skip((orderQueryParameters.PageNumber - 1) * orderQueryParameters.PageSize)
                .Take(orderQueryParameters.PageSize)
                .Select(order => new OrderResponse
                {
                    OrderId = order.Id,
                    RestaurantId = order.RestaurantId,
                    RestaurantName = order.Restaurant.RestaurantName,
                    Status = (order.Status).ToString(),
                    TotalPrice = order.TotalPrice,
                    CreatedAt = order.CreatedAt,
                    AddressLine1 = order.AddressLine1,
                    AddressLine2 = order.AddressLine2,
                    City = order.City,
                    PostalCode = order.PostalCode,
                    Country = order.Country
                })
                .ToListAsync(cancellationToken).ConfigureAwait(false); ;

            return (totalRecords, responses);
        }

    }
}
