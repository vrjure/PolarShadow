using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Services
{
    public interface IPreferenceService
    {
        void Set(PreferenceModel item);

        Task SetAsync(PreferenceModel item);

        PreferenceModel Get(string key);

        Task<PreferenceModel> GetAsync(string key);

        void Clear();

        Task ClearAsync();

        Task<ICollection<PreferenceModel>> GetAllAsync();
    }
}
