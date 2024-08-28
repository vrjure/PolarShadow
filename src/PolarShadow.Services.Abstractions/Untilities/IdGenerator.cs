using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace PolarShadow.Services
{
    public sealed class IdGenerator
    {
        static IdGenerator()
        {
            var options = new IdGeneratorOptions();
            YitIdHelper.SetIdGenerator(options);

        }
        public static long NextId()
        {
            return YitIdHelper.NextId();
        }
    }
}
