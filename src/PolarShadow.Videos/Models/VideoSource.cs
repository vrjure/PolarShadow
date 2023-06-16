using System;
using System.Collections.Generic;
using System.Text;
using PolarShadow.Core;

namespace PolarShadow.Videos
{
    public class VideoSource
    {
        public string Src { get; set; }
        private LinkType _srcType;
        public LinkType SrcType
        {
            get
            {
                if (_srcType == LinkType.None && !string.IsNullOrEmpty(Src))
                {
                    _srcType = Src.GetLinkType();
                }

                return _srcType;
            }
            set { _srcType = value; }
        }
    }
}
