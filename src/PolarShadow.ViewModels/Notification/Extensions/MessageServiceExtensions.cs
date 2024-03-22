using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Notification
{
    public static class MessageServiceExtensions
    {
        public static void Show(this IMessageService notify, Exception ex)
        {
            notify.Show(new Message() { Content = ex.Message});
        }
    }
}
