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
    public class SourceController : ControllerBase, ISourceService
    {
        private readonly ISourceService _sourceService;
        private readonly IOptions<PolarShadowSetting> _settings;
        private readonly FileSafeOperate _fileSafeOperate;
        public SourceController(ISourceService sourceService, IOptions<PolarShadowSetting> settings, FileSafeOperate fileSafeOperate) 
        {
            _sourceService = sourceService;
            _settings = settings;
            _fileSafeOperate = fileSafeOperate;
        }

        [HttpGet("download")]
        public IActionResult DownloadFileAsync()
        {
            if (string.IsNullOrEmpty(_settings.Value.SourceFileName) || string.IsNullOrEmpty(_settings.Value.SourceFileFolder))
            {
                throw new ResultException(ResultCode.ServerConfigError, "File not existed");
            }
            var path = Path.Combine(_settings.Value.SourceFileFolder, _settings.Value.SourceFileName);
            if (!System.IO.File.Exists(path))
            {
                throw new ResultException(ResultCode.ServerConfigError, "File not existed");
            }
            var fileName =Path.GetFileName(path);
            try
            {
                var stream = _fileSafeOperate.FileRead();
                return File(stream, "application/json", fileName);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [NonAction]
        public Task<Stream> DownloadAsync()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public async Task<SourceVersionModel> GetSourceViersionAsync()
        {
            return await _sourceService.GetSourceViersionAsync();
        }

        [HttpPost("upload")]
        public async Task UploadAsync()
        {
            if (string.IsNullOrEmpty(_settings.Value.SourceFileFolder) || string.IsNullOrEmpty(_settings.Value.SourceFileName))
            {
                throw new ResultException(ResultCode.ServerConfigError, "File folder or file name not config");
            }
            if (string.IsNullOrEmpty(Request.ContentType) || Request.ContentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) < 0)
            {
                throw new ResultException(ResultCode.ParameterError, "ContentType is not correct");
            }

            var contentType = MediaTypeHeaderValue.Parse(Request.ContentType);
            var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;
            var reader = new MultipartReader(boundary!, Request.Body);
            var sectioin = await reader.ReadNextSectionAsync();

            _fileSafeOperate.Increment();

            try
            {
                while (sectioin != null)
                {
                    if (ContentDispositionHeaderValue.TryParse(sectioin.ContentDisposition, out var contentDisposition))
                    {
                        if (!HasFileContentDisposition(contentDisposition))
                        {
                            throw new ResultException(ResultCode.ParameterError, "ContentDisposition error");
                        }
                        else
                        {
                            var fileName = WebUtility.HtmlEncode(contentDisposition.FileName.Value);

                            using var fileStream = _fileSafeOperate.CreateFileStream();
                            sectioin.Body.CopyTo(fileStream);
                            await fileStream.FlushAsync();
                        }
                    }

                }
            }
            finally
            {
                _fileSafeOperate.Decrement(); 
            }

        }

        private static bool HasFileContentDisposition(ContentDispositionHeaderValue contentDisposition)
        {
            // Content-Disposition: form-data; name="myfile1"; filename="Misc 002.jpg"
            return contentDisposition != null
                && contentDisposition.DispositionType.Equals("form-data")
                && (!string.IsNullOrEmpty(contentDisposition.FileName.Value)
                    || !string.IsNullOrEmpty(contentDisposition.FileNameStar.Value));
        }
    }
}
