using PolarShadow.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Services
{
    public interface ISourceService
    {
        Task<SourceVersionModel> GetSourceViersionAsync();
        Task UploadAsync();
        Task<Stream> DownloadAsync();
    }
}
