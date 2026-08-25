using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using System.Threading;
using System.Threading.Tasks;

public interface IAuthService
{
    Task<SignupResponse> SignupAsync(SignupRequest request, CancellationToken cancellationToken = default);
    Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<RefreshResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);
}
