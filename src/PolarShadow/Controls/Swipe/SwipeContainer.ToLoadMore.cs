using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PolarShadow.Controls
{
    public partial class SwipeContainer
    {
        private static readonly double _minValidLoadMoreSwipeDistance = 150;

        private TaskCompletionSource _loadMoreTCS;

        public static readonly StyledProperty<bool> CanLoadMoreProperty = AvaloniaProperty.Register<SwipeContainer, bool>(nameof(CanLoadMore));
        public bool CanLoadMore
        {
            get => GetValue(CanLoadMoreProperty);
            set => SetValue(CanLoadMoreProperty, value);
        }

        public static readonly StyledProperty<bool> LoadingProperty = AvaloniaProperty.Register<SwipeContainer, bool>(nameof(Loading));
        public bool Loading
        {
            get => GetValue(LoadingProperty);
            set => SetValue(LoadingProperty, value);
        }

        public static readonly StyledProperty<ICommand> LoadMoreCommandProperty = AvaloniaProperty.Register<SwipeContainer, ICommand>(nameof(LoadMoreCommand));
        public ICommand LoadMoreCommand
        {
            get => GetValue(LoadMoreCommandProperty);
            set => SetValue(LoadMoreCommandProperty, value);
        }

        private async void LoadingPropertyChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var val = e.GetOldAndNewValue<bool>();
            if (val.newValue)
            {
                if (_loadMoreTCS != null)
                {
                    return;
                }
                LoadingState();
                _loadMoreTCS = new TaskCompletionSource();
                try
                {
                    await _loadMoreTCS.Task;
                }
                catch { }
                finally
                {
                    _loadMoreTCS = null;
                }
            }
            else
            {
                if (_loadMoreTCS != null)
                {
                    _loadMoreTCS.TrySetCanceled();
                    _loadMoreTCS = null;
                }

                if (CanLoadMore)
                {
                    LoadMoreState();
                }
                else
                {
                    NoMoreState();
                }
            }
        }

        private void CanLoadMorePropertyChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (Loading)
            {
                return;
            }

            var val = e.GetOldAndNewValue<bool>();

            if (val.newValue)
            {
                LoadMoreState();
            }
            else
            {
                NoMoreState();
            }
        }

        private void BottomToTopEnd(double y)
        {
            if (!CanLoadMore || Math.Abs(y) < _minValidLoadMoreSwipeDistance)
            {
                return;
            }

            Loading = true;
        }

        private void LoadMoreState()
        {
            PseudoClasses.Set(":reset", false);
            PseudoClasses.Set(":normal", true);
            PseudoClasses.Set(":loadMore", true);
            PseudoClasses.Set(":loading", false);
            PseudoClasses.Set(":noMore", false);
        }

        private void LoadingState()
        {
            PseudoClasses.Set(":reset", false);
            PseudoClasses.Set(":normal", true);
            PseudoClasses.Set(":loadMore", false);
            PseudoClasses.Set(":loading", true);
            PseudoClasses.Set(":noMore", false);
        }

        private void NoMoreState()
        {
            PseudoClasses.Set(":reset", false);
            PseudoClasses.Set(":normal", true);
            PseudoClasses.Set(":loadMore", false);
            PseudoClasses.Set(":loading", false);
            PseudoClasses.Set(":noMore", true);
        }
    }
}
