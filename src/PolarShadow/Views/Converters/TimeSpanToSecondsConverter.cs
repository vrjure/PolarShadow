using CommunityToolkit.Maui.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Views
{
    class TimeSpanToSecondsConverter : BaseConverter<TimeSpan, double>
    {
        public override double DefaultConvertReturnValue { get; set; } = 0d;
        public override TimeSpan DefaultConvertBackReturnValue { get; set; } = TimeSpan.Zero;

        public override TimeSpan ConvertBackTo(double value, CultureInfo culture)
        {
            return TimeSpan.FromSeconds(value);
        }

        public override double ConvertFrom(TimeSpan value, CultureInfo culture)
        {
            return value.TotalSeconds;
        }
    }
}
