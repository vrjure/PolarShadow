using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Resources
{
    internal sealed class WebAnalysisSite : SiteDefault, IWebAnalysisSite
    {
        private string _title;
        public string Title
        {
            get => string.IsNullOrEmpty(_title) ? this.Name : _title;
            set => _title = value;
        }
    }
}
