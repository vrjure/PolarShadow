﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Essentials
{
    public interface IDispatcherUI
    {
        bool CheckAccess();
        void Post(Action action);
    }
}
