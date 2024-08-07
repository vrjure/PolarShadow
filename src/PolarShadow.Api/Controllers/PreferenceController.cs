using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PolarShadow.Services;

namespace PolarShadow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreferenceController : ControllerBase, IPreferenceService
    {
        private readonly IPreferenceService _preferenceService;
        public PreferenceController(IPreferenceService preferenceService)
        {
            _preferenceService = preferenceService;
        }

        [HttpDelete("clear")]
        public async Task ClearAsync()
        {
            await _preferenceService.ClearAsync();
        }

        [HttpGet]
        public async Task<ICollection<PreferenceModel>> GetAllAsync()
        {
            return await _preferenceService.GetAllAsync();
        }

        [HttpGet("/{key}")]
        public async Task<PreferenceModel> GetAsync(string key)
        {
            return await _preferenceService.GetAsync(key);
        }

        [HttpPost]
        public async Task SetAsync(PreferenceModel item)
        {
            await _preferenceService.SetAsync(item);
        }

        [NonAction]
        public void Set(PreferenceModel item)
        {
            throw new NotImplementedException();
        }

        [NonAction]
        public PreferenceModel Get(string key)
        {
            throw new NotImplementedException();
        }

        [NonAction]
        public void Clear()
        {
            throw new NotImplementedException();
        }
    }
}
