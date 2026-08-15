using RestaurantServer.Helpers.Interfaces;
using System.Net.Http.Headers;

namespace RestaurantServer.Helpers
{
    public class CookieHelper:ICookieHelper
    {
        /// <summary>
        /// Creates a secure HTTP-only cookie containing the cookie name, cookie Value, cookie path.
        /// </summary>
        /// <param name="cookieName">
        /// Name of cookie.
        /// </param>
        /// <param name="cookieValue">
        /// value of the cookie.
        /// </param>
        /// <param name="cookiePath">
        /// path name where cookie set.
        /// </param>
        /// <returns>
        /// A configured cookie.
        /// </returns>
        public CookieHeaderValue CreateHttpOnlySecureCookie(string cookieName,string cookieValue, string cookiePath)
        {
            var cookie = new CookieHeaderValue(
                cookieName,
                cookieValue);

            cookie.HttpOnly = true;
            cookie.Secure = true;
            cookie.Path = cookiePath;

            return cookie;
        }
    }
}
