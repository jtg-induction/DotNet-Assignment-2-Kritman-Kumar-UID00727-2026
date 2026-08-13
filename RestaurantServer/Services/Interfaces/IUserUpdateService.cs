using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using System.Threading.Tasks;

namespace RestaurantServer.Services.Interfaces
{
    public interface IUserUpdateService
    {
        Task<UpdateUserResponse> UpdateAccountAsync(long userId,  UpdateAccountRequest request);
        Task<string> DeactivateAccountAsync(long userId);
    }
}
