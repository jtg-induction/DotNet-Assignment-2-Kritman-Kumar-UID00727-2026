using System.Net;
using System.Web.Http.ExceptionHandling;
using RestaurantServer.Constants;

namespace RestaurantServer.Exceptions
{
    public class GlobalExceptionHandler : ExceptionHandler
    {
        public override void Handle(ExceptionHandlerContext context)
        {
            HttpStatusCode statusCode;
            object response;

            if (context.Exception is BusinessException businessException)
            {
                statusCode = HttpStatusCode.BadRequest;

                response = new
                {
                    Message = businessException.Message
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

            context.Result = new ErrorMessageResult(
                context.Request,
                statusCode,
                response
            );
        }
    }
}
