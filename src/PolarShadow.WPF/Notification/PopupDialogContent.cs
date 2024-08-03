using PolarShadow.Essentials;
using PolarShadows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PolarShadow.Notification
{
    internal class PopupDialogContent : PopupContent
    {
        public static readonly DependencyProperty MessageResultProperty = DP.Register<PopupDialogContent, MessageResult>("MessageResult", MessageResult.Cancel);
        public MessageResult MessageResult
        {
            get => (MessageResult)GetValue(MessageResultProperty);
            set => SetValue(MessageResultProperty, value);
        }

        public static readonly DependencyProperty DialogTypeProperty = DP.Register<PopupDialogContent, DialogType>("DialogType", DialogType.None);
        public DialogType DialogType
        {
            get => (DialogType)GetValue(DialogTypeProperty);
            set => SetValue(DialogTypeProperty, value);
        }


        private Button _okButton;
        private Button _cancelButton;

        private Button OkButton
        {
            get => _okButton;
            set
            {
                if (_okButton != null)
                {
                    _okButton.Click -= okButton_Click;
                }
                _okButton = value;
                if (_okButton != null)
                {
                    _okButton.Click += okButton_Click;
                }
            }
        }

        private Button CancelButton
        {
            get => _cancelButton;
            set
            {
                if (_cancelButton != null)
                {
                    _cancelButton.Click -= cancelButton_Click;
                }
                _cancelButton = value;
                if (_cancelButton != null)
                {
                    _cancelButton.Click += cancelButton_Click;
                }
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            MessageResult = MessageResult.Cancel;
            TryClosePopup();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            MessageResult = MessageResult.OK;
            TryClosePopup();
        }

        private void TryClosePopup()
        {
            if (Parent is Popup popup)
            {
                popup.IsOpen = false;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            OkButton = this.Template.FindName("ButtonOk", this) as Button;
            CancelButton = this.Template.FindName("ButtonCancel", this) as Button;
        }
    }
}
