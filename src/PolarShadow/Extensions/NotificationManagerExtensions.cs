using Avalonia.Controls.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls.Notifications
{
    public static class NotificationManagerExtensions
    {
        public static TimeSpan Expiration = TimeSpan.FromSeconds(2);
        public static TimeSpan ErrorExpiration = TimeSpan.FromSeconds(10);
        public static void Show(this INotificationManager manager, string message, NotificationType type = NotificationType.Information)
        {
            manager.Show(new Notification(default, message, type, Expiration));
        }

        public static void Show(this INotificationManager manager, Exception ex)
        {
            manager.Show(new Notification(default, ex.Message, NotificationType.Error, ErrorExpiration));
        }

        public static void ShowSuccess(this INotificationManager manager) => Show(manager, "Success");
    }
}
