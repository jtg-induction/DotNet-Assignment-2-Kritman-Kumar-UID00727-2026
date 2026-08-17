using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestaurantServer.Middleware;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantServer.Tests.Middleware
{
    [TestClass]
    public class JwtAuthenticationMiddlewareTests
    {
        [TestMethod]
        public async Task Invoke_WithoutAuthorizationHeader_ShouldContinuePipeline()
        {
            var nextMiddleware = new TestNextMiddleware();
            var middleware =
                new JwtAuthenticationMiddleware(nextMiddleware);

            var context = CreateContext();

            await middleware.Invoke(context);

            Assert.IsTrue(nextMiddleware.WasInvoked);
        }

        [TestMethod]
        public async Task Invoke_WithNonBearerAuthorizationHeader_ShouldContinuePipeline()
        {
            var nextMiddleware = new TestNextMiddleware();
            var middleware =
                new JwtAuthenticationMiddleware(nextMiddleware);

            var context = CreateContext();

            context.Request.Headers["Authorization"] =
                "Basic some-token";

            await middleware.Invoke(context);

            Assert.IsTrue(nextMiddleware.WasInvoked);
        }

        [TestMethod]
        public async Task Invoke_WithInvalidToken_ShouldReturnUnauthorized()
        {
            var nextMiddleware = new TestNextMiddleware();
            var middleware =
                new JwtAuthenticationMiddleware(nextMiddleware);

            var context = CreateContext();

            context.Request.Headers["Authorization"] =
                "Bearer invalid-token";

            await middleware.Invoke(context);

            Assert.AreEqual(401, context.Response.StatusCode);
            Assert.IsFalse(nextMiddleware.WasInvoked);
        }

        [TestMethod]
        public async Task Invoke_WithExpiredToken_ShouldReturnUnauthorized()
        {
            var nextMiddleware = new TestNextMiddleware();
            var middleware =
                new JwtAuthenticationMiddleware(nextMiddleware);

            var context = CreateContext();

            var expiredToken =
                CreateToken(DateTime.UtcNow.AddMinutes(-10));

            context.Request.Headers["Authorization"] =
                "Bearer " + expiredToken;

            await middleware.Invoke(context);

            Assert.AreEqual(401, context.Response.StatusCode);
            Assert.IsFalse(nextMiddleware.WasInvoked);
        }

        [TestMethod]
        public async Task Invoke_WithValidToken_ShouldSetAuthenticatedUser()
        {
            var nextMiddleware = new TestNextMiddleware();
            var middleware =
                new JwtAuthenticationMiddleware(nextMiddleware);

            var context = CreateContext();

            var token =
                CreateToken(DateTime.UtcNow.AddMinutes(10));

            context.Request.Headers["Authorization"] =
                "Bearer " + token;

            await middleware.Invoke(context);

            Assert.IsTrue(nextMiddleware.WasInvoked);

            Assert.IsNotNull(context.Request.User);
            Assert.IsNotNull(context.Request.User.Identity);
            Assert.IsTrue(
                context.Request.User.Identity.IsAuthenticated);

            Assert.IsNotNull(
                context.Environment["server.User"]);
        }

        private static OwinContext CreateContext()
        {
            var environment =
                new Dictionary<string, object>
                {
                    {
                        "owin.RequestHeaders",
                        new Dictionary<string, string[]>()
                    },
                    {
                        "owin.ResponseHeaders",
                        new Dictionary<string, string[]>()
                    },
                    {
                        "owin.ResponseStatusCode",
                        200
                    }
                };

            return new OwinContext(environment);
        }

        private static string CreateToken(DateTime expiration)
        {
            var secretKey =
                ConfigurationManager.AppSettings["JwtSecretKey"];

            var issuer =
                ConfigurationManager.AppSettings["JwtIssuer"];

            var audience =
                ConfigurationManager.AppSettings["JwtAudience"];

            var securityKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secretKey));

            var credentials =
                new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    "101"),

                new Claim(
                    ClaimTypes.Email,
                    "test@example.com"),

                new Claim(
                    ClaimTypes.Role,
                    "3")
            };

            var notBefore =
                expiration.AddMinutes(-10);

            var token =
                new JwtSecurityToken(
                    issuer,
                    audience,
                    claims,
                    notBefore,
                    expiration,
                    credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        private class TestNextMiddleware : OwinMiddleware
        {
            public bool WasInvoked { get; private set; }

            public TestNextMiddleware()
                : base(null)
            {
            }

            public override Task Invoke(IOwinContext context)
            {
                WasInvoked = true;

                return Task.CompletedTask;
            }
        }
    }
}
