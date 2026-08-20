using RestaurantServer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantServer.Validators.Interfaces
{
    public interface IRestaurantValidator
    {
        void ValidateRestaurantExists(Restaurant restaurant);
        void ValidateOwnerRelationshipDoesNotExist(RestaurantOwner restaurantOwner);
        Task ValidateMobileNumber(string mobileNumber);
        void ValidateEmail(string email);
        void ValidateAdminRole(int role);
        void IsOwnersEmailEmpty(List<string> emails);
    }
}
