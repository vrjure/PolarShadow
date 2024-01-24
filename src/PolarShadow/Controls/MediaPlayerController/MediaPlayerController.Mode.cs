using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    public partial class MediaPlayerController
    {
        private void SetPlayMode(MediaMode mode)
        {
            switch (mode)
            {
                case MediaMode.Normal:
                    NormalMode();
                    break;
                case MediaMode.Simple:
                    SimpleMode();
                    break;
                case MediaMode.Min:
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
