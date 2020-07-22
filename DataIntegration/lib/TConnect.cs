using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using System.Data;
using DataIntegration.xml;


namespace DataIntegration.lib
{
    class TConnect
    {
        private SqlConnection conn;
        //private TConnectConfig config;
        //=====================================================


        public SqlConnection Conn
        {
            get { return this.conn; }
        }



        //=====================================================

        public TConnect(String sourceName)
        {
            TConnectConfig cfg = TXML.GetDBConnfig(sourceName);
            this.NewConn(cfg.Ip, cfg.User, cfg.Passwd, cfg.DbName);
        }



        private void NewConn(string ip, string user, string passwd, string dbName)
        {
            string connStr = "server=" + ip + ";uid=" + user + ";pwd=" + passwd + ";database=" + dbName;
            this.conn = new SqlConnection(connStr);
            try
            {
                this.conn.Open();
            }
            catch(Exception e)
            {
                Global.logMutex.WaitOne();
                Global.Log(connStr);
                Global.Log(e.ToString());
                Global.logMutex.ReleaseMutex();
            }
        }

        public int ExecuteQuery(string sql)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(sql, this.conn);
                return cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Global.logMutex.WaitOne();
                Global.Log(sql);
                Global.Log(e.ToString());
                Global.logMutex.ReleaseMutex();
                return 0;
            }
        }


        public SqlDataReader ExecuteReader(string sql)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(sql, this.conn);
                return cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                Global.logMutex.WaitOne();
                Global.Log(sql);
                Global.Log(e.ToString());
                Global.logMutex.ReleaseMutex();
                return null;
            }
        }


        public DataTable GetTable(string sql)
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter(sql, this.conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception e)
            {
                Global.logMutex.WaitOne();
                Global.Log(sql);
                Global.Log(e.ToString());
                Global.logMutex.ReleaseMutex();
                return null;
            }
        }




        public void CloseConn()
        {
            if (!(this.conn == null))
            {
                this.conn.Close();
            }
        }
    }
}
