using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using RestaurantServer.Constants;
using System.Web.Http.Controllers;

namespace RestaurantServer.Filters
{
    /// <summary>
    /// Validates the model state before an action is executed and returns
    /// validation errors when the model is invalid.
    /// </summary>
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Executes before the controller action and returns a bad request
        /// response when model validation fails.
        /// </summary>
        /// <param name="actionContext">
        /// The context for the current HTTP action.
        /// </param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                var errors = actionContext.ModelState
                    .Where(modelState => modelState.Value.Errors.Count > 0)
                    .ToDictionary(
                        modelState =>
                            modelState.Key.StartsWith("request.")
                                ? modelState.Key.Substring("request.".Length)
                                : modelState.Key,
                        modelState => modelState.Value.Errors
                            .Select(modelStateError => modelStateError.ErrorMessage)
                            .ToArray()
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
