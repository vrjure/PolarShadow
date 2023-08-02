using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public interface ILink
    {
        string Src { get; set; }
        LinkType SrcType { get; set; }
        string Site { get; set; }
        string Name { get; set; }
    }
}
