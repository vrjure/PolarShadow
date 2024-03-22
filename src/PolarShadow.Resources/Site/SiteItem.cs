using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Resources
{
    internal class SiteItem : SiteItemBase<SiteDefault>, ISiteItem
    {
        public SiteItem(string name, IRequestHandler httpHandler, IRequestHandler webViewHandler, IEnumerable<RequestRule> requestRules):base(httpHandler, webViewHandler, requestRules)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            this.Name = name;
        }

        public override string Name { get; }
    }
}
