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
    public class JwtTokenService : IJwtTokenService
    {
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

        public string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
