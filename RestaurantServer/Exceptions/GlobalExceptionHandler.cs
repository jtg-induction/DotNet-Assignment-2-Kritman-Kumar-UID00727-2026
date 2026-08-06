using System.Net;
using System.Net.Http;
using System.Web.Http.ExceptionHandling;
using RestaurantServer.Constants;

namespace RestaurantServer.Exceptions
{
    public class GlobalExceptionHandler : ExceptionHandler
    {
        public override void Handle(ExceptionHandlerContext context)
        {
            var response = new
            {
                Message = ErrorMessages.InternalServerError
            };

            context.Result = new ErrorMessageResult(
                context.Request,
                HttpStatusCode.InternalServerError,
                response
            );
        }
    }
}
