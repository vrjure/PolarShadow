using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PolarShadow.Core;
using PolarShadow.Storage;

namespace PolarShadow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyCollectionController : ControllerBase, IMyCollectionService
    {
        private readonly IMyCollectionService _myCollectionService;
        public MyCollectionController(IMyCollectionService myCollectionService) 
        {
            _myCollectionService = myCollectionService;
        }

        [HttpPost("add")]
        public async Task AddToMyCollectionAsync([FromBody]VideoSummary summary)
        {
            await _myCollectionService.AddToMyCollectionAsync(summary);
        }

        [HttpGet("list")]
        public async Task<ICollection<VideoSummary>> GetMyCollectionAsync([FromQuery]int page,[FromQuery]int pageSize)
        {
            return await _myCollectionService.GetMyCollectionAsync(page, pageSize);
        }

        public Task<bool> HasAsync(VideoSummary summary)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("delete")]
        public async Task RemoveFromMyCollectionAsync([FromQuery]string name)
        {
            await _myCollectionService.RemoveFromMyCollectionAsync(name);
        }
    }
}
