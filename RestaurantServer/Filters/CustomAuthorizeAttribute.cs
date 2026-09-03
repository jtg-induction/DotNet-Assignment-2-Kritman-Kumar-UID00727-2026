using RestaurantServer.Constants;
using RestaurantServer.Enums;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace RestaurantServer.Filters
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly UserRole[] _allowedRoles;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAuthorizeAttribute"/> class.
        /// </summary>
        /// <param name="roles">
        /// The user roles allowed to access the decorated action.
        /// </param>
        public CustomAuthorizeAttribute(params UserRole[] roles)
        {
            _allowedRoles = roles;
        }

        /// <summary>
        /// Determines whether the current user is authenticated and has
        /// at least one of the required roles.
        /// </summary>
        /// <param name="actionContext">
        /// The context for the current HTTP action.
        /// </param>
        /// <returns>
        /// <c>true</c> if the user is authenticated and authorized;
        /// otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsAuthorized(
            HttpActionContext actionContext)
        {   
            var principal =
                actionContext.RequestContext.Principal
                as ClaimsPrincipal;

            if (principal == null ||
                principal.Identity == null ||
                !principal.Identity.IsAuthenticated)
            {
                return false;
            }

            if (_allowedRoles == null ||
                _allowedRoles.Length == 0)
            {
                return true;
            }

            return _allowedRoles.Any(role =>
                principal.IsInRole(((int)role).ToString()));
        }

        /// <summary>
        /// Handles unauthorized requests by returning either a 401 Unauthorized
        /// response for unauthenticated users or a 403 Forbidden response for
        /// authenticated users without the required role.
        /// </summary>
        /// <param name="actionContext">
        /// The context for the current HTTP action.
        /// </param>
        protected override void HandleUnauthorizedRequest(
            HttpActionContext actionContext)
        {
            var principal =
                actionContext.RequestContext.Principal;

            if (principal?.Identity?.IsAuthenticated != true)
            {
                actionContext.Response =
                    actionContext.Request.CreateResponse(
                        HttpStatusCode.Unauthorized,
                        new
                        {
                            Message = ValidationMessages.AuthenticationRequired
                        });

                return;
            }

            actionContext.Response =
                actionContext.Request.CreateResponse(
                    HttpStatusCode.Forbidden,
                    new
                    {
                        Message = ErrorMessages.NotAuthorized
                    });
        }
    }
}
