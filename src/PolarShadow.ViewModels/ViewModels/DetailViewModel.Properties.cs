using CommunityToolkit.Mvvm.Input;
using PolarShadow.Media;
using PolarShadow.Resources;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PolarShadow.ViewModels
{
    public partial class DetailViewModel
    {
        private ResourceTreeNode _resource;
        public ResourceTreeNode Resource
        {
            get => _resource;
            private set => SetProperty(ref _resource, value);
        }

        private ResourceTreeNode _currentHead;
        public ResourceTreeNode CurrentHead
        {
            get => _currentHead;
            set => SetProperty(ref _currentHead, value);
        }

        private ResourceTreeNode _currentEpisode;
        public ResourceTreeNode CurrentEpisode
        {
            get => _currentEpisode;
            set
            {
                if(SetProperty(ref _currentEpisode, value))
                {
                    EpisodeSelectedAsync(_currentEpisode).WithoutWait();
                }
            }
        }

        private int _currentEpisodeIndex = -1;
        public int CurrentEpisodeIndex
        {
            get => _currentEpisodeIndex;
            set => SetProperty(ref _currentEpisodeIndex, value);
        }

        private bool _isSave;
        public bool IsSaved
        {
            get => _isSave;
            private set => SetProperty(ref _isSave, value);
        }

        private HistoryModel _history;
        public HistoryModel History
        {
            get => _history;
            set => SetProperty(ref _history, value);
        }

        private ICollection<ResourceTreeNode> _sourceOptions;
        public ICollection<ResourceTreeNode> SourceOptions
        {
            get => _sourceOptions;
            set => SetProperty(ref _sourceOptions, value);
        }

        private ICollection<ISite> _webAnalysisSites;
        public ICollection<ISite> WebAnalysisSites
        {
            get => _webAnalysisSites;
            set => SetProperty(ref _webAnalysisSites, value);
        }

        private IVideoViewController _videoController;
        public IVideoViewController VideoController
        {
            get => _videoController;
            set
            {
                var c = _videoController;
                if(SetProperty(ref _videoController, value))
                {
                    if (c != null)
                    {
                        c.TimeChanged -= _controller_TimeChanged;
                        c.MediaChanged -= Controller_MediaChanged;
                        c.Ended -= Controller_Ended;
                    }

                    if (_videoController != null)
                    {
                        _videoController.TimeChanged += _controller_TimeChanged;
                        _videoController.MediaChanged += Controller_MediaChanged;
                        _videoController.Ended += Controller_Ended;
                    }
                }
            }
        }

        private bool _fullScreen;
        public bool FullScreen
        {
            get => _fullScreen;
            set => SetProperty(ref _fullScreen, value);
        }

        private ICommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand ??= new AsyncRelayCommand(LoadOnline);

        private ICommand _resourceOperateCommand;
        public ICommand ResourceOperateCommand => _resourceOperateCommand ??= new AsyncRelayCommand(ResourceOperate);

        private ICommand _linkClickCommand;
        public ICommand LinkClickCommand => _linkClickCommand ??= new AsyncRelayCommand<ResourceTreeNode>(EpisodeSelectedAsync);
        
        private ICommand _previousCommand;
        private ICommand PreviousCommand => _previousCommand ??= new AsyncRelayCommand(PreviousAsync);

        private ICommand _nextCommand;
        public ICommand NextCommand => _nextCommand ??= new AsyncRelayCommand(NextAsync);

        public ICommand _playPauseCommand;
        public ICommand PlayPauseCommand => _playPauseCommand ??= new AsyncRelayCommand(PlayPauseAsync);
    }
}
