using PolarShadows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PolarShadow.Controls
{
    [TemplatePart(Name ="PART_ClearButton", Type = typeof(Button))]
    internal class SearchBox : TextBox
    {
        public static readonly DependencyProperty SearchCommandProperty = DP.Register<SearchBox, ICommand>("SearchCommand");
        public ICommand SearchCommand
        {
            get => (ICommand)GetValue(SearchCommandProperty);
            set => SetValue(SearchCommandProperty, value);
        }

        public static readonly DependencyProperty HasTextProperty = DP.Register<SearchBox, bool>("HasText");
        public bool HasText
        {
            get => (bool)GetValue(HasTextProperty);
            protected set => SetValue(HasTextProperty, value);
        }

        private Button _clearButton;
        private Button ClearButton
        {
            get => _clearButton;
            set
            {
                if (_clearButton != null)
                {
                    _clearButton.Click -= ClearButton_Click;
                }

                _clearButton = value;
                if (_clearButton != null)
                {
                    _clearButton.Click += ClearButton_Click;
                }
            }
        }

        public SearchBox()
        {
            
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ClearButton = this.Template.FindName("Part_ClearButton", this) as Button;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.Key == Key.Enter)
            {
                ExecuteCommand();
            }
        }

        private void ExecuteCommand()
        {
            if (SearchCommand == null || !SearchCommand.CanExecute(Text)) return;
            SearchCommand.Execute(Text);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            this.Text = string.Empty;
        }
    }
}
