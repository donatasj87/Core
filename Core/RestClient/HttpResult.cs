using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Donatas.Core.RestClient
{
    public class HttpResult<T> : IActionResult
    {
        public HttpStatusCode HttpStatus { get; set; }

        public HttpRequestStatus RequestStatus
        {
            get
            {
                if (string.IsNullOrEmpty(Error)
                    && HttpStatus != HttpStatusCode.Unauthorized)
                {
                    return HttpRequestStatus.Success;
                }

                return HttpRequestStatus.Fail;
            }
        }

        public T Content { get; set; }
        public string RawContent { get; set; }
        public string Error { get; set; }

        Task IActionResult.ExecuteResultAsync(ActionContext context)
        {
            var response = new HttpResponseMessage(HttpStatus)
            {
                Content = new StringContent(Content.ToString())
            };
            
            return Task.FromResult(response);
        }
    }
}
