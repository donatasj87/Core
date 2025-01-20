using System.Net;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;

namespace Donatas.Core.Extensions
{
    /// <summary>
    /// Usefull in function apps to get the parameters
    /// </summary>
    public static class HttpRequestDataExtensions
    {
        public static T? GetBody<T>(this HttpRequestData httpRequestData)
        {
            using var streamReader = new StreamReader(httpRequestData.Body);
            var requestBody = streamReader.ReadToEnd();
            var data = JsonConvert.DeserializeObject<T>(requestBody);
            return data;
        }

        public static async Task<T?> GetBodyAsync<T>(this HttpRequestData httpRequestData)
        {
            using var streamReader = new StreamReader(httpRequestData.Body);
            var requestBody = await streamReader.ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<T>(requestBody);
            return data;
        }

        public static HttpResponseData Response(this HttpRequestData httpRequestData, HttpStatusCode httpStatusCode, string output)
        {
            var requestResponse = httpRequestData.CreateResponse(httpStatusCode);
            requestResponse.Headers.Add("Content-Type", "text/json; charset=utf-8");
            requestResponse.WriteString(output);
            return requestResponse;
        }
    }
}
