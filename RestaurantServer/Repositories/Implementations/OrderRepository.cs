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

        public async Task<Order> GetOrderWithItemsByIdAsync(long orderId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(order => order.OrderItems)
                .FirstOrDefaultAsync(order => order.Id == orderId, cancellationToken);
        }

        public async Task<Order> GetOrderForUpdateAsync(long orderId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .SqlQuery("SELECT * FROM Orders WITH (UPDLOCK, ROWLOCK) WHERE Id = @p0", orderId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the orders matching the specified filters, sorting, and pagination settings.
        /// </summary>
        /// <param name="ownerId">The ID of the restaurant owner.</param>
        /// <param name="orderQueryParameters">The filters, sorting, and pagination parameters.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The total number of matching orders and the orders for the requested page.</returns>
        public async Task<(int TotalRecords, List<OrderResponse> Orders)> GetFilteredOrders(
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
                        Id = order.Id,
                        CustomerId = order.UserId,
                        CustomerName = order.User.Name,
                        RestaurantId = order.RestaurantId,
                        RestaurantName = order.Restaurant.RestaurantName,
                        Status = order.Status,
                        TotalPrice = order.TotalPrice,
                        CreatedAt = order.CreatedAt,
                        AddressLine1 = order.AddressLine1,
                        AddressLine2 = order.AddressLine2,
                        City = order.City,
                        PostalCode = order.PostalCode,
                        Country = order.Country
                    })
                    .FirstOrDefaultAsync(cancellationToken);

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

            var totalRecords = await query.CountAsync(cancellationToken);

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
                    Id = order.Id,
                    CustomerId = order.UserId,
                    CustomerName = order.User.Name,
                    RestaurantId = order.RestaurantId,
                    RestaurantName = order.Restaurant.RestaurantName,
                    Status = order.Status,
                    TotalPrice = order.TotalPrice,
                    CreatedAt = order.CreatedAt,
                    AddressLine1 = order.AddressLine1,
                    AddressLine2 = order.AddressLine2,
                    City = order.City,
                    PostalCode = order.PostalCode,
                    Country = order.Country
                })
                .ToListAsync(cancellationToken);

            return (totalRecords, responses);
        }

    }
}
