using RestaurantServer.Models;
using System.Threading;
using System.Threading.Tasks;


namespace RestaurantServer.Repositories.Interfaces
{
    public interface IRestaurantOwnerRepository: IRepository<RestaurantOwner>
    {
         Task<RestaurantOwner> GetOwnerWithRestaurantIdAsync(long restaurantId,long userId,CancellationToken cancellationToken = default);
    }
}
