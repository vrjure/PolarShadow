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

        private IMediaController _mediaController;
        public IMediaController MediaController
        {
            get => _mediaController;
            set
            {
                var c = _mediaController;
                if(SetProperty(ref _mediaController, value))
                {
                    if (c != null)
                    {
                        c.Controller.TimeChanged -= _controller_TimeChanged;
                        _mediaController.Controller.MediaChanged -= Controller_MediaChanged;
                        _mediaController.PreviousClick -= Controller_PreviousClick;
                        _mediaController.NextClick -= Controller_NextClick;
                        _mediaController.PropertyChanged -= MediaController_PropertyChanged;
                    }

                    if (_mediaController != null)
                    {
                        _mediaController.Controller.TimeChanged += _controller_TimeChanged;
                        _mediaController.Controller.MediaChanged += Controller_MediaChanged;
                        _mediaController.PreviousClick += Controller_PreviousClick;
                        _mediaController.NextClick += Controller_NextClick;
                        _mediaController.PropertyChanged += MediaController_PropertyChanged;
                    }
                }
            }
        }

        private IAsyncRelayCommand _refreshCommand;
        public IAsyncRelayCommand RefreshCommand => _refreshCommand ??= new AsyncRelayCommand(LoadOnline);

        private IAsyncRelayCommand _resourceOperateCommand;
        public IAsyncRelayCommand ResourceOperateCommand => _resourceOperateCommand ??= new AsyncRelayCommand(ResourceOperate);

        private IAsyncRelayCommand _linkClickCommand;
        public IAsyncRelayCommand LinkClickCommand => _linkClickCommand ??= new AsyncRelayCommand<ResourceTreeNode>(LinkClick);
    }
}
