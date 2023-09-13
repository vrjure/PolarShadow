using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PolarShadow.Resources
{
    public interface ISearchHandler<TLink> : ISequentialRequest<PageResult<TLink>> where TLink : class, ILink
    {
        Task<PageResult<TLink>> SearchNextAsync(CancellationToken cancellation = default);
    }
}
