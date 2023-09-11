using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Aria2
{
    public static class Aria2ResponseExtensions
    {
        public static bool IsError(this Aria2Response response)
        {
            return response == null || response.Error != null;
        }

        public static bool IsOk(this Aria2Response<string> response)
        {
            return response == null ? false : response.Result== null ? false: response.Result.Equals("OK", StringComparison.OrdinalIgnoreCase);
        }
    }
}
