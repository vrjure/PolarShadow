using Avalonia.Controls;
using Avalonia.Controls.Selection;
using CommunityToolkit.Mvvm.Input;
using PolarShadow.Controls;
using PolarShadow.Resources;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private ISelectionModel _headerSelection;
        public ISelectionModel HeaderSelection
        {
            get => _headerSelection;
            set => SetProperty(ref _headerSelection, value);
        }

        private ISelectionModel _analysisSelection;
        public ISelectionModel AnalysisSelection
        {
            get => _analysisSelection;
            set
            {
                var cache = _analysisSelection;
                if (SetProperty(ref _analysisSelection, value))
                {
                    if (cache != null)
                    {
                        cache.SelectionChanged -= AnalysisSelection_SelectionChanged;
                    }
                    if (_analysisSelection != null)
                    {
                        _analysisSelection.SelectionChanged += AnalysisSelection_SelectionChanged;
                    }
                }
            }
        }

        private ISelectionModel _sourceSelection;
        public ISelectionModel SourceSelection
        {
            get => _sourceSelection;
            set
            {
                var cache = _sourceSelection;
                if (SetProperty(ref _sourceSelection, value))
                {
                    if (cache != null)
                    {
                        cache.SelectionChanged -= SourceSelection_SelectionChanged;
                    }
                    if (_sourceSelection != null)
                    {
                        _sourceSelection.SelectionChanged += SourceSelection_SelectionChanged;
                    }
                }
            }
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

        private bool _fullScreen;
        public bool FullScreen
        {
            get => _fullScreen;
            set
            {
                if (SetProperty(ref _fullScreen, value))
                {
                    if (_fullScreen)
                    {
                        SetFullScreen();
                        PlayerMode = MediaPlayerMode.Normal;
                    }
                    else
                    {
                        ExitFullScreen();
                        PlayerMode = MediaPlayerMode.Simple;
                    }
                }
            }
        }

        private IVideoViewController _controller;
        public IVideoViewController Controller
        {
            get => _controller;
            set
            {
                var cache = _controller;
                if (SetProperty(ref _controller, value))
                {
                    if (cache != null)
                    {
                        cache.TimeChanged -= _controller_TimeChanged;
                    }
                    if (_controller != null)
                    {
                        _controller.TimeChanged += _controller_TimeChanged;
                    }
                }
            }

        }

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private MediaPlayerMode _playerMode = MediaPlayerMode.Simple;
        public MediaPlayerMode PlayerMode
        {
            get => _playerMode;
            set => SetProperty(ref _playerMode, value);
        }

        private IAsyncRelayCommand _refreshCommand;
        public IAsyncRelayCommand RefreshCommand => _refreshCommand ??= new AsyncRelayCommand(LoadOnline);

        private IAsyncRelayCommand _resourceOperateCommand;
        public IAsyncRelayCommand ResourceOperateCommand => _resourceOperateCommand ??= new AsyncRelayCommand(ResourceOperate);

        private IAsyncRelayCommand _linkClickCommand;
        public IAsyncRelayCommand LinkClickCommand => _linkClickCommand ??= new AsyncRelayCommand<ResourceTreeNode>(LinkClick);

        private IRelayCommand _previousCommand;
        public IRelayCommand PreviousCommand => _previousCommand ??= new RelayCommand(PlayPrevious);
        private IRelayCommand _nextCommand;
        public IRelayCommand NextCommand => _nextCommand ??= new RelayCommand(PlayNext);
    }
}
