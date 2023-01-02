using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    class FileVideoSource : PlayerSource
    {
        public static readonly BindableProperty FileProperty = BindableProperty.Create(nameof(File), typeof(string), typeof(FileVideoSource));

        public string File
        {
            get { return (string)GetValue(FileProperty); }
            set { SetValue(FileProperty, value); }
        }
    }
}
