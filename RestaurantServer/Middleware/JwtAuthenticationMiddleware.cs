using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantServer.Middleware
{
    public class JwtAuthenticationMiddleware : OwinMiddleware
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="JwtAuthenticationMiddleware"/> class.
        /// </summary>
        /// <param name="next">
        /// The next middleware component in the OWIN pipeline.
        /// </param>
        public JwtAuthenticationMiddleware(OwinMiddleware next)
            : base(next)
        {

        }

        /// <summary>
        /// Validates the JWT bearer token from the Authorization header
        /// and sets the authenticated user principal for the current request.
        /// </summary>
        /// <param name="context">
        /// The OWIN context for the current HTTP request.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous middleware operation.
        /// </returns>
        public override async Task Invoke(IOwinContext context)
        {
            var authorizationHeader =
                context.Request.Headers["Authorization"];

            if (!string.IsNullOrWhiteSpace(authorizationHeader) &&
                authorizationHeader.StartsWith(
                    "Bearer ",
                    StringComparison.OrdinalIgnoreCase))
            {
                var token = authorizationHeader.Substring(7).Trim();

                try
                {
                    var secretKey =
                        ConfigurationManager.AppSettings["JwtSecretKey"];

                    var issuer =
                        ConfigurationManager.AppSettings["JwtIssuer"];

                    var audience =
                        ConfigurationManager.AppSettings["JwtAudience"];

                    var tokenHandler = new JwtSecurityTokenHandler();

                    var validationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(secretKey)
                                ),
                            ValidateIssuer = true,
                            ValidIssuer = issuer,
                            ValidateAudience = true,
                            ValidAudience = audience,
                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero
                        };

                    SecurityToken validatedToken;

                    ClaimsPrincipal principal =
                        tokenHandler.ValidateToken(
                            token,
                            validationParameters,
                            out validatedToken
                        );

                    context.Request.User = principal;
                    context.Environment["server.User"] = principal;
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
