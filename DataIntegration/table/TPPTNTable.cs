using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using System.Data;
using DataIntegration.lib;

namespace DataIntegration.table
{
    class TPPTNTable
    {
        private TConnect conn;

        public TConnect Conn
        {
            get { return this.conn; }
            set { this.conn = value; }
        }

        public TPPTNTable()
        { 
        }

        public TPPTNTable(TConnect conn)
        {
            this.conn = conn;
        }

        public DataTable GetData(string sql)
        {
            DataTable dt = this.conn.GetTable(sql);
            return dt;
        }

        public int SaveData(string sql)
        {
            int s = this.conn.ExecuteQuery(sql);
            return s;

        }
    }
}
