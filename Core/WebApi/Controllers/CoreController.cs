using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donatas.Core.WebApi.Controllers
{
    // You can choose to have one or both of those routes
    [Route("api/[controller]/[action]")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [Authorize]
    [ApiController]
    public class CoreController : ControllerBase { }
}
