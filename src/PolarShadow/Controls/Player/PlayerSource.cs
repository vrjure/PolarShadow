using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    [TypeConverter(typeof(PlayerSourceConverter))]
    public abstract class PlayerSource : Element
    {
        public static PlayerSource FromUri(string uri)
        {
            return new UriVideoSource { Uri = uri };
        }

        public static PlayerSource FromFile(string file)
        {
            return new FileVideoSource { File = file };
        }

        public static PlayerSource FromResource(string path)
        {
            return new ResourceVideoSource { Path = path };
        }
    }
}
