using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Notification
{
    public interface IMessageService
    {
        void Show(IMessage message);
    }
}
