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

        public CustomAuthorizeAttribute(params UserRole[] roles)
        {
            _allowedRoles = roles;
        }

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

        protected override void HandleUnauthorizedRequest(
            HttpActionContext actionContext)
        {
            var principal =
                actionContext.RequestContext.Principal;

            // User is not authenticated
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

            // User is authenticated but doesn't have permission
            actionContext.Response =
                actionContext.Request.CreateResponse(
                    HttpStatusCode.Forbidden,
                    new
                    {
                        Message = ValidationMessages.NotAuthorized
                    });
        }
    }
}
