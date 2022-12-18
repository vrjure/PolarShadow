using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core.Aria2
{
    public class Aria2Response
    {
        public string Jsonrpc { get; set; }
        public string Id { get; set; }
        public Aria2Error Error { get; set; }
    }

    public class Aria2Response<T> : Aria2Response
    {
        public T Result { get; set; }
    }
}
