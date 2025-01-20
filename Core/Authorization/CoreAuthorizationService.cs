using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Donatas.Core.Authorization
{
    /// <summary>
    /// Generic helper that extracts many useful details from the token and headers
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    public class CoreAuthorizationService(IHttpContextAccessor httpContextAccessor) : ICoreAuthorizationService
    {
        private readonly IEnumerable<Claim>? _claims = httpContextAccessor.HttpContext?.User.Claims;
        private readonly HttpRequest? _request = httpContextAccessor.HttpContext?.Request;

        public string? AccessToken() =>
            string.IsNullOrEmpty(_request.Headers["X-MS-CLIENT-PRINCIPAL"]) ?
            _request.Headers["X-MS-TOKEN-AAD-ID-TOKEN"] : // OpenId
            _request.Headers["X-MS-CLIENT-PRINCIPAL"]; // Bearer

        public string? IdToken() =>
            string.IsNullOrEmpty(_request.Headers.Authorization) ?
            _request.Headers["X-MS-TOKEN-AAD-ACCESS-TOKEN"] : // OpenId
            _request.Headers.Authorization; // Bearer

        public string? ClientIp() =>
            _request.Headers["CLIENT-IP"];

        public string? Host() =>
            _request.Headers.Host;

        public string? LaName() =>
            _request.Headers["x-ms-workflow-name"];

        public string? LaRunId() =>
            _request.Headers["x-ms-workflow-run-id"];

        public string? LaActionName() =>
            _request.Headers["x-ms-workflow-operation-name"];

        public string? LaRGName() =>
            _request.Headers["x-ms-workflow-resourcegroup-name"];

        public string? ClientId() =>
            _request.Headers["X-MS-CLIENT-PRINCIPAL-NAME"];

        public bool HasRole(string role) =>
            _claims.Where(RoleClaims).Any(_ => _.Value == role);

        public bool IsUser() =>
            !_claims.Any(_ => _.Type == "appid");

        public string? Email() =>
            _claims.FirstOrDefault(_ => _.Type == "Email")?.Value ??
            _claims.FirstOrDefault(_ => _.Type == "preferred_username")?.Value;

        public string? Name() =>
            _claims.FirstOrDefault(_ => _.Type == "name")?.Value;

        public string? AadId() =>
            _claims.FirstOrDefault(_ => _.Type == "oid")?.Value;

        public IEnumerable<string> GetRoles() =>
            _claims.Where(RoleClaims).Select(_ => _.Value);

        private Func<Claim, bool> RoleClaims = _ =>
            _.Type == "roles";
    }
}
