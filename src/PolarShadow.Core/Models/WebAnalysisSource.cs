﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PolarShadow.Core
{
    public sealed class WebAnalysisSource : IKeyName
    {
        [JsonRequired]
        public string Name { get; set; }
        public string Title { get; set; }

        public string Src { get; set; }
    }
}
