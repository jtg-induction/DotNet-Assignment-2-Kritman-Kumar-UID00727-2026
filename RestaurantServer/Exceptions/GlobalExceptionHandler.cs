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
        public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            if (context.Exception is ValidationException validationException)
            {
                context.Result = new ResponseMessageResult(context.Request
                    .CreateResponse(HttpStatusCode.BadRequest,
                        new
                        {
                            Message = validationException.Message
                        }
                    )
                );
                return Task.CompletedTask;
            }

            context.Result = new ResponseMessageResult(
                context.Request.CreateResponse(
                    HttpStatusCode.InternalServerError,
                    new
                    {
                        Message = ErrorMessages.InternalServerError
                    }));

            return Task.CompletedTask;
        }
    }
}
