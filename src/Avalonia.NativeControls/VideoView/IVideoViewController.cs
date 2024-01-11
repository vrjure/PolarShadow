using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls
{
    public interface IVideoViewController : IDisposable
    {
        TimeSpan Length { get; }
        TimeSpan Time { get; set; }
        bool IsPlaying { get; }

        void Play();
        void Play(Uri uri);
        void Pause();
        void Stop();

        event EventHandler<TimeSpan> LengthChanged;
        event EventHandler<TimeSpan> TimeChanged;
        event EventHandler Playing;
        event EventHandler Paused;
    }
}
