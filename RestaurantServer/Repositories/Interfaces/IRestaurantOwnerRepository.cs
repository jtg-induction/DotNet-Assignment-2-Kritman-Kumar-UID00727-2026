using RestaurantServer.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace RestaurantServer.Repositories.Interfaces
{
    public interface IRestaurantOwnerRepository : IRepository<RestaurantOwner>
    {
        Task<List<RestaurantOwner>> GetOwnersByRestaurantAndUserIdsAsync(
            long restaurantId, IEnumerable<long> userIds, CancellationToken cancellationToken = default);
    }
}
