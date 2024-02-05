using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls
{
    public interface IVideoViewController : IDisposable, INotifyPropertyChanged
    {
        TimeSpan Length { get; }
        TimeSpan Time { get; set; }
        bool IsPlaying { get; }
        float Speed { get; set; }

        void Play();
        Task PlayAsync();
        void Play(Uri uri);
        Task PlayAsync(Uri uri);
        void Pause();
        Task PauseAsync();
        void Stop();
        Task StopAsync();

        event EventHandler<TimeSpan> LengthChanged;
        event EventHandler<TimeSpan> TimeChanged;
        event EventHandler Playing;
        event EventHandler Paused;
        event EventHandler Stopped;
        event EventHandler Error;
        event EventHandler Ended;
        event EventHandler MediaChanged;
        event EventHandler<float> Buffering;
    }
}
