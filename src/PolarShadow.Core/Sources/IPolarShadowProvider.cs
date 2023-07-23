﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public interface IPolarShadowProvider
    {
        JsonElement Root { get; }
        void Load();
    }
}
