using RestaurantServer.Constants;
using RestaurantServer.Exceptions;
using RestaurantServer.Helpers.Interfaces;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers; 

namespace RestaurantServer.Helpers
{
    public class RefreshTokenHelper : IRefreshTokenHelper
    {

        /// <summary>
        /// Retrieves the refresh token from the request cookie.
        /// </summary>
        /// <param name="request">
        /// The HTTP request containing the refresh token cookie.
        /// </param>
        /// <param name="refreshTokenCookieName">
        /// string refresh Token Cookie Name.
        /// </param>
        /// <returns>
        /// The refresh token stored in the request cookie.
        /// </returns>
        /// <exception cref="ValidationException">
        /// Thrown when the refresh token cookie is missing or invalid.
        /// </exception>
        public string GetRefreshTokenFromRequest(HttpRequestMessage request, string refreshTokenCookieName)
        {
            var cookie = request.Headers
                .GetCookies(refreshTokenCookieName)
                .FirstOrDefault();

            if (cookie == null)
            {
                throw new ValidationException(
                    ErrorMessages.InvalidRefreshToken);
            }

            var refreshToken =
                cookie[refreshTokenCookieName]?.Value;

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ValidationException(
                    ErrorMessages.InvalidRefreshToken);
            }

            return refreshToken;
        }
    }
}
