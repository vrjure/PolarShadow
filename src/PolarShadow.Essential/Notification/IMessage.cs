using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Essentials
{
    public interface IMessage
    {
        string Title { get; set; }
        object Content { get; set; }
        MessageType MessageType { get; set; }
        TimeSpan Expiration { get; set; }
        Action OnClosed { get; set; }
    }
}
