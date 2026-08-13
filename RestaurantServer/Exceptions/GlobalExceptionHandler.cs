using RestaurantServer.Constants;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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
        /// HTTP response.
        /// </summary>
        public Task HandleAsync(
            ExceptionHandlerContext context,
            CancellationToken cancellationToken)
        {
            HttpStatusCode statusCode;
            object response;

            if (context.Exception is RestaurantServer.Exceptions.ValidationException validationException)
            {
                statusCode = HttpStatusCode.BadRequest;

                response = new
                {
                    Message = validationException.Message
                };
            }
            else
            {
                statusCode = HttpStatusCode.InternalServerError;

                response = new
                {
                    Message = ErrorMessages.InternalServerError
                };
            }
                
            context.Result = new ResponseMessageResult(
                context.Request.CreateResponse(statusCode, response)
            );

            return Task.CompletedTask;
        }
    }
}
