using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Essentials
{
    public class FilePickType
    {
        public string Name { get; set; }
        public IReadOnlyList<string> Patterns { get; set; }
        public IReadOnlyList<string> MimeTypes { get; set; }
    }
}
