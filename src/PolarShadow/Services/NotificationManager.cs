using Avalonia.Controls.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Services
{
    internal class NotificationManager : INotificationManager
    {
        private readonly ITopLevelService _topLevelService;
        private WindowNotificationManager _notificationManager;
        public NotificationManager(ITopLevelService topLevelService)
        {
            _topLevelService = topLevelService;
        }
        
        public void Show(INotification notification)
        {
            if (_notificationManager == null)
            {
                var topLevel = _topLevelService.GetTopLevel();
                _notificationManager = new WindowNotificationManager(topLevel);
                _notificationManager.Position = NotificationPosition.BottomRight;
                _notificationManager.TemplateApplied += (sender, arg) =>
                {
                    _notificationManager.Show(notification);
                };
            }
            else
            {
                _notificationManager.Show(notification);
            }
        }
    }
}
