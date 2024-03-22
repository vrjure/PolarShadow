using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Resources
{
    /// <summary>
    /// 表示一个资源的链接
    /// </summary>
    public interface ILink
    {
        /// <summary>
        /// 链接的名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 链接的地址
        /// </summary>
        string Src { get; set; }
        /// <summary>
        /// 链接的类型
        /// </summary>
        LinkType SrcType { get; set; }
        /// <summary>
        /// 链接所属站点名称
        /// </summary>
        string Site { get; set; }
        /// <summary>
        /// 获取链接内容要使用的请求
        /// </summary>
        string Request { get; set; }
        /// <summary>
        /// 此链接来自的请求
        /// </summary>
        string FromRequest { get; set; }
    }
}
