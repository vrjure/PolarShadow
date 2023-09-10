using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PolarShadow.Core
{
    public interface ISearchHandler : ISequentialRequest<PageResult<Resource>>
    {
        Task<PageResult<Resource>> SearchNextAsync(CancellationToken cancellation = default);
    }
}
