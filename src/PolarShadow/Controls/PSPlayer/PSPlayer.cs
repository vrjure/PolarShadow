using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    public class PSPlayer : TemplatedControl
    {
        public static readonly DirectProperty<PSPlayer, IVideoViewController> ControllerProperty = MediaPlayerController.ControllerProperty.AddOwner<PSPlayer>(s=>s.Controller, (s, v) => s.Controller = v);
        private IVideoViewController _controller;
        public IVideoViewController Controller
        {
            get => _controller;
            set => SetAndRaise(ControllerProperty, ref _controller, value);
        }

        public static readonly DirectProperty<PSPlayer, string> TitleProperty = MediaPlayerController.TitleProperty.AddOwner<PSPlayer>(s => s.Title, (s, v)=> s.Title = v);
        private string _title;
        public string Title
        {
            get => _title;
            set => SetAndRaise(TitleProperty, ref _title, value);
        }

        public static readonly DirectProperty<PSPlayer, bool> FullScreenProperty = MediaPlayerController.FullScreenProperty.AddOwner<PSPlayer>(s=> s.FullScreen, (s, v)=> s.FullScreen = v);
        private bool _fullScreen;
        public bool FullScreen
        {
            get => _fullScreen;
            set => SetAndRaise(FullScreenProperty, ref _fullScreen, value);
        }

        public static readonly DirectProperty<PSPlayer, MediaPlayerMode> PlayerModeProperty = MediaPlayerController.PlayerModeProperty.AddOwner<PSPlayer>(s => s.PlayerMode, (s, v) => s.PlayerMode = v);

        private MediaPlayerMode _playerMode;
        public MediaPlayerMode PlayerMode
        {
            get => _playerMode;
            set => SetAndRaise(PlayerModeProperty, ref _playerMode, value);
        }
    }
}
