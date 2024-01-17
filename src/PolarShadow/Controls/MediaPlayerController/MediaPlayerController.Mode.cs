using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    public partial class MediaPlayerController
    {
        private void SetPlayMode(MediaPlayerMode mode)
        {
            switch (mode)
            {
                case MediaPlayerMode.Normal:
                    NormalMode();
                    break;
                case MediaPlayerMode.Simple:
                    SimpleMode();
                    break;
                case MediaPlayerMode.Min:
                    MinMode();
                    break;
                default:
                    break;
            }
        }

        private void NormalMode()
        {
            PreviousBtn.IsVisible = NextBtn.IsVisible = TimeText.IsVisible = LengthText.IsVisible =  true;
        }

        private void SimpleMode()
        {
            PreviousBtn.IsVisible = NextBtn.IsVisible = false;
            TimeText.IsVisible = LengthText.IsVisible = true;
        }

        private void MinMode()
        {
            PreviousBtn.IsVisible = NextBtn.IsVisible = TimeText.IsVisible = LengthText.IsVisible = false;
        }
    }
}
