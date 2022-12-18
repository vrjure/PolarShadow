using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PolarShadow.Core.Aria2
{
    public interface IAria2Client
    {
        public Uri Uri { get; }
        public string Secret { get; }
        public string Id { get; }
    }
}
