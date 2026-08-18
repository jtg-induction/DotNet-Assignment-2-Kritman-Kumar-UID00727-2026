using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace RestaurantServer.Middleware
{
    public class JwtAuthenticationMiddleware : OwinMiddleware
    {
        public JwtAuthenticationMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            var authorizationHeader =
                context.Request.Headers["Authorization"];

            if (!string.IsNullOrWhiteSpace(authorizationHeader) &&
                authorizationHeader.StartsWith(
                    "Bearer ",
                    StringComparison.OrdinalIgnoreCase))
            {
                var token =
                    authorizationHeader.Substring(7).Trim();

                try
                {
                    var secretKey =
                        ConfigurationManager.AppSettings[
                            "JwtSecretKey"];

                    var issuer =
                        ConfigurationManager.AppSettings[
                            "JwtIssuer"];

                    var audience =
                        ConfigurationManager.AppSettings[
                            "JwtAudience"];

                    if (string.IsNullOrWhiteSpace(secretKey) ||
                        string.IsNullOrWhiteSpace(issuer) ||
                        string.IsNullOrWhiteSpace(audience))
                    {
                        context.Response.StatusCode =
                            (int)HttpStatusCode.Unauthorized;

                        return;
                    }

                    var tokenHandler =
                        new JwtSecurityTokenHandler();

                    var validationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                            ValidateIssuer = true,
                            ValidIssuer = issuer,
                            ValidateAudience = true,
                            ValidAudience = audience,
                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero
                        };

                    SecurityToken validatedToken;

                    ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                    context.Request.User = principal;

                    context.Environment["server.User"] = principal;

                    if (HttpContext.Current != null)
                    {
                        HttpContext.Current.User = principal;
                    }

                    Thread.CurrentPrincipal = principal;
                }
                catch (SecurityTokenException)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                    return;
                }
                catch (ArgumentException)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                    return;
                }
            }

            await Next.Invoke(context);
        }
    }
}
