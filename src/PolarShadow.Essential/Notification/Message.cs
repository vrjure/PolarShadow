using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Essentials
{
    public class Message : IMessage
    {
        public string Title { get; set; }
        public object Content { get; set; }
        public TimeSpan Expiration { get; set; }
        public Action OnClosed { get; set; }
        public MessageType MessageType { get; set; }
    }
}
