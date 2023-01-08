using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public class WatchRecord
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EpisodeName { get; set; }
        public long? Position { get; set; }
    }
}
