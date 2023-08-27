using Avalonia.Controls;
using Avalonia.Markup.Xaml.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Models
{
    public class MenuIconItem
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public Type ViewType { get; set; }
    }
}
