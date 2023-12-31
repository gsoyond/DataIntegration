﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataIntegration.xml;
using DataIntegration.lib;
using System.Data.SqlClient;

namespace DataIntegration
{
    public partial class DBSourceSetForm : Form
    {
        private string sourceName;

        public event EventHandler DbConfigChanged;

        public string SourceName
        {
            get { return this.sourceName; }
            set { this.sourceName = value; }
        }


        public DBSourceSetForm()
        {
            
            InitializeComponent();
            initail();
        }

        public DBSourceSetForm(string sourceName)
        {
            this.sourceName = sourceName;
            
            InitializeComponent();
            initail();
        }


        private void initail()
        {
            //this.cb_dbType.Items.Clear();
            //this.cb_dbType.Items.Add("Oracle 10g");
            //this.cb_dbType.Items.Add("Oracle 11g");
            //this.cb_dbType.Items.Add("Sqlserver 2000");
            //this.cb_dbType.Items.Add("Sqlserver 2008");
            //this.cb_dbType.Items.Add("Sqlserver 2008 R2");

            TConnectConfig cfg = TXML.GetDBConnfig(this.sourceName);
            this.cb_dbType.Text = "Sqlserver 2008 R2";
            this.tb_port.Text = "1433";

            this.tb_ip.Text = cfg.Ip;
            this.tb_dbName.Text = cfg.DbName;
            this.tb_userName.Text = cfg.User;
            this.tb_pswd.Text = cfg.Passwd;

        }

        private void bt_cannel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bt_ok_Click(object sender, EventArgs e)
        {
            String ip = this.tb_ip.Text;
            String dbName = this.tb_dbName.Text;
            String userName = this.tb_userName.Text;
            String passwd = this.tb_pswd.Text;

            TConnectConfig cfg = new TConnectConfig();
            cfg.Ip = ip;
            cfg.DbName = dbName;
            cfg.User = userName;
            cfg.Passwd = passwd;

            TXML.SetDBConnfig(cfg, this.sourceName);

            //MessageBox.Show("请重新启动系统或重新启动服务，新的参数才能生效！");
            DbConfigChanged?.Invoke(this, EventArgs.Empty);

            this.Close();
        }

        private void bt_test_Click(object sender, EventArgs e)
        {
            String ip = this.tb_ip.Text;
            String dbName = this.tb_dbName.Text;
            String userName = this.tb_userName.Text;
            String passwd = this.tb_pswd.Text;

            if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(dbName) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passwd))
            {
                MessageBox.Show("请先完善信息！");
                return;
            }

            string connStr = "server=" + ip + ";uid=" + userName + ";pwd=" + passwd + ";database=" + dbName;
            using (IDbConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    MessageBox.Show("连接成功！");
                }
                catch (Exception ex)
                {
                    Global.logMutex.WaitOne();
                    Global.Log(connStr);
                    Global.Log(ex.ToString());
                    Global.logMutex.ReleaseMutex();
                    MessageBox.Show("连接失败！ 出错信息：" + ex.ToString());
                }
            }
        }
    }



}
