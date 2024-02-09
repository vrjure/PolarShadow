using PolarShadow.Essentials;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Desktop.Essentials
{
    internal class DeviceService : IDeviceService
    {
        private readonly int _appId;
        public DeviceService()
        {
            _appId = Process.GetCurrentProcess().Id;
        }
        public int MaxVolume => 100;

        public int MinVolume => 0;

        public int Volume
        {
            get => (int)Math.Round(AudioManager.GetApplicationVolume(_appId));
            set => AudioManager.SetApplicationVolume(_appId, value);
        }

        public int MaxBrightness => 0;

        public int MinBrightness => 0;

        public int Brightness { get; set; }

        public int SystemBrightness => 0;
    }
}
