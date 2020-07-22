using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataIntegration.util
{
    class TRelation
    {
        private string stcd;
        private string gprs_stcd;
        private string wave_stcd;
        private string hisz_stcd;
        private string hisp_stcd;
        private string hisr_stcd;

        private string conn_stcd;
        private string formula;

        private string state;


        private float last_p;
        private DateTime lastTime_p;
        private Boolean needSup_p;

        private float last_z;
        private DateTime lastTime_z;
        private Boolean needSup_z;

        private float last_q;
        private DateTime lastTime_q;
        private Boolean needSup_q;

        private float last_rz;
        private DateTime lastTime_rz;
        private Boolean needSup_rz;

        private float last_tdz;
        private DateTime lastTime_tdz;
        private Boolean needSup_tdz;

        public string Stcd
        {
            get { return this.stcd; }
            set { this.stcd = value; }
        }

        public string Gprs_stcd
        {
            get { return this.gprs_stcd; }
            set { this.gprs_stcd = value; }
        }

        public string Wave_stcd
        {
            get { return this.wave_stcd; }
            set { this.wave_stcd = value; }
        }

        public string Hisz_stcd
        {
            get { return this.hisz_stcd; }
            set { this.hisz_stcd = value; }
        }


        public string Hisp_stcd
        {
            get { return this.hisp_stcd; }
            set { this.hisp_stcd = value; }
        }

        public string Hisr_stcd
        {
            get { return this.hisr_stcd; }
            set { this.hisr_stcd = value; }
        }


        public string Conn_stcd
        {
            get { return this.conn_stcd; }
            set { this.conn_stcd = value; }
        }

        public string Formula
        {
            get { return this.formula; }
            set { this.formula = value; }
        }

        //-----------------------------------------------------

        public string State
        {
            get { return this.state; }
            set { this.state = value; }
        }

        //-----------------------------------------------

        public float Last_p
        {
            get { return this.last_p; }
            set { this.last_p = value; }
        }
        public DateTime LastTime_p
        {
            get { return this.lastTime_p; }
            set { this.lastTime_p = value; }
        }
        public Boolean NeedSup_p
        {
            get { return this.needSup_p; }
            set { this.needSup_p = value; }
        }


        public float Last_z
        {
            get { return this.last_z; }
            set { this.last_z = value; }
        }
        public DateTime LastTime_z
        {
            get { return this.lastTime_z; }
            set { this.lastTime_z = value; }
        }
        public Boolean NeedSup_z
        {
            get { return this.needSup_z; }
            set { this.needSup_z = value; }
        }


        public float Last_q
        {
            get { return this.last_q; }
            set { this.last_q = value; }
        }
        public DateTime LastTime_q
        {
            get { return this.lastTime_q; }
            set { this.lastTime_q = value; }
        }
        public Boolean NeedSup_q
        {
            get { return this.needSup_q; }
            set { this.needSup_q = value; }
        }


        public float Last_rz
        {
            get { return this.last_rz; }
            set { this.last_rz = value; }
        }
        public DateTime LastTime_rz
        {
            get { return this.lastTime_rz; }
            set { this.lastTime_rz = value; }
        }
        public Boolean NeedSup_rz
        {
            get { return this.needSup_rz; }
            set { this.needSup_rz = value; }
        }

        public float Last_tdz
        {
            get { return this.last_tdz; }
            set { this.last_tdz = value; }
        }
        public DateTime LastTime_tdz
        {
            get { return this.lastTime_tdz; }
            set { this.lastTime_tdz = value; }
        }
        public Boolean NeedSup_tdz
        {
            get { return this.needSup_tdz; }
            set { this.needSup_tdz = value; }
        }



        public TRelation()
        { 
        }


    }
}
