using PolarShadow.ViewModels.Notification;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Notification
{
    public static class MessageServiceExtensions
    {
        public static TimeSpan Expiration = TimeSpan.FromSeconds(2);
        public static TimeSpan ErrorExpiration = TimeSpan.FromSeconds(10);
        public static void Show(this IMessageService manager, string message, MessageType type = MessageType.Information)
        {
            manager.Show(new Message() { Content = message, Expiration = Expiration, Title = "", MessageType = type});
        }

        public static void Show(this IMessageService notify, Exception ex)
        {
            notify.Show(new Message() { Content = ex.Message});
        }

        public static void ShowSuccess(this IMessageService manager) => Show(manager, "Success");
    }
}
