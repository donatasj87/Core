using System.Net;

namespace Donatas.Core.WebApi.Responses
{
    public class CoreResponse(HttpStatusCode statusCode, string message, IEnumerable<string> errors) : ICoreResponse
    {
        public HttpStatusCode StatusCode { get; set; } = statusCode;
        public string Message { get; set; } = message;
        public IEnumerable<string> Errors { get; set; } = errors;
    }
}
