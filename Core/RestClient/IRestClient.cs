using System.Collections.Specialized;
using System.Net.Http.Headers;

namespace Donatas.Core.RestClient
{
    public interface IRestClient
    {
        HttpClient Client { get; }

        /// <summary>
        /// AR id of application to be called
        /// </summary>
        string ServerClientId { get; set; }

        /// <summary>
        /// Azure Tenant Id
        /// </summary>
        string TenantId { get; set; }

        /// <summary>
        /// AR id of the calling application
        /// </summary>
        string ClientId { get; set; }

        /// <summary>
        /// AR secret of the calling application
        /// </summary>
        string ClientSecret { get; set; }

        /// <summary>
        /// Provide Authentication Context yourself, generated from some token
        /// </summary>
        AuthenticationHeaderValue AuthenticationHeader { get; set; }

        /// <summary>
        /// Authentication type, default is Adal
        /// https://docs.microsoft.com/en-us/azure/active-directory/develop/msal-net-differences-adal-net
        /// </summary>
        AuthenticationType AuthenticationType { get; set; }

        /// <summary>
        /// When requesting resource with MSAL authentication, here you should provide `X-MS-TOKEN-AAD-ID-TOKEN` request header.
        /// If running from localhost, just use the calling application url with {appUrl}/.auth/me and use id_token from there
        /// </summary>
        string MsalAuthToken { get; set; }

        /// <summary>
        /// Use this when Windows authentication is needed to send the current user credentials
        /// </summary>
        bool UseDefaultCredentials { get; set; }

        /// <summary>
        /// Method for GET request
        /// </summary>
        /// <typeparam name="T">Expected return type</typeparam>
        /// <param name="address">Uri to make the GET request</param>
        /// <param name="urlParameters">Parameters to pass to Uri</param>
        /// <returns>The result of the request</returns>
        HttpResult<T> Get<T>(Uri address, NameValueCollection urlParameters = null);

        /// <summary>
        /// Method for PATCH request
        /// </summary>
        /// <typeparam name="T">Expected return type</typeparam>
        /// <param name="address">Uri to make the PATCH request</param>
        /// <param name="urlParameters">Parameters to pass to Uri</param>
        /// <param name="body">Any object to pass via body or already serialised string</param>
        /// <param name="bodyType">Define how the body value should be sent</param>
        /// <returns>The result of the request</returns>
        HttpResult<T> Patch<T>(Uri address, NameValueCollection urlParameters = null, object body = null, BodyType bodyType = BodyType.raw);

        /// <summary>
        /// Method for PUT request
        /// </summary>
        /// <typeparam name="T">Expected return type</typeparam>
        /// <param name="address">Uri to make the PUT request</param>
        /// <param name="urlParameters">Parameters to pass to Uri</param>
        /// <param name="body">Any object to pass via body or already serialised string</param>
        /// <param name="bodyType">Define how the body value should be sent</param>
        /// <returns>The result of the request</returns>
        HttpResult<T> Put<T>(Uri address, NameValueCollection urlParameters = null, object body = null, BodyType bodyType = BodyType.raw);

        /// <summary>
        /// Method for POST request
        /// </summary>
        /// <typeparam name="T">Expected return type</typeparam>
        /// <param name="address">Uri to make the POST request</param>
        /// <param name="urlParameters">Parameters to pass to Uri</param>
        /// <param name="body">Any object to pass via body or already serialised string</param>
        /// <param name="bodyType">Define how the body value should be sent</param>
        /// <returns>The result of the request</returns>
        HttpResult<T> Post<T>(Uri address, NameValueCollection urlParameters = null, object body = null, BodyType bodyType = BodyType.raw);

        /// <summary>
        /// Method for DELETE request
        /// </summary>
        /// <typeparam name="T">Expected return type</typeparam>
        /// <param name="address">Uri to make the DELETE request</param>
        /// <param name="urlParameters">Parameters to pas to Uri</param>
        /// <returns>The result of the request</returns>
        HttpResult<T> Delete<T>(Uri address, NameValueCollection urlParameters = null);

        /// <summary>
        /// Method for extracting parameters sent via http request body
        /// </summary>
        /// <typeparam name="T">Type that you want to deserialize</typeparam>
        /// <param name="req">Request.Content object from controller</param>
        /// <returns>Body of the http content</returns>
        T GetBody<T>(HttpContent req);

        /// <summary>
        /// Method for asynchronous GET request
        /// </summary>
        /// <typeparam name="T">Expected return type</typeparam>
        /// <param name="address">Uri to make the GET request</param>
        /// <param name="urlParameters">Parameters to pass to Uri</param>
        /// <returns>The result of the request</returns>
        Task<HttpResult<T>> GetAsync<T>(Uri address, NameValueCollection urlParameters = null);

        /// <summary>
        /// Method for asynchronous PATCH request
        /// </summary>
        /// <typeparam name="T">Expected return type</typeparam>
        /// <param name="address">Uri to make the PATCH request</param>
        /// <param name="urlParameters">Parameters to pass to Uri</param>
        /// <param name="body">Any object to pass via body or already serialised string</param>
        /// <param name="bodyType">Define how the body value should be sent</param>
        /// <returns>The result of the request</returns>
        Task<HttpResult<T>> PatchAsync<T>(Uri address, NameValueCollection urlParameters = null, object body = null, BodyType bodyType = BodyType.raw);

        /// <summary>
        /// Method for asynchronous PUT request
        /// </summary>
        /// <typeparam name="T">Expected return type</typeparam>
        /// <param name="address">Uri to make the PUT request</param>
        /// <param name="urlParameters">Parameters to pass to Uri</param>
        /// <param name="body">Any object to pass via body or already serialised string</param>
        /// <param name="bodyType">Define how the body value should be sent</param>
        /// <returns>The result of the request</returns>
        Task<HttpResult<T>> PutAsync<T>(Uri address, NameValueCollection urlParameters = null, object body = null, BodyType bodyType = BodyType.raw);

        /// <summary>
        /// Method for asynchronous POST request
        /// </summary>
        /// <typeparam name="T">Expected return type</typeparam>
        /// <param name="address">Uri to make the POST request</param>
        /// <param name="urlParameters">Parameters to pass to Uri</param>
        /// <param name="body">Any object to pass via body or already serialised string</param>
        /// <param name="bodyType">Define how the body value should be sent</param>
        /// <returns>The result of the request</returns>
        Task<HttpResult<T>> PostAsync<T>(Uri address, NameValueCollection urlParameters = null, object body = null, BodyType bodyType = BodyType.raw);

        /// <summary>
        /// Method for asynchronous DELETE request
        /// </summary>
        /// <typeparam name="T">Expected return type</typeparam>
        /// <param name="address">Uri to make the DELETE request</param>
        /// <param name="urlParameters">Parameters to pas to Uri</param>
        /// <returns>The result of the request</returns>
        Task<HttpResult<T>> DeleteAsync<T>(Uri address, NameValueCollection urlParameters = null);
    }
}
