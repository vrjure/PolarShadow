using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PolarShadow.Api.Utilities;
using PolarShadow.Services;

namespace PolarShadow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.Client)]
    public class ServerController : ControllerBase, IServerService
    {
        [HttpGet("time")]
        public ValueTask<DateTime> GetServerTimeAsync()
        {
            return ValueTask.FromResult(DateTime.Now);
        }
    }
}
