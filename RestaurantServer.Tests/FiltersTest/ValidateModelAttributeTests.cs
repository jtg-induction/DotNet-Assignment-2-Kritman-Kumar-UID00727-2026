using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestaurantServer.Constants;
using RestaurantServer.Filters;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;

namespace RestaurantServer.Tests.FiltersTest
{
    [TestClass]
    public class ValidateModelAttributeTests
    {
        [TestMethod]
        public void OnActionExecuting_WithValidModelState_ShouldNotCreateResponse()
        {
            var attribute =
                new ValidateModelAttribute();

            var context =
                CreateActionContext();

            attribute.OnActionExecuting(context);

            Assert.IsNull(context.Response);
        }

        [TestMethod]
        public void OnActionExecuting_WithInvalidModelState_ShouldReturnBadRequest()
        {
            var attribute =
                new ValidateModelAttribute();

            var context =
                CreateActionContext();

            context.ModelState.AddModelError(
                "Email",
                "Email is required.");

            attribute.OnActionExecuting(context);

            Assert.IsNotNull(context.Response);
            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                context.Response.StatusCode);
        }

        [TestMethod]
        public async Task OnActionExecuting_WithInvalidModelState_ShouldReturnValidationFailedMessage()
        {
            var attribute =
                new ValidateModelAttribute();

            var context =
                CreateActionContext();

            context.ModelState.AddModelError(
                "Email",
                "Email is required.");

            attribute.OnActionExecuting(context);

            Assert.IsNotNull(context.Response);

            var responseContent =
                await context.Response.Content.ReadAsStringAsync();

            Assert.IsTrue(
                responseContent.Contains(
                    ErrorMessages.ValidationFailed));
        }

        [TestMethod]
        public async Task OnActionExecuting_WithMultipleValidationErrors_ShouldReturnAllErrors()
        {
            var attribute =
                new ValidateModelAttribute();

            var context =
                CreateActionContext();

            context.ModelState.AddModelError(
                "Email",
                "Email is required.");

            context.ModelState.AddModelError(
                "Password",
                "Password is required.");

            context.ModelState.AddModelError(
                "Name",
                "Name is required.");

            attribute.OnActionExecuting(context);

            Assert.IsNotNull(context.Response);

            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                context.Response.StatusCode);

            var responseContent =
                await context.Response.Content.ReadAsStringAsync();

            Assert.IsTrue(
                responseContent.Contains("Email"));

            Assert.IsTrue(
                responseContent.Contains("Email is required."));

            Assert.IsTrue(
                responseContent.Contains("Password"));

            Assert.IsTrue(
                responseContent.Contains("Password is required."));

            Assert.IsTrue(
                responseContent.Contains("Name"));

            Assert.IsTrue(
                responseContent.Contains("Name is required."));
        }

        [TestMethod]
        public async Task OnActionExecuting_WithModelStateErrorWithoutMessage_ShouldIncludeEmptyErrorMessage()
        {
            var attribute =
                new ValidateModelAttribute();

            var context =
                CreateActionContext();

            context.ModelState.AddModelError(
                "Email",
                new System.Exception());

            attribute.OnActionExecuting(context);

            Assert.IsNotNull(context.Response);

            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                context.Response.StatusCode);

            var responseContent =
                await context.Response.Content.ReadAsStringAsync();

            Assert.IsTrue(
                responseContent.Contains("Email"));
        }

        private static HttpActionContext CreateActionContext()
        {
            var configuration =
                new HttpConfiguration();

            var request =
                new HttpRequestMessage(
                    HttpMethod.Post,
                    "http://localhost/test");

            request.Properties[
                HttpPropertyKeys.HttpConfigurationKey] =
                configuration;

            var controllerContext =
                new HttpControllerContext
                {
                    Configuration = configuration,
                    Request = request
                };

            return new HttpActionContext
            {
                ControllerContext = controllerContext
            };
        }
    }
}
