using RestaurantServer.Constants;
using RestaurantServer.DTOs.Requests;  
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using RestaurantServer.Exceptions;

namespace RestaurantServer.Controllers
{
    [RoutePrefix("auth")] 
    public class AuthController : ApiController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("signup")]
        public async Task<IHttpActionResult> Signup(SignupRequest request)
        {
            var response = await _authService.SignupAsync(request);

            return Content(HttpStatusCode.Created, response);
        }

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

        [HttpPost]
        [Route("refresh")]
        public async Task<IHttpActionResult> Refresh()
        {
            var refreshTokenCookie = Request.Headers
                .GetCookies("refreshToken")
                .FirstOrDefault();

            if (refreshTokenCookie == null)
            {
                throw new BusinessException(
                    ValidationMessages.InvalidRefreshToken);
            }

            var refreshToken = refreshTokenCookie["refreshToken"]?.Value;

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new BusinessException(
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
    }
}
