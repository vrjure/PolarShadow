using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Provider;
using Android.Views;
using PolarShadow.Essentials;

namespace PolarShadow.Android.Essentials
{
    internal class DeviceService : IDeviceService
    {
        private readonly Context _context;
        private readonly AudioManager _audioManageer;
        private readonly Window _window;
        private readonly int _maxVolume;
        private readonly int _minVolume;
        public DeviceService(Context context)
        {
            _context = context;
            _audioManageer = context.GetSystemService(Context.AudioService) as AudioManager;
            _maxVolume = _audioManageer.GetStreamMaxVolume(Stream.Music);
#if ANDROID28_0_OR_GREATER
            _minVolume = _audioManageer.GetStreamMinVolume(Stream.Music);
#else
            _minVolume = 0;
#endif

            if (context is Activity activity)
            {
                _window = activity.Window;
            }
        }

        public int MaxVolume => _maxVolume;

        public int MinVolume => _minVolume;

        public int Volume
        {
            get => _audioManageer.GetStreamVolume(Stream.Music);
            set
            {
                var v = Math.Clamp(value, MinVolume, MaxVolume);
                _audioManageer.SetStreamVolume(Stream.Music, v, VolumeNotificationFlags.ShowUi);
            }
        }
        public int Brightness
        {
            get => _window == null ? 0 : (int)(_window.Attributes.ScreenBrightness * 255);
            set
            {
                if (_window == null)
                {
                    return;
                }
                var b = Math.Clamp(value, MinBrightness, MaxBrightness);
                var attr = _window.Attributes;
                attr.ScreenBrightness = b / 255f;
                _window.Attributes = attr;
            }
        }

        public int MaxBrightness => 255;

        public int MinBrightness => 0;

        public int SystemBrightness => Settings.System.GetInt(_context.ContentResolver, Settings.System.ScreenBrightness);
    }
}
