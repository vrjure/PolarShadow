using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Essentials
{
    public interface IStorageItem
    {
        string Name { get; }
        Uri Uri { get; }
        Task DeleteAsync();
        Task<IStorageItem> MoveAsync(IStorageFolder destination);
    }
}
