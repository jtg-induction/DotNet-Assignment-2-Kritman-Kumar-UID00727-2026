using RestaurantServer.Constants;
using RestaurantServer.DTOs.Requests;
using RestaurantServer.Helpers.Interfaces;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace RestaurantServer.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly IAuthService _authService;
        private readonly IRefreshTokenHelper _refreshTokenHelper;
        private readonly ICookieHelper _cookieHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="authService">
        /// The authentication service used to perform authentication and token operations.
        /// </param>
        /// <param name="cookieHelper">
        /// The helper used to manage authentication cookies.
        /// </param>
        public AuthController(
            IAuthService authService,
            IRefreshTokenHelper refreshTokenHelper,
            ICookieHelper cookieHelper)
        {
            _authService = authService;
            _refreshTokenHelper = refreshTokenHelper;
            _cookieHelper = cookieHelper;
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

            var cookie = _cookieHelper.CreateHttpOnlySecureCookie("refreshToken", result.RefreshToken, "auth");

            response.Headers.AddCookies(new[] { cookie });

            return ResponseMessage(response);
        }

        /// <summary>
        /// Generates a new access token using the refresh token stored in the request cookie.
        /// </summary>
        /// <returns>
        /// An HTTP 200 response containing the new authentication response and refresh token.
        /// </returns>
        [HttpPost]
        [Route("refresh")]
        public async Task<IHttpActionResult> Refresh()
        {
            var refreshToken =
                _refreshTokenHelper.GetRefreshTokenFromRequest(Request, "refreshToken");

            var result =
                await _authService.RefreshTokenAsync(refreshToken);

            var cookie = _cookieHelper.CreateHttpOnlySecureCookie("refreshToken", result.RefreshToken, "auth");

            var response = Request.CreateResponse(
                HttpStatusCode.OK,
                result.Response
            );

            response.Headers.AddCookies(new[] { cookie });

            return ResponseMessage(response);
        }

        /// <summary>
        /// Logs out the current user by invalidating the refresh token
        /// and expiring the refresh token cookie.
        /// </summary>
        /// <returns>
        /// An HTTP 200 response containing a logout confirmation message.
        /// </returns>
        [HttpPost]
        [Route("logout")]
        public async Task<IHttpActionResult> Logout()
        {
            var refreshToken = _refreshTokenHelper.GetRefreshTokenFromRequest(Request, "refreshToken");

            await _authService.LogoutAsync(refreshToken);

            var response = Request.CreateResponse(
                HttpStatusCode.OK,
                new
                {
                    Message = SuccessMessages.LogoutSuccessful
                });

            var expiredCookie = _cookieHelper.CreateHttpOnlySecureCookie("refreshToken", string.Empty, "auth");

            response.Headers.AddCookies(new[] { expiredCookie });

            return ResponseMessage(response);
        }
    }
}
