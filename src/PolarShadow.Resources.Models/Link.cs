using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Resources
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

        public string Name { get; set; }

        public string Site { get; set; }

        public string Request { get; set; }

        public string FromRequest { get; set; }

    }
}
