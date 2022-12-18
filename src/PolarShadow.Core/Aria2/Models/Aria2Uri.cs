using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core.Aria2
{
    public class Aria2Uri
    {
        /// <summary>
        /// URI
        /// </summary>
        public string Uri { get; set; }
        /// <summary>
        /// 'used' if the URI is in use. 
        /// 'waiting' if the URI is still waiting in the queue.
        /// </summary>
        public string Status { get; set; }
    }
}
