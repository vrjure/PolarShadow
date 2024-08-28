using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Services
{
    public interface IHistoryService : ISyncAble<HistoryModel>
    {
        Task<ICollection<HistoryModel>> GetListPageAsync(int page, int pageSize, string filter = default);
        Task DeleteAsync(long id);
        Task AddOrUpdateAsync(HistoryModel model);
        Task<HistoryModel> GetByIdAsync(long id);
        Task<HistoryModel> GetByResourceNameAsync(string resourceName);
    }
}
