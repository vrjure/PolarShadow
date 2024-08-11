using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PolarShadow.Services;

namespace PolarShadow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerController : ControllerBase, IServerService
    {
        [HttpGet("time")]
        public ValueTask<DateTime> GetServerTime()
        {
            return ValueTask.FromResult(DateTime.Now);
        }
    }
}
