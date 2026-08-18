using RestaurantServer.Models;
using System.Threading;
using System.Threading.Tasks;


namespace RestaurantServer.Repositories.Interfaces
{
    public interface IRestaurantOwnerRepository
    {
         Task<RestaurantOwner> GetAsync(long restaurantId,long userId,CancellationToken cancellationToken = default);

         Task AddAsync(RestaurantOwner restaurantOwner, CancellationToken cancellationToken = default);
    }
}
