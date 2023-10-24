using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using PolarShadow.Core;

namespace PolarShadow.Resources
{
    public interface ISearchHandler<TLink> where TLink : class, ILink
    {
        Task<ICollection<TLink>> SearchNextAsync(CancellationToken cancellation = default);
    }
}
