using RestaurantServer.Constants;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace RestaurantServer.Exceptions
{
    /// <summary>
    /// Handles unhandled exceptions globally across the Web API
    /// and returns a standardized internal server error response.
    /// </summary>
    public class GlobalExceptionHandler : IExceptionHandler
    {
        /// <summary>
        /// Handles an unhandled exception and creates a standardized
        /// HTTP 500 Internal Server Error response.
        /// </summary>
        /// <param name="context">
        /// The exception handling context containing the request and exception information.
        /// </param>
        /// <param name="cancellationToken">
        /// A token that can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task representing the exception handling operation.
        /// </returns>
        public Task HandleAsync(
            ExceptionHandlerContext context,
            CancellationToken cancellationToken)
        {
            var response = context.Request.CreateResponse(
                HttpStatusCode.InternalServerError,
                new
                {
                    Message = ErrorMessages.InternalServerError
                });

            context.Result = new ResponseMessageResult(response);

            return Task.CompletedTask;
        }
    }
}