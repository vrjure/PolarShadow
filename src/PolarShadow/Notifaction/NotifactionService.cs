using Avalonia.Controls.Notifications;
using PolarShadow.Essentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Notifaction
{
    internal class NotificationService : IMessageService
    {
        private readonly ITopLevelService _topLevelService;
        private WindowNotificationManager _notificationManager;
        public NotificationService(ITopLevelService topLevelService)
        {
            _topLevelService = topLevelService;
        }

        public void Show(IMessage message)
        {
            EnsureNotification(message);
        }

        public Task<MessageResult> ShowDialog(IMessage message, DialogType dialogType)
        {
            throw new NotImplementedException();
        }

        private void EnsureNotification(IMessage message)
        {
            if (_notificationManager == null)
            {
                var topLevel = _topLevelService.GetTopLevel();
                _notificationManager = new WindowNotificationManager(topLevel);
                _notificationManager.TemplateApplied += (s, e) =>
                {
                    _notificationManager.Show(new Notification(message.Title, message.Content.ToString(), GetNotificationType(message.MessageType), message.Expiration, null, message.OnClosed));
                };
            }
            else
            {
                _notificationManager.Show(new Notification(message.Title, message.Content.ToString(), GetNotificationType(message.MessageType), message.Expiration, null, message.OnClosed));


            }
        }

        private NotificationType GetNotificationType(MessageType type)
        {
            return type switch
            {
                MessageType.Error => NotificationType.Error,
                MessageType.Warning => NotificationType.Warning,
                MessageType.Information => NotificationType.Information,
                MessageType.Success => NotificationType.Success,
                _ => NotificationType.Information
            };
        }
    }
}
