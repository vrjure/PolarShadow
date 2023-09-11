using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolarShadow.Aria2
{
    public class Aria2MulticallMethod
    {
        public Aria2MulticallMethod() { }

        public Aria2MulticallMethod(Aria2Request request)
        {
            this.MethodName = request.Method;
            this.Params = request.Params.ToList();
        }

        public string MethodName { get; set; }
        public ICollection<object> Params { get; set; }
    }
}
