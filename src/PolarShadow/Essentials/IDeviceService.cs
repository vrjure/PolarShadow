using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Essentials
{
    public interface IDeviceService
    {
        int MaxVolume { get; }
        int MinVolume { get; }
        int Volume {  get; set; }
        int MaxBrightness { get; }
        int MinBrightness { get; }
        int Brightness { get; set; }
        int SystemBrightness { get; }
    }
}
