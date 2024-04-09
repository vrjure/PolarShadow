using PolarShadows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PolarShadow.Notification
{
    internal class PopupContent : ContentControl
    {
        public static readonly DependencyProperty MessageTypeProperty = DP.Register<PopupContent, MessageType>("MessageType", MessageType.Information);
        public MessageType MessageType
        {
            get => (MessageType)GetValue(MessageTypeProperty);
            set => SetValue(MessageTypeProperty, value);
        }

        public static readonly DependencyProperty TitleProperty = DP.Register<PopupContent, string>("Title");
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
    }
}
