using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Notification
{
    public interface IMessage
    {
        string Title { get; set; }
        object Content { get; set; }
        TimeSpan Timeout { get; set; }
        Action OnClosed { get; set; }
    }
}
