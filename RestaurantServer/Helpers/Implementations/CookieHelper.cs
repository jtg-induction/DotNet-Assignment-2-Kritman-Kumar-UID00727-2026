using RestaurantServer.Constants;
using RestaurantServer.Exceptions;
using RestaurantServer.Helpers.Interfaces;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace RestaurantServer.Helpers
{
    /// <summary>
    /// Provides operations for managing authentication cookies.
    /// </summary>
    public class CookieHelper : ICookieHelper
    {
        private const string RefreshTokenCookieName = "refreshToken";
        private const string AuthPath = "/auth";

        /// <summary>
        /// Creates a secure HTTP-only cookie containing the refresh token.
        /// </summary>
        /// <param name="refreshToken">
        /// The refresh token to store in the cookie.
        /// </param>
        /// <returns>
        /// A configured refresh token cookie.
        /// </returns>
        public CookieHeaderValue CreateRefreshTokenCookie(
            string refreshToken)
        {
            var cookie = new CookieHeaderValue(
                RefreshTokenCookieName,
                refreshToken);

            cookie.HttpOnly = true;
            cookie.Secure = true;
            cookie.Path = AuthPath;

            return cookie;
        }

        /// <summary>
        /// Creates an expired refresh token cookie to remove the
        /// refresh token from the client.
        /// </summary>
        /// <returns>
        /// An expired refresh token cookie.
        /// </returns>
        public CookieHeaderValue CreateExpiredRefreshTokenCookie()
        {
            var cookie = CreateRefreshTokenCookie(string.Empty);

            cookie.Expires = DateTimeOffset.UtcNow.AddDays(-1);

            return cookie;
        }

        /// <summary>
        /// Retrieves the refresh token from the request cookie.
        /// </summary>
        /// <param name="request">
        /// The HTTP request containing the refresh token cookie.
        /// </param>
        /// <returns>
        /// The refresh token stored in the request cookie.
        /// </returns>
        /// <exception cref="ValidationException">
        /// Thrown when the refresh token cookie is missing or invalid.
        /// </exception>
        public string GetRefreshTokenFromRequest(
            HttpRequestMessage request)
        {
            var cookie = request.Headers
                .GetCookies(RefreshTokenCookieName)
                .FirstOrDefault();

            if (cookie == null)
            {
                throw new ValidationException(
                    ValidationMessages.InvalidRefreshToken);
            }

            var refreshToken =
                cookie[RefreshTokenCookieName]?.Value;

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ValidationException(
                    ValidationMessages.InvalidRefreshToken);
            }

            return refreshToken;
        }
    }
}
