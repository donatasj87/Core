using System.Net;

namespace Donatas.Core.WebApi.Responses
{
    public interface ICoreResponse
    {
        HttpStatusCode StatusCode { get; set; }
        string Message { get; set; }
        IEnumerable<string> Errors { get; set; }
    }
}
