using Microsoft.IdentityModel.Tokens;
using RestaurantServer.Helpers.Interfaces;
using RestaurantServer.Models;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RestaurantServer.Helpers.Implementations
{
    /// <summary>
    /// Provides functionality for generating JWT access tokens
    /// and refresh tokens for authenticated users.
    /// </summary>
    public class JwtTokenService : IJwtTokenService
    {
        /// <summary>
        /// Generates a signed JWT access token containing the user's
        /// identity, email, and role claims.
        /// </summary>
        /// <param name="user">
        /// The user for whom the access token is generated.
        /// </param>
        /// <returns>
        /// A signed JWT access token.
        /// </returns>
        public string GenerateAccessToken(User user)
        {
            var secretKey =
                ConfigurationManager.AppSettings["JwtSecretKey"];

            var issuer =
                ConfigurationManager.AppSettings["JwtIssuer"];

            var audience =
                ConfigurationManager.AppSettings["JwtAudience"];

            var expiryMinutes =
                int.Parse(
                    ConfigurationManager.AppSettings[
                        "JwtAccessTokenExpiryMinutes"
                    ]);

            var key =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secretKey)
                );

            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256
                );

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()
                ),

                new Claim(
                    ClaimTypes.Email,
                    user.Email
                ),

                new Claim(
                    ClaimTypes.Role,
                    user.Role.ToString()
                )
            };

            var token =
                new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(
                        expiryMinutes
                    ),
                    signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        /// <summary>
        /// Generates a unique refresh token for maintaining
        /// an authenticated session.
        /// </summary>
        /// <returns>
        /// A unique refresh token string.
        /// </returns>
        public string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
