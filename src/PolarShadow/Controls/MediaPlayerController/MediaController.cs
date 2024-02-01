using Avalonia.Controls;
using Avalonia.Controls.Documents;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    internal class MediaController : ObservableObject, IMediaController
    {
        private IVideoViewController _controller;
        public IVideoViewController Controller
        {
            get => _controller;
            set => SetProperty(ref _controller, value);
        }

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private bool _fullScreen;
        public bool FullScreen
        {
            get => _fullScreen;
            set => SetProperty(ref _fullScreen, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private MediaMode _mediaMode = MediaMode.Simple;
        public MediaMode MediaMode
        {
            get => _mediaMode;
            set => SetProperty(ref _mediaMode, value);
        }

        private string _tip;
        public string Tip
        {
            get => _tip;
            set => SetProperty(ref _tip, value);
        }

        public event EventHandler PreviousClick;
        public event EventHandler NextClick;
        
        internal void OnPreviousClick()
        {
            this.PreviousClick?.Invoke(this, EventArgs.Empty);
        }

        internal void OnNextClick()
        {
            this.NextClick?.Invoke(this, EventArgs.Empty);
        }
    }
}
