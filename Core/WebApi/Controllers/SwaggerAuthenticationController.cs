using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Donatas.Core.WebApi.Controllers
{
    // dummy controller that does not do anything and is intended to be called from swagger locally for the authentication process to be triggered
    // Back in the day swagger requests could not have been authenticated, so this was a nice workarround
    [ApiVersion("0.5")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    public class SwaggerAuthenticationController : ControllerBase
    {
        [HttpGet]
        [Route("~/api/SignIn")]
        public bool Login() => true;

        [HttpGet]
        [Route("~/api/SignOut")]
        public bool Logout()
        {
            HttpContext.SignOutAsync("Cookies");
            HttpContext.SignOutAsync("oidc");
            return true;
        }
    }
}
