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
        /// <summary>
        /// SrcName
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 站点名称
        /// </summary>
        public string Site { get; set; }
        /// <summary>
        /// 用于请求src的Request
        /// </summary>
        public string Request { get; set; }
    }
}
