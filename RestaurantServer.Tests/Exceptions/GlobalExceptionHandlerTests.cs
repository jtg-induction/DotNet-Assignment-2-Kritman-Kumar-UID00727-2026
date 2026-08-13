using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestaurantServer.Constants;
using RestaurantServer.Exceptions;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Hosting;

namespace RestaurantServer.Tests.Exceptions
{
    [TestClass]
    public class GlobalExceptionHandlerTests
    {
        [TestMethod]
        public async Task Handle_WithValidationException_ShouldReturnBadRequest()
        {
            var expectedMessage =
                "Validation failed for the request.";

            var exception =
                new ValidationException(expectedMessage);

            var context =
                CreateExceptionHandlerContext(exception);

            var handler =
                new GlobalExceptionHandler();

            handler.Handle(context);

            Assert.IsNotNull(context.Result);

            var response =
                await context.Result.ExecuteAsync(
                    CancellationToken.None);

            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                response.StatusCode);
        }

        [TestMethod]
        public async Task Handle_WithValidationException_ShouldReturnExceptionMessage()
        {
            var expectedMessage =
                "Email already exists.";

            var exception =
                new ValidationException(expectedMessage);

            var context =
                CreateExceptionHandlerContext(exception);

            var handler =
                new GlobalExceptionHandler();

            handler.Handle(context);

            Assert.IsNotNull(context.Result);

            var response =
                await context.Result.ExecuteAsync(
                    CancellationToken.None);

            var content =
                await response.Content.ReadAsStringAsync();

            Assert.IsTrue(
                content.Contains(expectedMessage));
        }

        [TestMethod]
        public async Task Handle_WithGeneralException_ShouldReturnInternalServerError()
        {
            var exception =
                new Exception(
                    "Unexpected database failure.");

            var context =
                CreateExceptionHandlerContext(exception);

            var handler =
                new GlobalExceptionHandler();

            handler.Handle(context);

            Assert.IsNotNull(context.Result);

            var response =
                await context.Result.ExecuteAsync(
                    CancellationToken.None);

            Assert.AreEqual(
                HttpStatusCode.InternalServerError,
                response.StatusCode);
        }

        [TestMethod]
        public async Task Handle_WithGeneralException_ShouldReturnGenericErrorMessage()
        {
            var exception =
                new Exception(
                    "Sensitive internal exception.");

            var context =
                CreateExceptionHandlerContext(exception);

            var handler =
                new GlobalExceptionHandler();

            handler.Handle(context);

            Assert.IsNotNull(context.Result);

            var response =
                await context.Result.ExecuteAsync(
                    CancellationToken.None);

            var content =
                await response.Content.ReadAsStringAsync();

            Assert.IsTrue(
                content.Contains(
                    ErrorMessages.InternalServerError));

            Assert.IsFalse(
                content.Contains(
                    "Sensitive internal exception."));
        }

        private static ExceptionHandlerContext
            CreateExceptionHandlerContext(
                Exception exception)
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

            var exceptionContext =
                new ExceptionContext(
                    exception,
                    ExceptionCatchBlocks.HttpServer,
                    request);

            return new ExceptionHandlerContext(
                exceptionContext);
        }
    }
}
