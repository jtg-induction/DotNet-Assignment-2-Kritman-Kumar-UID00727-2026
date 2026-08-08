using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using RestaurantServer.Constants;
using System.Web.Http.Controllers;

namespace RestaurantServer.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                var errors = actionContext.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new
                    {
                        Message = ErrorMessages.ValidationFailed,
                        Errors = errors
                    });
            }
        }
    }
}
