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
    public class HistoryController : ControllerBase, IHistoryService
    {
        private readonly IHistoryService _historyService;
        public HistoryController(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        [HttpPost("addOrUpdate")]
        public async Task AddOrUpdateAsync([FromBody] HistoryModel model)
        {
            await _historyService.AddOrUpdateAsync(model);
        }

        [HttpDelete("delete/{id}")]
        public async Task DeleteAsync([FromRoute] long id)
        {
            await _historyService.DeleteAsync(id);
        }

        [HttpGet("byId/{id:long}")]
        public async Task<HistoryModel> GetByIdAsync([FromRoute] long id)
        {
            return await _historyService.GetByIdAsync(id);
        }

        [HttpGet("byName/{resourceName}")]
        public async Task<HistoryModel> GetByResourceNameAsync([FromRoute] string resourceName)
        {
            return await _historyService.GetByResourceNameAsync(resourceName);
        }

        [HttpGet]
        public async Task<ICollection<HistoryModel>> GetListPageAsync([FromQuery] int page, [FromQuery]int pageSize, [FromQuery]string? filter = null)
        {
            return await _historyService.GetListPageAsync(page, pageSize, filter);
        }

        [HttpPost("upload")]
        public async Task UploadAsync([FromBody] ICollection<HistoryModel> data)
        {
            await _historyService.UploadAsync(data);
        }

        [HttpGet("download")]
        public async Task<ICollection<HistoryModel>> DownloadAsync([FromQuery] DateTime updateTime)
        {
            return await _historyService.DownloadAsync(updateTime);
        }

        [HttpGet("lastTime")]
        public async Task<DateTime> GetLastTimeAsync()
        {
            return await _historyService.GetLastTimeAsync();
        }
    }
}
