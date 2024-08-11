using PolarShadow.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Services
{
    public interface ISourceService : ISyncAble<SourceModel>
    {
        Task<SourceModel> GetSouuceAsync();
        Task SaveSourceAsync(SourceModel source);
    }
}
