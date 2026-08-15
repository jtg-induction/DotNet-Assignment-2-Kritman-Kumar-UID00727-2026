using System.Net.Http;
using System.Net.Http.Headers;

namespace RestaurantServer.Helpers.Interfaces
{
    public interface IRefreshTokenHelper
    {
        string GetRefreshTokenFromRequest(HttpRequestMessage request, string refreshTokenCookieName);
    }
}
