using System.Net.Http.Headers; 

namespace RestaurantServer.Helpers.Interfaces
{
    public interface ICookieHelper
    {
        CookieHeaderValue CreateHttpOnlySecureCookie(string cookieName, string cookieValue, string cookiePath);
    }
}
