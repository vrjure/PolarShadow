﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Services
{
    public interface ISyncAbleModel
    {
        DateTime UpdateTime { get; set; }
    }
}