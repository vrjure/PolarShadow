using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    [TypeConverter(typeof(FlexBasisConverter))]
    internal class FlexBasis : FlexParam
    {
        public FlexBasis(string value) : base(value)
        {

        }

        public static readonly FlexBasis Auto = new FlexBasis("auto");

    }
}
