using RestaurantServer.Models;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Validators.Interfaces
{
    public interface IRestaurantValidator
    {
        void ValidateRestaurantExists(Restaurant restaurant);
        void ValidateOwnerRelationshipDoesNotExist(RestaurantOwner restaurantOwner);
        Task ValidateMobileNumber(string mobileNumber, CancellationToken cancellationToken = default); 
        void ValidateAdminRole(int role); 
    }
}
