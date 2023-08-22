using CommunityToolkit.Maui.Core.Handlers;
using CommunityToolkit.Maui.Views;
using PolarShadow.Views;
using PolarShadow.Views.ViewHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow
{
    internal static class AppBuilderExtensions
    {
        public static MauiAppBuilder UseVLCPlayer(this MauiAppBuilder builder)
        {
            builder.ConfigureMauiHandlers(h =>
            {
                h.AddHandler<VLCView, VLCViewHandler>();
            });

            return builder;
        }
    }
}
