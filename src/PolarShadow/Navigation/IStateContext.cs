﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow
{
    internal interface IStateContext
    {
        bool TryGet(string key, out object value);
        void Set(string key, object value);
        void Remove(string key);
    }
}
