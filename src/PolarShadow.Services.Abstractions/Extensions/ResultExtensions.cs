using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Services
{
    public static class ResultExtensions
    {
        public static bool IsSuccess(this Result result)
        {
            return result != null && result.Code == 0;
        }

        public static void ThrowIfUnsuccessful(this Result result)
        {
            if (!result.IsSuccess())
            {
                throw new ResultException(result.Code, result.Message);
            }
        }
    }
}
