using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
            notify.Show(new Message() { Content = ex.Message, Expiration = ErrorExpiration, MessageType = MessageType.Error});
        }

        public static void ShowSuccess(this IMessageService manager) => Show(manager, "Success");

        public static Task<MessageResult> ShowDialog(this IMessageService manager, string message, MessageType type = MessageType.Information, DialogType dialogType = DialogType.OKCancel)
        {
            return manager.ShowDialog(new Message() { Content = message, Expiration = Expiration, Title = "", MessageType = type }, dialogType);
        }
    }
}
