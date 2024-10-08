﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using PolarShadow.Api.Utilities;
using PolarShadow.Resources;
using PolarShadow.Services;
using System.Net;

namespace PolarShadow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.Client)]
    public class SourceController : ControllerBase, ISourceService
    {
        private readonly ISourceService _sourceService;
        private readonly IOptions<PolarShadowSetting> _settings;
        public SourceController(ISourceService sourceService, IOptions<PolarShadowSetting> settings) 
        {
            _sourceService = sourceService;
            _settings = settings;
        }

        [HttpGet]
        public async Task<SourceModel> GetSouuceAsync()
        {
            return await _sourceService.GetSouuceAsync();
        }

        [HttpPost]
        public async Task SaveSourceAsync(SourceModel source)
        {
            await _sourceService.SaveSourceAsync(source);
        }

        [HttpPost("upload")]
        public async Task UploadAsync(ICollection<SourceModel> data)
        {
            await _sourceService.UploadAsync(data);
        }

        [HttpGet("download")]
        public async Task<ICollection<SourceModel>> DownloadAsync([FromQuery] DateTime lastTime)
        {
            return await _sourceService.DownloadAsync(lastTime);
        }

        [HttpGet("lastTime")]
        public async Task<DateTime> GetLastTimeAsync()
        {
            return await _sourceService.GetLastTimeAsync();
        }
    }
}
