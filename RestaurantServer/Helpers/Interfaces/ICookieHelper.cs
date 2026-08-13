using System.Net.Http;
using System.Net.Http.Headers;

namespace RestaurantServer.Helpers.Interfaces
{
    public interface ICookieHelper
    {
        CookieHeaderValue CreateRefreshTokenCookie(string refreshToken);
        CookieHeaderValue CreateExpiredRefreshTokenCookie();
        string GetRefreshTokenFromRequest(HttpRequestMessage request);
    }
}
