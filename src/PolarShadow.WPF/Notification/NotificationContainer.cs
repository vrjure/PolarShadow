using PolarShadow.Essentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Effects;
using System.Windows.Threading;

namespace PolarShadow.Notification
{
    internal class NotificationContainer : IMessageService
    {
        private Panel _container;
        public void Initialize(Panel container)
        {
            _container = container;
        }
        public async void Show(IMessage message)
        {
            var popup = CreatePopup(message, false);
            popup.Tag = message;
            popup.IsOpen = true;

            if (message.Expiration > TimeSpan.Zero)
            {
                await Task.Delay(message.Expiration);
                popup.IsOpen = false;
            }

        }

        public async Task<MessageResult> ShowDialog(IMessage message, DialogType dialogType)
        {
            var popup = CreatePopup(message, true, dialogType);

            var closeCts = new TaskCompletionSource();
            popup.Tag = new KeyValuePair<IMessage, TaskCompletionSource>(message, closeCts);

            popup.IsOpen = true;

            await closeCts.Task;
            return GetResult(popup);
        }

        private void Popup_Closed(object sender, EventArgs e)
        {
            var tag = (sender as Popup).Tag;
            if (tag is IMessage message)
            {
                message.OnClosed?.Invoke();
            }
            else if (tag is KeyValuePair<IMessage, TaskCompletionSource> kv)
            {
                kv.Key.OnClosed?.Invoke();
                kv.Value.TrySetResult();
            }
        }

        private Popup CreatePopup(IMessage message, bool isDialog, DialogType dialogType = DialogType.None)
        {
            var popupContent = isDialog ? new PopupDialogContent() { DialogType = dialogType} : new PopupContent();
            popupContent.MessageType = message.MessageType;
            popupContent.Title = message.Title;
            popupContent.Content = message.Content;
            
            var popup = new Popup();
            popup.Child = popupContent;
            popup.StaysOpen = isDialog;
            popup.AllowsTransparency = true;
            popup.PlacementTarget = _container;
            if (isDialog)
            {
                SetDialog(popup);
            }
            else
            {
                SetMessage(popup);
            }

            popup.Closed += Popup_Closed;

            return popup;
        }

        private void SetMessage(Popup popup)
        {
            popup.Placement = PlacementMode.Custom;
            popup.PopupAnimation = PopupAnimation.Scroll;
            popup.HorizontalOffset = -5;
            popup.VerticalOffset = -5;
            popup.CustomPopupPlacementCallback = CustomPopupPlacementCallback;
            popup.AddHandler(Popup.MouseUpEvent, new RoutedEventHandler(PopupClick));
        }

        private void SetDialog(Popup popup)
        {
            popup.Placement = PlacementMode.Center;
            popup.PopupAnimation = PopupAnimation.Fade;
        }

        private MessageResult GetResult(Popup popup)
        {
            return (popup.Child as PopupDialogContent).MessageResult;
        }

        private void PopupClick(object sender, RoutedEventArgs e)
        {
            (sender as Popup).IsOpen = false;
        }

        private CustomPopupPlacement[] CustomPopupPlacementCallback(Size popupSize, Size targetSize, Point offset)
        {
            return
            [
                new CustomPopupPlacement(new Point(targetSize.Width - popupSize.Width + offset.X, targetSize.Height - popupSize.Height + offset.Y), PopupPrimaryAxis.Horizontal)
            ];
        }
    }
}
