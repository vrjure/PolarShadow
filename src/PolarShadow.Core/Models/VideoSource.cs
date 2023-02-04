using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public class VideoSource
    {
        public string Src { get; set; }
        private SrcType _srcType;
        public SrcType SrcType
        {
            get
            {
                if (_srcType == SrcType.None && !string.IsNullOrEmpty(Src))
                {
                    _srcType = Src.GetVideoSourceType();
                }

                return _srcType;
            }
            set { _srcType = value; }
        }
    }
}
