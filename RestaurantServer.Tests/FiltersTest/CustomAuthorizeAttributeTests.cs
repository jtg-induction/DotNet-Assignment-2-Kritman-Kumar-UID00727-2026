using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestaurantServer.Enums;
using RestaurantServer.Filters;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;

namespace RestaurantServer.Tests.Filters
{
    [TestClass]
    public class CustomAuthorizeAttributeTests
    {
        [TestMethod]
        public void IsAuthorized_WithUnauthenticatedUser_ShouldReturnFalse()
        {
            var attribute =
                new TestableCustomAuthorizeAttribute(
                    UserRole.Admin);

            var context =
                CreateActionContext(
                    CreatePrincipal(false));

            var result =
                attribute.IsAuthorizedPublic(context);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsAuthorized_WithAuthenticatedUserAndCorrectRole_ShouldReturnTrue()
        {
            var attribute =
                new TestableCustomAuthorizeAttribute(
                    UserRole.Admin);

            var principal =
                CreatePrincipal(
                    true,
                    ((int)UserRole.Admin).ToString());

            var context =
                CreateActionContext(principal);

            var result =
                attribute.IsAuthorizedPublic(context);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsAuthorized_WithAuthenticatedUserAndWrongRole_ShouldReturnFalse()
        {
            var attribute =
                new TestableCustomAuthorizeAttribute(
                    UserRole.Admin);

            var principal =
                CreatePrincipal(
                    true,
                    ((int)UserRole.Customer).ToString());

            var context =
                CreateActionContext(principal);

            var result =
                attribute.IsAuthorizedPublic(context);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsAuthorized_WithMultipleAllowedRolesAndMatchingRole_ShouldReturnTrue()
        {
            var attribute =
                new TestableCustomAuthorizeAttribute(
                    UserRole.Admin,
                    UserRole.Owner);

            var principal =
                CreatePrincipal(
                    true,
                    ((int)UserRole.Owner).ToString());

            var context =
                CreateActionContext(principal);

            var result =
                attribute.IsAuthorizedPublic(context);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsAuthorized_WithNoAllowedRoles_ShouldReturnTrue()
        {
            var attribute =
                new TestableCustomAuthorizeAttribute();

            var principal =
                CreatePrincipal(true);

            var context =
                CreateActionContext(principal);

            var result =
                attribute.IsAuthorizedPublic(context);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsAuthorized_WithMissingPrincipal_ShouldReturnFalse()
        {
            var attribute =
                new TestableCustomAuthorizeAttribute(
                    UserRole.Admin);

            var context =
                CreateActionContext(null);

            var result =
                attribute.IsAuthorizedPublic(context);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void HandleUnauthorizedRequest_WithUnauthenticatedUser_ShouldReturn401()
        {
            var attribute =
                new TestableCustomAuthorizeAttribute(
                    UserRole.Admin);

            var context =
                CreateActionContext(
                    CreatePrincipal(false));

            attribute.HandleUnauthorizedRequestPublic(context);

            Assert.AreEqual(
                HttpStatusCode.Unauthorized,
                context.Response.StatusCode);
        }

        [TestMethod]
        public void HandleUnauthorizedRequest_WithAuthenticatedUser_ShouldReturn403()
        {
            var attribute =
                new TestableCustomAuthorizeAttribute(
                    UserRole.Admin);

            var principal =
                CreatePrincipal(
                    true,
                    ((int)UserRole.Customer).ToString());

            var context =
                CreateActionContext(principal);

            attribute.HandleUnauthorizedRequestPublic(context);

            Assert.AreEqual(
                HttpStatusCode.Forbidden,
                context.Response.StatusCode);
        }

        private static ClaimsPrincipal CreatePrincipal(
            bool isAuthenticated,
            string role = null)
        {
            var claims = new List<Claim>();

            if (role != null)
            {
                claims.Add(
                    new Claim(
                        ClaimTypes.Role,
                        role));
            }

            var identity =
                new ClaimsIdentity(
                    claims,
                    isAuthenticated
                        ? "TestAuthentication"
                        : null);

            return new ClaimsPrincipal(identity);
        }

        private static HttpActionContext CreateActionContext(
            ClaimsPrincipal principal)
        {
            var config =
                new HttpConfiguration();

            var request =
                new HttpRequestMessage(
                    HttpMethod.Get,
                    "http://localhost/test");

            request.Properties[
                HttpPropertyKeys.HttpConfigurationKey] =
                config;

            var controllerContext =
                new HttpControllerContext
                {
                    Configuration = config,
                    Request = request
                };

            var actionContext =
                new HttpActionContext
                {
                    ControllerContext = controllerContext
                };

            actionContext.RequestContext.Principal =
                principal;

            return actionContext;
        }

        private class TestableCustomAuthorizeAttribute
            : CustomAuthorizeAttribute
        {
            public TestableCustomAuthorizeAttribute(
                params UserRole[] roles)
                : base(roles)
            {
            }

            public bool IsAuthorizedPublic(
                HttpActionContext context)
            {
                return base.IsAuthorized(context);
            }

            public void HandleUnauthorizedRequestPublic(
                HttpActionContext context)
            {
                base.HandleUnauthorizedRequest(context);
            }
        }
    }
}

