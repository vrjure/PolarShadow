using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public class CombinationObject<T1,T2>
    {
        public T1 Object1 { get; set; }
        public T2 Object2 { get; set; }
    }

    public class CombinationObject<T1,T2, T3> : CombinationObject<T1,T2>
    {
        public T3 Object3 { get; set; }
    }
}
