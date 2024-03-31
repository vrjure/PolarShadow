using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Essentials
{
    public interface IDispatcher
    {
        void Post(Action action);
    }
}
