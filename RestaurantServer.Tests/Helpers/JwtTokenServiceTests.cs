using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestaurantServer.Enums;
using RestaurantServer.Helpers.Implementations;
using RestaurantServer.Models;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace RestaurantServer.Tests.Helpers
{
    [TestClass]
    public class JwtTokenServiceTests
    {
        private JwtTokenService _jwtTokenService;

        [TestInitialize]
        public void Setup()
        {
            _jwtTokenService = new JwtTokenService();
        }

        [TestMethod]
        public void GenerateAccessToken_ShouldReturnToken()
        {
            var user = CreateUser();

            var token = _jwtTokenService.GenerateAccessToken(user);

            Assert.IsFalse(string.IsNullOrWhiteSpace(token));
        }

        [TestMethod]
        public void GenerateAccessToken_ShouldContainUserIdClaim()
        {
            var user = CreateUser();

            var token = _jwtTokenService.GenerateAccessToken(user);

            var jwtToken = ReadToken(token);

            var claim = jwtToken.Claims
                .FirstOrDefault(c =>
                    c.Type == ClaimTypes.NameIdentifier);

            Assert.IsNotNull(claim);
            Assert.AreEqual(user.Id.ToString(), claim.Value);
        }

        [TestMethod]
        public void GenerateAccessToken_ShouldContainEmailClaim()
        {
            var user = CreateUser();

            var token = _jwtTokenService.GenerateAccessToken(user);

            var jwtToken = ReadToken(token);

            var claim = jwtToken.Claims
                .FirstOrDefault(c =>
                    c.Type == ClaimTypes.Email);

            Assert.IsNotNull(claim);
            Assert.AreEqual(user.Email, claim.Value);
        }

        [TestMethod]
        public void GenerateAccessToken_ShouldContainRoleClaim()
        {
            var user = CreateUser();

            var token = _jwtTokenService.GenerateAccessToken(user);

            var jwtToken = ReadToken(token);

            var claim = jwtToken.Claims
                .FirstOrDefault(c =>
                    c.Type == ClaimTypes.Role);

            Assert.IsNotNull(claim);

            Assert.AreEqual(
                user.Role.ToString(),
                claim.Value);
        }

        [TestMethod]
        public void GenerateAccessToken_ShouldUseConfiguredIssuerAndAudience()
        {
            var user = CreateUser();

            var token = _jwtTokenService.GenerateAccessToken(user);

            var jwtToken = ReadToken(token);

            var expectedIssuer =
                ConfigurationManager.AppSettings["JwtIssuer"];

            var expectedAudience =
                ConfigurationManager.AppSettings["JwtAudience"];

            Assert.AreEqual(expectedIssuer, jwtToken.Issuer);
            Assert.IsTrue(
                jwtToken.Audiences.Contains(expectedAudience));
        }

        [TestMethod]
        public void GenerateAccessToken_ShouldHaveConfiguredExpiration()
        {
            var user = CreateUser();

            var beforeGeneration = DateTime.UtcNow;

            var token =
                _jwtTokenService.GenerateAccessToken(user);

            var afterGeneration = DateTime.UtcNow;

            var jwtToken = ReadToken(token);

            var expiryMinutes = int.Parse(
                ConfigurationManager.AppSettings[
                    "JwtAccessTokenExpiryMinutes"]);

            var minimumExpectedExpiration =
                beforeGeneration.AddMinutes(expiryMinutes);

            var maximumExpectedExpiration =
                afterGeneration.AddMinutes(expiryMinutes);

            Assert.IsTrue(
                jwtToken.ValidTo >=
                minimumExpectedExpiration.AddSeconds(-1));

            Assert.IsTrue(
                jwtToken.ValidTo <=
                maximumExpectedExpiration.AddSeconds(1));
        }

        [TestMethod]
        public void GenerateAccessToken_ShouldHaveValidSignature()
        {
            var user = CreateUser();

            var token =
                _jwtTokenService.GenerateAccessToken(user);

            var secretKey =
                ConfigurationManager.AppSettings["JwtSecretKey"];

            var issuer =
                ConfigurationManager.AppSettings["JwtIssuer"];

            var audience =
                ConfigurationManager.AppSettings["JwtAudience"];

            var validationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,

                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(secretKey)),

                    ValidateIssuer = true,
                    ValidIssuer = issuer,

                    ValidateAudience = true,
                    ValidAudience = audience,

                    ValidateLifetime = true,

                    ClockSkew = TimeSpan.FromSeconds(5)
                };

            var handler = new JwtSecurityTokenHandler();

            var principal =
                handler.ValidateToken(
                    token,
                    validationParameters,
                    out _);

            Assert.IsNotNull(principal);
            Assert.IsTrue(principal.Identity.IsAuthenticated);
        }

        [TestMethod]
        public void GenerateRefreshToken_ShouldReturnUniqueToken()
        {
            var firstToken =
                _jwtTokenService.GenerateRefreshToken();

            var secondToken =
                _jwtTokenService.GenerateRefreshToken();

            Assert.IsFalse(
                string.IsNullOrWhiteSpace(firstToken));

            Assert.IsFalse(
                string.IsNullOrWhiteSpace(secondToken));

            Assert.AreNotEqual(firstToken, secondToken);
        }

        private static User CreateUser()
        {
            return new User
            {
                Id = 101,
                Name = "Test User",
                Email = "test@example.com",
                Role = (int)UserRole.Admin,
                IsActive = true
            };
        }

        private static JwtSecurityToken ReadToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            return handler.ReadJwtToken(token);
        }
    }
}
