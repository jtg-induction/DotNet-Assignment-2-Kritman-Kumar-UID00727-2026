using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestaurantServer.Exceptions;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;

namespace RestaurantServer.Tests.Exceptions
{
    [TestClass]
    public class ErrorMessageResultTests
    {
        [TestMethod]
        public async Task ExecuteAsync_ShouldReturnConfiguredStatusCode()
        {
            var request = CreateRequest();

            var result =
                new ErrorMessageResult(
                    request,
                    HttpStatusCode.BadRequest,
                    new
                    {
                        Message = "Validation failed."
                    });

            var response =
                await result.ExecuteAsync(
                    CancellationToken.None);

            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                response.StatusCode);
        }

        [TestMethod]
        public async Task ExecuteAsync_ShouldReturnConfiguredContent()
        {
            var request = CreateRequest();

            var result =
                new ErrorMessageResult(
                    request,
                    HttpStatusCode.BadRequest,
                    new
                    {
                        Message = "Validation failed."
                    });

            var response =
                await result.ExecuteAsync(
                    CancellationToken.None);

            var content =
                await response.Content.ReadAsStringAsync();

            Assert.IsTrue(
                content.Contains(
                    "Validation failed."));
        }

        [TestMethod]
        public async Task ExecuteAsync_WithInternalServerError_ShouldReturn500()
        {
            var request = CreateRequest();

            var result =
                new ErrorMessageResult(
                    request,
                    HttpStatusCode.InternalServerError,
                    new
                    {
                        Message =
                            "An unexpected error occurred."
                    });

            var response =
                await result.ExecuteAsync(
                    CancellationToken.None);

            Assert.AreEqual(
                HttpStatusCode.InternalServerError,
                response.StatusCode);
        }

        [TestMethod]
        public async Task ExecuteAsync_ShouldSerializeMultipleContentProperties()
        {
            var request = CreateRequest();

            var result =
                new ErrorMessageResult(
                    request,
                    HttpStatusCode.BadRequest,
                    new
                    {
                        Message = "Validation failed.",
                        Errors = new Dictionary<string, string[]>
                        {
                            {
                                "Email",
                                new[]
                                {
                                    "Email is required."
                                }
                            },
                            {
                                "Password",
                                new[]
                                {
                                    "Password is required."
                                }
                            }
                        }
                    });

            var response =
                await result.ExecuteAsync(
                    CancellationToken.None);

            var content =
                await response.Content.ReadAsStringAsync();

            Assert.IsTrue(
                content.Contains("Validation failed."));

            Assert.IsTrue(
                content.Contains("Email"));

            Assert.IsTrue(
                content.Contains("Email is required."));

            Assert.IsTrue(
                content.Contains("Password"));

            Assert.IsTrue(
                content.Contains("Password is required."));
        }

        [TestMethod]
        public async Task ExecuteAsync_ShouldReturnJsonContentType()
        {
            var request = CreateRequest();

            var result =
                new ErrorMessageResult(
                    request,
                    HttpStatusCode.BadRequest,
                    new
                    {
                        Message = "Validation failed."
                    });

            var response =
                await result.ExecuteAsync(
                    CancellationToken.None);

            Assert.IsNotNull(response.Content);
            Assert.IsNotNull(
                response.Content.Headers.ContentType);

            Assert.AreEqual(
                "application/json",
                response.Content.Headers.ContentType.MediaType);
        }

        [TestMethod]
        public async Task ExecuteAsync_ShouldReturnSuccessfulTask()
        {
            var request = CreateRequest();

            var result =
                new ErrorMessageResult(
                    request,
                    HttpStatusCode.BadRequest,
                    new
                    {
                        Message = "Validation failed."
                    });

            var task =
                result.ExecuteAsync(
                    CancellationToken.None);

            Assert.IsNotNull(task);
            Assert.IsTrue(task.IsCompleted);

            var response =
                await task;

            Assert.IsNotNull(response);
        }

        private static HttpRequestMessage CreateRequest()
        {
            var configuration =
                new HttpConfiguration();

            var request =
                new HttpRequestMessage(
                    HttpMethod.Get,
                    "http://localhost/test");

            request.Properties[
                HttpPropertyKeys.HttpConfigurationKey] =
                configuration;

            return request;
        }
    }
}
