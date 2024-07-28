using PolarShadows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace PolarShadow.Controls
{
    [TemplatePart(Name = "PART_Up",Type = typeof(ButtonBase))]
    [TemplatePart(Name = "PART_Down", Type = typeof(ButtonBase))]
    class NumericUpDown : TextBox
    {
        private ButtonBase _upButton;
        private ButtonBase _downButton;
        public ButtonBase UpButton
        {
            get => _upButton;
            set
            {
                if (_upButton != null)
                {
                    _upButton.Click -= UpButton_Click;
                }
                _upButton = value;
                if (_upButton != null)
                {
                    _upButton.Click += UpButton_Click;
                }
            }
        }

        public ButtonBase DownButton
        {
            get => _downButton;
            set
            {
                if (_downButton != null)
                {
                    _downButton.Click -= DownButton_Click;
                }

                _downButton = value;

                if (_downButton != null)
                {
                    _downButton.Click += DownButton_Click;
                }
            }
        }

        public static readonly DependencyProperty MaximumProperty = DP.Register<NumericUpDown, int>(nameof(Maximum));
        public int Maximum
        {
            get => (int)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public static readonly DependencyProperty MinimumProperty = DP.Register<NumericUpDown, int>(nameof(Mininum));
        public int Mininum
        {
            get => (int)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public static readonly DependencyProperty ValueProperty = DP.Register<NumericUpDown, int>(nameof(Value), PropertyChanged);
        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public NumericUpDown()
        {
            BindingBase();
        }

        private void BindingBase()
        {
            var binding = new Binding("Value");
            binding.Source = this;
            binding.Mode = BindingMode.TwoWay;
            binding.Converter = NumericStringConverter.Instance;
            this.SetBinding(TextProperty, binding);

        }

        private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not NumericUpDown) return;
            
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            if (Value > Mininum)
            {
                Value--;
            }
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            if (Value < Maximum)
            {
                Value++;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpButton = (ButtonBase)GetTemplateChild("PART_Up");
            DownButton = (ButtonBase)GetTemplateChild("PART_Down");
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.IsDown && e.Key == Key.Up && Value < Maximum)
            {
                Value++;
            }
            else if (e.IsDown && e.Key == Key.Down && Value > Mininum)
            {
                Value--;
            }
            else if (e.IsDown && ((int)e.Key >= (int)Key.D0 && (int)e.Key <= (int)Key.D9
                || (int)e.Key >= (int)Key.NumPad0 && (int)e.Key <= (int)Key.NumPad9)
                || e.Key == Key.Back)
            {

            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
