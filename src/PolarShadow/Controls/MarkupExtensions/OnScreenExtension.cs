using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Metadata;
using PolarShadow.Essentials;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls.MarkupExtensions
{
    public class OnScreenExtension : OnScreenExtensionBase<object>
    {
        public OnScreenExtension()
        {

        }
        public OnScreenExtension(object defaultValue)
        {
            Default = defaultValue;
        }

        public static bool ShouldProvideOption(string option)
        {
            return ShouldProvideOptionInternal(option);
        }
    }

    public class OnScreenExtensionBase<TReturn> : IAddChild<On<TReturn>>
    {
        [MarkupExtensionDefaultOption]
        public TReturn Default { get; set; }
        [MarkupExtensionOption(nameof(Small))]
        public TReturn Small { get; set; }
        [MarkupExtensionOption(nameof(Medium))]
        public TReturn Medium { get; set; }
        [MarkupExtensionOption(nameof(Large))]
        public TReturn Large { get; set; }

        private protected static bool ShouldProvideOptionInternal(string option)
        {
            var topLevel = TopLevelService.TopLevel;
            if (topLevel == null)
            {
                return false;
            }
            return option switch
            {
                "Small" => topLevel.ClientSize.Width < 450,
                "Medium" => topLevel.ClientSize.Width >= 450 && topLevel.ClientSize.Width < 1024,
                "Large" => topLevel.ClientSize.Width >= 1024,
                _ => false
            };
        }

        void IAddChild<On<TReturn>>.AddChild(On<TReturn> child) { }

        public object ProvideValue() { return this; }
    }
}
