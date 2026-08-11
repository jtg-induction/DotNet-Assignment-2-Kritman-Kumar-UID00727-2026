using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace RestaurantServer.Exceptions
{
    public class ErrorMessageResult : IHttpActionResult
    {
        private readonly HttpRequestMessage _request;
        private readonly HttpStatusCode _statusCode;
        private readonly object _content;

        public ErrorMessageResult(
            HttpRequestMessage request,
            HttpStatusCode statusCode,
            object content)
        {
            _request = request;
            _statusCode = statusCode;
            _content = content;
        }

        public Task<HttpResponseMessage> ExecuteAsync(
            CancellationToken cancellationToken)
        {
            var response = _request.CreateResponse(
                _statusCode,
                _content
            );

            return Task.FromResult(response);
        }
    }
}
