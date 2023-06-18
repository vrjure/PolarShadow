using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Cache
{
    internal interface IHttpResource
    {
        Task<string> CreateObjectUrlAsync(string orign);
        Task RevokeObjectUrlAsync(string objectUrl);
    }
}
