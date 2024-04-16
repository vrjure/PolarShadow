using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Essentials
{
    public sealed class SHA
    {
        public static string SHA256(string url, string format = "x2")
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var buffer = sha.ComputeHash(Encoding.UTF8.GetBytes(url));
            var sb = new StringBuilder();

            foreach (var item in buffer)
            {
                sb.Append(item.ToString(format));
            }
            return sb.ToString();
        }
    }
}
