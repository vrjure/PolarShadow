using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    internal partial struct NameSlotReader
    {
        private bool IsParameterStart(byte ch)
        {
            return ch >= NameSlotConstants.A && ch <= NameSlotConstants.z
                || ch == NameSlotConstants._;
        }
        private bool IsParameterChar(byte ch)
        {
            return ch >= NameSlotConstants.A && ch <= NameSlotConstants.z
                || ch == NameSlotConstants._
                || ch >= NameSlotConstants.Num0 && ch <= NameSlotConstants.Num9;
        }
    }
}
