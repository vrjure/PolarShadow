using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace PolarShadow
{
    internal static class HasHelper
    {
        public static string Hash(this string src)
        {
            if (string.IsNullOrEmpty(src))
            {
                return src;
            }

            var buffer = Encoding.UTF8.GetBytes(src);
            using var sha256 = SHA256.Create();
            var result = sha256.ComputeHash(buffer);
            var sb = new StringBuilder();
            foreach (var item in result)
            {
                sb.Append(item.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
