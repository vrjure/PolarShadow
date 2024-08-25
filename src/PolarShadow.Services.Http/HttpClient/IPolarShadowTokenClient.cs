using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Services.Http
{
    public interface IPolarShadowTokenClient
    {
        Task<TokenModel?> GetTokenAsync();
    }
}
