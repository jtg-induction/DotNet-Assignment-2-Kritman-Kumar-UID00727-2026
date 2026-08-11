using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using System.Threading.Tasks;

public interface IAuthService
{
    Task<SignupResponse> SignupAsync(SignupRequest request);
    Task<LoginResult> LoginAsync(LoginRequest request);
    Task<LoginResult> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(string refreshToken);
}
