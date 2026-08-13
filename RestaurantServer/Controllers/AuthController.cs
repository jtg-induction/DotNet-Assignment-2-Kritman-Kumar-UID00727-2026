using RestaurantServer.Constants;
using RestaurantServer.DTOs.Requests;  
using RestaurantServer.Exceptions;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace RestaurantServer.Controllers
{
    [RoutePrefix("auth")] 
    public class AuthController : ApiController
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="authService">
        /// The authentication service used to perform authentication and token operations.
        /// </param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="request">
        /// The user registration details.
        /// </param>
        /// <returns>
        /// An HTTP 201 response containing the newly registered user's information.
        /// </returns>
        [HttpPost]
        [Route("signup")]
        public async Task<IHttpActionResult> Signup(SignupRequest request)
        {
            var response = await _authService.SignupAsync(request);

            return Content(HttpStatusCode.Created, response);
        }

        /// <summary>
        /// Authenticates a user and issues an access token and refresh token.
        /// The refresh token is stored in a secure HTTP-only cookie.
        /// </summary>
        /// <param name="request">
        /// The user's login credentials.
        /// </param>
        /// <returns>
        /// An HTTP 200 response containing the authentication response.
        /// </returns>
        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);

            var response = Request.CreateResponse(
                HttpStatusCode.OK,
                result.Response
            );

            var cookie = new CookieHeaderValue(
                "refreshToken",
                result.RefreshToken
            );

            cookie.HttpOnly = true;
            cookie.Secure = true;
            cookie.Path = "/auth";

            response.Headers.AddCookies(new[] { cookie });

            return ResponseMessage(response);
        }

        /// <summary>
        /// Generates a new access token using the refresh token stored in the request cookie.
        /// </summary>
        /// <returns>
        /// An HTTP 200 response containing the new authentication response and refresh token.
        /// </returns>
        /// <exception cref="ValidationException">
        /// Thrown when the refresh token cookie is missing or invalid.
        /// </exception>
        [HttpPost]
        [Route("refresh")]
        public async Task<IHttpActionResult> Refresh()
        {
            var refreshTokenCookie = Request.Headers
                .GetCookies("refreshToken")
                .FirstOrDefault();

            if (refreshTokenCookie == null)
            {
                throw new ValidationException(
                    ValidationMessages.InvalidRefreshToken);
            }

            var refreshToken = refreshTokenCookie["refreshToken"]?.Value;

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ValidationException(
                    ValidationMessages.InvalidRefreshToken);
            }

            var result = await _authService.RefreshTokenAsync(refreshToken);

            var response = Request.CreateResponse(
                HttpStatusCode.OK,
                result.Response
            );

            var newCookie = new CookieHeaderValue(
                "refreshToken",
                result.RefreshToken
            );

            newCookie.HttpOnly = true;
            newCookie.Secure = true;
            newCookie.Path = "/auth";

            response.Headers.AddCookies(new[] { newCookie });

            return ResponseMessage(response);
        }

        /// <summary>
        /// Logs out the current user by invalidating the refresh token
        /// and expiring the refresh token cookie.
        /// </summary>
        /// <returns>
        /// An HTTP 200 response containing a logout confirmation message.
        /// </returns>
        /// <exception cref="ValidationException">
        /// Thrown when the refresh token cookie is missing or invalid.
        /// </exception>
        [HttpPost]
        [Route("logout")]
        public async Task<IHttpActionResult> Logout()
        {
            var refreshTokenCookie = Request.Headers
                .GetCookies("refreshToken")
                .FirstOrDefault();

            if (refreshTokenCookie == null)
            {
                throw new ValidationException(
                    ValidationMessages.InvalidRefreshToken);
            }

            var refreshToken =
                refreshTokenCookie["refreshToken"]?.Value;

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ValidationException(
                    ValidationMessages.InvalidRefreshToken);
            }

            await _authService.LogoutAsync(refreshToken);

            var response = Request.CreateResponse(
                HttpStatusCode.OK,
                new
                {
                    Message = SuccessMessages.LogoutSuccessful
                });

            var expiredCookie = new CookieHeaderValue(
                "refreshToken",
                ""
            );

            expiredCookie.HttpOnly = true;
            expiredCookie.Secure = true;
            expiredCookie.Path = "/auth";
            expiredCookie.Expires = DateTimeOffset.UtcNow.AddDays(-1);

            response.Headers.AddCookies(
                new[] { expiredCookie });

            return ResponseMessage(response);
        }
    }
}
