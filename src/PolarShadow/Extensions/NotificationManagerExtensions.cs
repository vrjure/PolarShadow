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
        public static TimeSpan Expiration = TimeSpan.FromSeconds(3);
        public static void Show(this INotificationManager manager, string message, NotificationType type = NotificationType.Information)
        {
            manager.Show(new Notification(default, message, type, Expiration));
        }

        public static void Show(this INotificationManager manager, Exception ex)
        {
            manager.Show(new Notification(default, ex.Message, NotificationType.Error, Expiration));
        }

        public static void ShowSuccess(this INotificationManager manager) => Show(manager, "Success");
    }
}
