using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Services
{
    public interface ITopLevelService : INotifyPropertyChanged
    {
        void SetTopLevel(TopLevel topLevel);
        void SetTopLevelFactory(Func<Visual> factory);
        TopLevel GetTopLevel();
        bool FullScreen { get; set; }
    }
}
