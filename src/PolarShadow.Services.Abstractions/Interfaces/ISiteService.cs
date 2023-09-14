using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Services
{
    public interface ISiteService
    {
        Task<ICollection<SiteModel>> GetSitesAsync();
        Task<ICollection<RequestModel>> GetRequesetsAsync(string siteName);
        Task<ICollection<SiteInfoModel>> GetSiteInfoAsync();
        Task SaveAsync(IEnumerable<SiteInfoModel> sites);
        Task SaveAsync(SiteInfoModel site);
        Task DeleteSiteAsync(string site);
        Task DeleteRequestAsync(string requestName);
    }
}
