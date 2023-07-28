using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public class Link : ILink
    {
        private LinkType _linkType;
        public string Src { get; set; }
        public LinkType SrcType
        {
            get
            {
                if (_linkType == LinkType.None && !string.IsNullOrEmpty(Src))
                {
                    _linkType = Src.GetLinkType();
                }

                return _linkType;
            }
            set { _linkType = value; }  
        }

        public string Site { get; set; }
    }
}
