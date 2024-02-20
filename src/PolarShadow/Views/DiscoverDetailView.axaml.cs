using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.GestureRecognizers;
using PolarShadow.Controls;
using PolarShadow.ViewModels;
using System;

namespace PolarShadow.Views
{
    public partial class DiscoverDetailView : UserControl
    {
        public DiscoverDetailView()
        {
            InitializeComponent();
            
            Gestures.ScrollGestureEvent.AddClassHandler<DiscoverDetailView>(SwipeAttached.ScrollGestureHandler);
            Gestures.ScrollGestureEndedEvent.AddClassHandler<DiscoverDetailView>(SwipeAttached.ScrollGestureEndHandler);
            SwipeAttached.SwipedEvent.AddClassHandler<DiscoverDetailView>((s, e) => s.SwipedEvent(e), handledEventsToo: true);
        }

        public DiscoverDetailView(DiscoverDetailViewModel vm) : this()
        {
            this.DataContext = vm;
        }

        private DiscoverDetailViewModel VM => this.DataContext as DiscoverDetailViewModel;

        private async void SwipedEvent(SwipedEventArgs e)
        {
            if (e.Direction == SwipeDirection.BottomToTop)
            {
                await VM?.LoadMoreCommand.ExecuteAsync(default);
            }
        }
    }
}
