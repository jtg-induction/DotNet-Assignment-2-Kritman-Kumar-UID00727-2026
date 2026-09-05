using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Implementations
{
    public class RestaurantRepository : Repository<Restaurant>, IRestaurantRepository
    {
        public RestaurantRepository(ApplicationDbContext context)
            : base(context)
        {

        }

        /// <summary>
        /// Asynchronously checks whether a non-deleted restaurant with the specified mobile number exists.
        /// </summary>
        /// <param name="mobileNumber">
        /// The mobile number to search for.
        /// </param>
        /// <param name="cancellationToken">
        /// A token to observe while waiting for the asynchronous operation to complete.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// <see langword="true"/> if a matching non-deleted restaurant exists;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public async Task<bool> ExistsByMobileNumberAsync(string mobileNumber, CancellationToken cancellationToken = default)
        {
            IQueryable<Restaurant> query = _context.Restaurants;

            return await query.AnyAsync(
                restaurant => restaurant.MobileNumber == mobileNumber &&
                              !restaurant.IsDeleted, cancellationToken);
        }

        /// <summary>
        /// Asynchronously retrieves a paginated list of non-deleted restaurants.
        /// </summary>
        /// <param name="page">
        /// The one-based page number to retrieve.
        /// </param>
        /// <param name="pageSize">
        /// The maximum number of restaurants to return per page.
        /// </param>
        /// <param name="disableTracking">
        /// A value indicating whether Entity Framework tracking should be disabled.
        /// When set to <see langword="true"/>, <see cref="DbExtensions.AsNoTracking{T}(System.Linq.IQueryable{T})" />
        /// is applied to the query. This is recommended for read-only operations when
        /// the returned entities do not need to be tracked for updates.
        /// </param>
        /// <param name="cancellationToken">
        /// A token to observe while waiting for the asynchronous operation to complete.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// a list of non-deleted restaurants for the requested page.
        /// </returns>
        public async Task<List<Restaurant>> GetAvailableRestaurantsAsync(int page, int pageSize,
            bool disableTracking = false, CancellationToken cancellationToken = default)
        {
            IQueryable<Restaurant> query = _context.Restaurants
                .Where(restaurant => !restaurant.IsDeleted);

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            return await query
                .OrderBy(restaurant => restaurant.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Asynchronously counts the total number of non-deleted restaurants.
        /// </summary>
        /// <param name="disableTracking">
        /// A value indicating whether Entity Framework tracking should be disabled.
        /// This parameter has no practical effect for this count operation because
        /// <param name="cancellationToken">
        /// A token to observe while waiting for the asynchronous operation to complete.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// the total number of non-deleted restaurants.
        /// </returns>
        public async Task<int> CountAvailableRestaurantsAsync(
            CancellationToken cancellationToken = default)
        {
            IQueryable<Restaurant> query = _context.Restaurants;

            return await query.CountAsync(
                restaurant => !restaurant.IsDeleted,
                cancellationToken);
        }

    }
}
