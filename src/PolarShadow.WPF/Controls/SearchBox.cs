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
    internal class SearchBox : TextBox
    {
        public static readonly DependencyProperty SearchCommandProperty = DP.Register<SearchBox, ICommand>("SearchCommand");
        public ICommand SearchCommand
        {
            get => (ICommand)GetValue(SearchCommandProperty);
            set => SetValue(SearchCommandProperty, value);
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
    }
}
