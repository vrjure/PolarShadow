using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Videos
{
    public class VideoSource
    {
        public string Src { get; set; }
        private VideoSrcType _srcType;
        public VideoSrcType SrcType
        {
            get
            {
                if (_srcType == VideoSrcType.None && !string.IsNullOrEmpty(Src))
                {
                    _srcType = Src.GetVideoSourceType();
                }

                return _srcType;
            }
            set { _srcType = value; }
        }
    }
}
