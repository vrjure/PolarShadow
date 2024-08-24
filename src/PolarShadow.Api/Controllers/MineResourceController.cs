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
    public class MineResourceController : ControllerBase, IMineResourceService
    {
        private readonly IMineResourceService _mineResource;
        public MineResourceController(IMineResourceService mineResource)
        {
            _mineResource = mineResource;
        }

        [HttpDelete("delete/{rootId}")]
        public async Task DeleteRootResourceAsync([FromRoute] int rootId)
        {
            await _mineResource.DeleteRootResourceAsync(rootId);
        }

        [HttpGet("{id}")]
        public async Task<ResourceModel> GetResourceAsync([FromRoute] int id)
        {
            return await _mineResource.GetResourceAsync(id);
        }

        [HttpGet("children/{rootId}")]
        public async Task<ICollection<ResourceModel>> GetRootChildrenAsync([FromRoute] int rootId)
        {
            return await _mineResource.GetRootChildrenAsync(rootId);
        }

        [HttpGet("children/{rootId}/{level}")]
        public async Task<ICollection<ResourceModel>> GetRootChildrenAsync([FromRoute] int rootId, [FromRoute] int level)
        {
            return await _mineResource.GetRootChildrenAsync(rootId, level);
        }

        [HttpGet("children/count/{rootId}/{level}")]
        public async Task<int> GetRootChildrenCountAsync([FromRoute] int rootId, [FromRoute] int level)
        {
            return await _mineResource.GetRootChildrenCountAsync(rootId, level);
        }

        [HttpGet("{resourceName}/{site}")]
        public async Task<ResourceModel> GetRootResourceAsync([FromRoute] string resourceName, [FromRoute] string site)
        {
            return await _mineResource.GetRootResourceAsync(resourceName, site);
        }

        [HttpGet]
        public async Task<ICollection<ResourceModel>> GetRootResourcesAsync()
        {
            return await _mineResource.GetRootResourcesAsync();
        }

        [HttpPost("save")]
        public async Task SaveResourceAsync([FromBody] ResourceTreeNode tree)
        {
            await _mineResource.SaveResourceAsync(tree);
        }

        [HttpPost("upload")]
        public async Task UploadAsync([FromBody] ICollection<ResourceModel> data)
        {
            await _mineResource.UploadAsync(data);
        }

        [HttpGet("download")]
        public async Task<ICollection<ResourceModel>> DownloadAsync()
        {
            return await _mineResource.DownloadAsync();
        }
    }
}
