using System.Configuration;

namespace RestaurantServer.DTOs.Responses
{
    public class RefreshResponse
    {
        public RefreshResponse(string accessToken, string tokenType)
        {
            var expiryMinutes = int.Parse(ConfigurationManager.AppSettings["JwtAccessTokenExpiryMinutes"]);

            AccessToken = accessToken;
            TokenType = tokenType;
            ExpiresInSeconds = expiryMinutes*60;
        }

        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public int ExpiresInSeconds { get; set; }
    }
}
