using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataIntegration.util
{
    class TSystemConfig
    {
        private string pptnLastTime;
        private string riverLastTime;
        private string rvsrLastTime;
        private string tideLastTime;
        private int interval;
        private int dataItr;
        private float recordRate;

        public string PptnLastTime
        {
            get { return this.pptnLastTime; }
            set { this.pptnLastTime = value; }
        }

        public string RiverLastTime
        {
            get { return this.riverLastTime; }
            set { this.riverLastTime = value; }
        }

        public string RvsrLastTime
        {
            get { return this.rvsrLastTime; }
            set { this.rvsrLastTime = value; }
        }

        public string TideLastTime
        {
            get { return this.tideLastTime; }
            set { this.tideLastTime = value; }
        }


        public int Interval
        {
            get { return this.interval; }
            set { this.interval = value; }
        }

        public int DataItr
        {
            get { return this.dataItr; }
            set { this.dataItr = value; }
        }

        public float RecordRate
        {
            get { return this.recordRate; }
            set { this.recordRate = value; }
        }

        public TSystemConfig()
        {

        }
    }
}
