using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Headers;
using System.Web;
using Newtonsoft.Json;
using Microsoft.Identity.Client;
using System.Configuration;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Donatas.Core.Authorization;

namespace Donatas.Core.RestClient
{
    public class RestClient : IRestClient
    {
        private readonly HttpClient _client;

        public RestClient(IConfiguration configuration)
        {
            _client ??= new HttpClient(new HttpClientHandler { MaxConnectionsPerServer = 20, UseDefaultCredentials = UseDefaultCredentials });

            var azureAdOptions = configuration.GetSection("AzureAd").Get<AzureAdOptions>() ?? throw new ConfigurationErrorsException("No configuration found for AzureAd");

            TenantId = azureAdOptions.TenantId;
            ClientId = azureAdOptions.ClientId;
            ClientSecret = azureAdOptions.ClientSecret;

        }
        public HttpClient Client { get => _client; }
        public AuthenticationType AuthenticationType { get; set; } = AuthenticationType.Adal;
        private Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationResult _token;
        public string MsalAuthToken { get; set; }
        public string ServerClientId { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public AuthenticationHeaderValue AuthenticationHeader { get; set; }

        public HttpResult<T> Get<T>(Uri address, NameValueCollection urlParameters = null) =>
            CallApi<T>(address, urlParameters, null, HttpMethod.Get);

        public HttpResult<T> Patch<T>(Uri address, NameValueCollection urlParameters = null, object body = null, BodyType bodyType = BodyType.raw) =>
            CallApi<T>(address, urlParameters, body, new HttpMethod("PATCH"), bodyType);

        public HttpResult<T> Put<T>(Uri address, NameValueCollection urlParameters = null, object body = null, BodyType bodyType = BodyType.raw) =>
            CallApi<T>(address, urlParameters, body, HttpMethod.Put, bodyType);

        public HttpResult<T> Post<T>(Uri address, NameValueCollection urlParameters = null, object body = null, BodyType bodyType = BodyType.raw) =>
            CallApi<T>(address, urlParameters, body, HttpMethod.Post, bodyType);

        public HttpResult<T> Delete<T>(Uri address, NameValueCollection urlParameters = null) =>
            CallApi<T>(address, urlParameters, null, HttpMethod.Delete);

        public async Task<HttpResult<T>> GetAsync<T>(Uri address, NameValueCollection urlParameters = null) =>
            await CallApiAsync<T>(address, urlParameters, null, HttpMethod.Get);

        public async Task<HttpResult<T>> PatchAsync<T>(Uri address, NameValueCollection urlParameters = null, object body = null, BodyType bodyType = BodyType.raw) =>
            await CallApiAsync<T>(address, urlParameters, body, new HttpMethod("PATCH"), bodyType);

        public async Task<HttpResult<T>> PutAsync<T>(Uri address, NameValueCollection urlParameters = null, object body = null, BodyType bodyType = BodyType.raw) =>
            await CallApiAsync<T>(address, urlParameters, body, HttpMethod.Put, bodyType);

        public async Task<HttpResult<T>> PostAsync<T>(Uri address, NameValueCollection urlParameters = null, object body = null, BodyType bodyType = BodyType.raw) =>
            await CallApiAsync<T>(address, urlParameters, body, HttpMethod.Post, bodyType);

        public async Task<HttpResult<T>> DeleteAsync<T>(Uri address, NameValueCollection urlParameters = null) =>
            await CallApiAsync<T>(address, urlParameters, null, HttpMethod.Delete);

        private HttpResult<T> CallApi<T>(
            Uri address,
            NameValueCollection urlParameters,
            object body, HttpMethod httpMethod,
            BodyType bodyType = BodyType.raw,
            SecurityProtocolType securityProtocolType = SecurityProtocolType.Tls12
        )
        {
            var result = new HttpResult<T>();
            SetUp<T>(address, securityProtocolType);

            try
            {
                var content = GetBody(body, httpMethod, bodyType);
                var url = GetUri(address, urlParameters);

                var response = httpMethod.Method.ToUpper() switch
                {
                    "GET" => Task.Run(async () => await _client.GetAsync(url).ConfigureAwait(false)).Result,
                    "PUT" => Task.Run(async () => await _client.PutAsync(url, content).ConfigureAwait(false)).Result,
                    "POST" => Task.Run(async () => await _client.PostAsync(url, content).ConfigureAwait(false)).Result,
                    "DELETE" => Task.Run(async () => await _client.DeleteAsync(url).ConfigureAwait(false)).Result,
                    "PATCH" => Task.Run(async () => await _client.SendAsync(new HttpRequestMessage { Content = content, Method = new HttpMethod("PATCH"), RequestUri = url }).ConfigureAwait(false)).Result,
                    _ => throw new NotSupportedException($"Http method {httpMethod.Method} is not supported")
                };

                result = Task.Run(async () => await SetUpResponse<T>(response).ConfigureAwait(false)).Result;
            }
            catch (WebException ex)
            {
                result.Error = $"Status: {ex.Status}, error: {ex}";
            }
            catch (Exception ex)
            {
                result.Error = ex.ToString();
            }

            return result;
        }

        private async Task<HttpResult<T>> CallApiAsync<T>(
            Uri address,
            NameValueCollection urlParameters,
            object body,
            HttpMethod httpMethod,
            BodyType bodyType = BodyType.raw,
            SecurityProtocolType securityProtocolType = SecurityProtocolType.Tls12
        )
        {
            var result = new HttpResult<T>();

            SetUp<T>(address, securityProtocolType);

            try
            {
                var content = GetBody(body, httpMethod, bodyType);
                var url = GetUri(address, urlParameters);

                using var response = httpMethod.Method.ToUpper() switch
                {
                    "GET" => await _client.GetAsync(url),
                    "PUT" => await _client.PutAsync(url, content),
                    "POST" => await _client.PostAsync(url, content),
                    "DELETE" => await _client.DeleteAsync(url),
                    "PATCH" => await _client.SendAsync(new HttpRequestMessage { Content = content, Method = new HttpMethod("PATCH"), RequestUri = url }),
                    _ => throw new NotSupportedException($"Http method {httpMethod.Method} is not supported")
                };

                result = await SetUpResponse<T>(response);
            }
            catch (WebException ex)
            {
                result.Error = $"Status: {ex.Status}, error: {ex}";
            }
            catch (Exception ex)
            {
                result.Error = ex.ToString();
            }

            return result;
        }

        private HttpContent GetBody(object body, HttpMethod httpMethod, BodyType bodyType)
        {
            if (body == null)
                return null;

            return bodyType switch
            {
                BodyType.raw =>
                    new StringContent(body?.GetType() == typeof(string) ?
                    body.ToString() :
                    JsonConvert.SerializeObject(body), System.Text.Encoding.UTF8, httpMethod.Method.ToUpper() == "PATCH" ? "application/json-patch+json" : "application/json"),
                BodyType.typed =>
                    (HttpContent)body,
                BodyType.x_www_form_urlencoded =>
                    body?.GetType() == typeof(Dictionary<string, string>) ?
                    new FormUrlEncodedContent((Dictionary<string, string>)body) :
                    throw new ArgumentException("Passed body should be in Dictionary <string, string> type"),
                _ =>
                    throw new NotSupportedException($"Http client body type {bodyType} is not supported")
            };
        }

        private void SetUp<T>(Uri address, SecurityProtocolType securityProtocolType)
        {
            // Required for calling Azure function app
            ServicePointManager.SecurityProtocol = securityProtocolType;

            if (!string.IsNullOrEmpty(ServerClientId))
                SetUpHeaders(address);
            else if (AuthenticationHeader != null)
                _client.DefaultRequestHeaders.Authorization = AuthenticationHeader;
        }

        private async Task<HttpResult<T>> SetUpResponse<T>(HttpResponseMessage response)
        {
            var result = new HttpResult<T>();
            var data = await response.Content.ReadAsStringAsync();
            result.HttpStatus = response.StatusCode;

            if (response.IsSuccessStatusCode)
            {
                TryParseToT<T>(result, data);
            }
            else
                result.Error = data;
            return result;
        }

        private void SetUpHeaders(Uri address)
        {
            if (string.IsNullOrEmpty(ClientId))
                throw new ConfigurationErrorsException($"Please set up ClientId in order to use token authentication");

            if (string.IsNullOrEmpty(ClientSecret) && AuthenticationType != AuthenticationType.ManagedIdentity)
                throw new ConfigurationErrorsException($"Please set up ClientSecret in order to use token authentication");

            switch (AuthenticationType)
            {
                case AuthenticationType.Adal:
                    {
                        if (_token == null || _token.ExpiresOn.ToUniversalTime() < DateTime.Now.ToUniversalTime())
                        {
                            var authContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext($"https://login.microsoftonline.com/{TenantId}");
                            var token = Task.Run(async () => await authContext.AcquireTokenAsync(
                                ServerClientId,
                                new Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential(ClientId, ClientSecret)
                            ).ConfigureAwait(false)).Result;

                            _token = token;
                        }

                        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token.AccessToken);
                        break;
                    }
                case AuthenticationType.Msal:
                    {
                        var aadIdtoken = string.IsNullOrEmpty(MsalAuthToken) ?
                            throw new OperationCanceledException($"Please provide {nameof(MsalAuthToken)} property, retrieved from X-MS-TOKEN-AAD-ID-TOKEN header in order to use Msal authentication") :
                            MsalAuthToken;
                        var userAssertion = new Microsoft.Identity.Client.UserAssertion(aadIdtoken, "urn:ietf:params:oauth:grant-type:jwt-bearer");
                        var scopes = new[] { $"{ServerClientId}/.default" };
                        var token = Task.Run(async () => await ConfidentialClientApplicationBuilder
                            .Create(ClientId)
                            .WithClientSecret(ClientSecret)
                            .Build()
                            .AcquireTokenOnBehalfOf(scopes, userAssertion)
                            .WithTenantId(TenantId)
                            .ExecuteAsync().ConfigureAwait(false)).Result;

                        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                        break;
                    }
                case AuthenticationType.ManagedIdentity:
                    {
                        var accessToken = Task.Run(async () =>
                            await new ManagedIdentityCredential(ClientId).GetTokenAsync(
                                new Azure.Core.TokenRequestContext(scopes: [$"{ServerClientId}/.default"])
                            ).ConfigureAwait(false)).Result;

                        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Token);
                        break;
                    }
            }
        }

        private Uri GetUri(Uri address, NameValueCollection urlParameters)
        {
            var @params = HttpUtility.ParseQueryString(string.Empty);
            urlParameters?
                .Cast<string>()
                .Where(key => !string.IsNullOrEmpty(urlParameters[key]))
                .ToList()
                .ForEach(_ => @params[_] = urlParameters[_]);

            return new Uri($"{address}{(@params.Keys.Count > 0 ? "?" : string.Empty)}{@params}");
        }

        private void TryParseToT<T>(HttpResult<T> result, string data)
        {
            try
            {
                result.RawContent = data;
                result.Content = new[] { typeof(string), typeof(object) }.Contains(typeof(T)) ?
                    (T)(object)data :
                    JsonConvert.DeserializeObject<T>(data);
            }
            catch (JsonReaderException ex)
            {
                result.Content = default;
                result.Error = ex.Message;
            }
        }

        public T GetBody<T>(HttpContent req)
        {
            var request = Task.Run(async () => await req.ReadAsStringAsync().ConfigureAwait(false));
            var result = Task.Run(async () => await request.ConfigureAwait(false)).Result;
            return JsonConvert.DeserializeObject<T>(result);
        }
    }
}
