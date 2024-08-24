using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Services
{
    public class TokenModel
    {
        public string AccessToken { get; set; }
        public long Expires { get; set; }
    }
}
