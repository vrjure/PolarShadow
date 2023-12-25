using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Services
{
    public interface IHistoryService
    {
        Task<ICollection<HistoryModel>> GetListPageAsync(int page, int pageSize, string filter = default);
        Task DeleteAsync(int id);
        Task AddOrUpdateAsync(HistoryModel model);
        Task<HistoryModel> GetByIdAsync(int id);
        Task<HistoryModel> GetByResourceIdAsync(int resourceId);
    }
}
