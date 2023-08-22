using Microsoft.Maui;
using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Views.ViewHandlers
{
    internal partial class VLCViewHandler
    {
        public static IPropertyMapper<VLCView, VLCViewHandler> PropertyMapper = new PropertyMapper<VLCView, VLCViewHandler>(ViewMapper);
        public static CommandMapper<VLCView, VLCViewHandler> CommandMapper = new CommandMapper<VLCView, VLCViewHandler>(ViewCommandMapper);

        public VLCViewHandler(): base(PropertyMapper, CommandMapper)
        {

        }

    }
}
