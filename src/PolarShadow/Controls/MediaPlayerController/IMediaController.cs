using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    public interface IMediaController : INotifyPropertyChanged
    {
        IVideoViewController Controller { get; set; }
        string Title { get; set; }
        bool FullScreen { get; set; }
        bool IsLoading { get; set; }
        MediaMode MediaMode { get; set; }
        string Tip { get; set; }
        event EventHandler PreviousClick;
        event EventHandler NextClick;
    }
}
