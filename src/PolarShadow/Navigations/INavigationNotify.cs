﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Navigations
{
    public interface INavigationNotify
    {
        void OnLoad();
        void OnUnload();
    }
}