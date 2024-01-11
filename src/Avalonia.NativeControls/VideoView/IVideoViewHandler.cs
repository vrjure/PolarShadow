using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls
{
    public interface IVideoViewHandler : IViewHandler
    {
        new IPlatformVideoView PlatformView { get; }
    }
}
