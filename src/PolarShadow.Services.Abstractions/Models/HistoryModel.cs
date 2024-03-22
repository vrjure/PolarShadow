using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Services
{
    public class HistoryModel
    {
        public int Id { get; set; }
        public string ResourceName { get; set; }
        public long Progress { get; set; }
        public string ProgressDesc { get; set; }
        public int ProgressIndex { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
