using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataIntegration.lib
{
    class TConnectConfig
    {
        private string ip;
        private string user;
        private string passwd;
        private string dbName;


        public string Ip
        {
            get { return this.ip; }
            set { this.ip = value; }
        }

        public string User
        {
            get { return this.user; }
            set { this.user = value; }
        }

        public string Passwd
        {
            get { return this.passwd; }
            set { this.passwd = value; }
        }

        public string DbName
        {
            get { return this.dbName; }
            set { this.dbName = value; }
        }


        public TConnectConfig()
        { }

        public TConnectConfig(string ip, string user, string passwd, string dbName)
        {
            this.ip = ip;
            this.user = user;
            this.passwd = passwd;
            this.dbName = dbName;
        }
    }
}
