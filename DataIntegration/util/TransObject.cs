using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataIntegration.util
{
    public class TransObject
    {
        public string Stcd { get; set; }
        public DateTime BgTime { get; set; }
        public DateTime EdTime { get; set; }
        public int RetryCount { get; set; }
    }
}
