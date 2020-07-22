using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataIntegration.lib;
using DataIntegration.util;
using DataIntegration.xml;
using DataIntegration.table;

using System.Data.SqlClient;
using System.Threading;

using System.IO;


namespace DataIntegration
{
    public partial class MainForm : Form
    {
        private TSystemConfig systemCfg;


        private int transferYear = 2014;  //需要迁移数据的年份， 随着同步时间的推移而变化

        private int beginYear = 0;        //导出起始年份
        private int endYear = 0;          //导出结束年份

       
        private Dictionary<string, TRelation> stcds_pptn;   //P  A
        private Dictionary<string, TRelation> stcds_river;  //Z  B
        private Dictionary<string, TRelation> stcds_rvsr;   //R  C 
        private Dictionary<string, TRelation> stcds_tide;   //T  D

        //private Mutex runMutex;//     参数同步控制信号量， 用于线程启动、暂停、停止的信号变动

        private Boolean run;     //     线程启动、暂停、停止的信号


        //===================================
        Thread thread_pptn;
        Thread thread_river;
        Thread thread_rvsr;
        Thread thread_tide;
        //===================================


        private DateTime startTM_pptn;  //每次启动是记录本次同步的数据开始时间，注意是数据开始的时间，而不是启动开始的时间
        private DateTime startTM_river;
        private DateTime startTM_rvsr;
        private DateTime startTM_tide;


        public MainForm()
        {
            InitializeComponent();
            this.systemCfg = TXML.GetSytemConfig("Config");
            //runMutex = new Mutex();
            Global.pptnLastTime = Convert.ToDateTime(this.systemCfg.PptnLastTime);    //初始化时间
            Global.riverLastTime = Convert.ToDateTime(this.systemCfg.RiverLastTime);    //初始化时间
            Global.rvsrLastTime = Convert.ToDateTime(this.systemCfg.RvsrLastTime);    //初始化时间
            Global.tideLastTime = Convert.ToDateTime(this.systemCfg.TideLastTime);    //初始化时间

            Global.logMutex = new Mutex();
            //Global.InitailRedoList();//初始化重做列表

            //------------------获取测站信息-------------------------
            stcds_pptn  = TRelationsManager.GetStcdList("A", "DestDb");  //多少个站，及每个站的关联信息和相关控制数据
            stcds_river = TRelationsManager.GetStcdList("B", "DestDb");
            stcds_rvsr  = TRelationsManager.GetStcdList("C", "DestDb");
            stcds_tide  = TRelationsManager.GetStcdList("D", "DestDb");
            //------------------获取测站信息-------------------------

            run = true;

            this.btnStop.Enabled = false;
            this.btnStart.Enabled = true;
            this.btnAbort.Enabled = false;

            this.btnDestSet.Enabled = true;
            this.btnSourceSet.Enabled = true;
            this.btnSystemSet.Enabled = true;


            //this.pn_state.Visible = false;

            

            cb_pptn.Checked = true;
            cb_river.Checked = true;
            cb_rvsr.Checked = true;
            cb_tide.Checked = true;


        }


        private void btnSourceSet_Click(object sender, EventArgs e)
        {
            DBSourceSetForm setForm = new DBSourceSetForm("SourceDb");
            setForm.ShowDialog();
        }


        private void btnDestSet_Click(object sender, EventArgs e)
        {
            DBSourceSetForm setForm = new DBSourceSetForm("DestDb");
            setForm.ShowDialog();
        }


        /// <summary>
        /// 启动按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
 
        private void btnStart_Click(object sender, EventArgs e)
        {
            this.btnStart.Enabled = false;
            this.btnStop.Enabled = true;
            this.btnAbort.Enabled = true;

            this.btnDestSet.Enabled = false;
            this.btnSourceSet.Enabled = false;
            this.btnSystemSet.Enabled = false;


            //-------------------------------------启动雨量同步-----------------
            if (cb_pptn.Checked)
            {
                if (this.thread_pptn == null)
                {

                    

                    

                    thread_pptn = new Thread(pptnRun);
                    thread_pptn.IsBackground = true;
                    thread_pptn.Start();
                    //Global.Log("启动线程"); 
                }
                else
                {
                    try
                    {
                        thread_pptn.Resume();
                    }
                    catch
                    {
                        //
                    }
                }
            }
            //-------------------------------------启动雨量同步-----------------

            //-------------------------------------启动河道水情同步-----------------
            if (cb_river.Checked)
            {
                if (this.thread_river == null)
                {

                    

                    

                    thread_river = new Thread(riverRun);
                    thread_river.IsBackground = true;
                    thread_river.Start();
                    //Global.Log("启动线程"); 
                }
                else
                {
                    try
                    {
                        thread_river.Resume();
                    }
                    catch
                    {
                        //
                    }
                }
            }
            //-------------------------------------启动河道水情同步-----------------

            //-------------------------------------启动水库水情同步-----------------
            if (cb_rvsr.Checked)
            {
                if (this.thread_rvsr == null)
                {

                    

                    

                    thread_rvsr = new Thread(rvsrRun);
                    thread_rvsr.IsBackground = true;
                    thread_rvsr.Start();
                    //Global.Log("启动线程"); 
                }
                else
                {
                    try
                    {
                        thread_rvsr.Resume();
                    }
                    catch
                    {
                        //
                    }
                }
            }
            //-------------------------------------启动水库水情同步-----------------


            //-------------------------------------启动潮位同步-----------------
            if (cb_tide.Checked)
            {
                if (this.thread_tide == null)
                {

                    

                    

                    thread_tide = new Thread(tideRun);
                    thread_tide.IsBackground = true;
                    thread_tide.Start();
                    //Global.Log("启动线程"); 
                }
                else
                {
                    try
                    {
                        thread_tide.Resume();
                    }
                    catch
                    {
                        //
                    }
                }
            }
            //-------------------------------------启动潮位同步-----------------

            this.cb_pptn.Enabled = false;
            this.cb_river.Enabled = false;
            this.cb_rvsr.Enabled = false;
            this.cb_tide.Enabled = false;
        }


        private void pptnRun()
        {

            int ctls = 3;
            while (run)
            {
                //stcds_pptn = TRelationsManager.GetStcdList("A", "DestDb");  //多少个站，及每个站的关联信息和相关控制数据

                TConnect conn_s = new TConnect("SourceDb");
                TConnect conn_d = new TConnect("DestDb");

                if (!(conn_s.Conn.State == ConnectionState.Open))
                {
                    //Global.logMutex.WaitOne();
                    //Global.Log("源数据库(遥测数据库)无法连接 -- thread_pptn");
                    // Global.logMutex.ReleaseMutex();

                    //如果数据无法联机,一直等待, startTime 不断往前推
                    continue;
                }

                if (!(conn_d.Conn.State == ConnectionState.Open))
                {
                    //Global.logMutex.WaitOne();
                    //Global.Log("目标数据库(实时数据库)无法连接 -- thread_pptn");
                    //Global.logMutex.ReleaseMutex();
                    //如果数据无法联机,一直等待, startTime 不断往前推
                    continue;
                }



                //解决数据丢失的问题，延迟30分钟，可能自动传输的数据不能马上到达遥测库
                //timeA启动时间，往后推一个小时
                DateTime timeA = Global.pptnLastTime;
                DateTime timeB = timeA.AddHours(1);  //往后推一个小时

                Boolean rec = PPTNIntegration(conn_s, conn_d, timeB);

                conn_s.CloseConn();  //关闭数据库
                conn_d.CloseConn();

                /*
                DateTime overTime = DateTime.Now;  //一次同步结束时间 , 不一定是一个小时

                TimeSpan ts = overTime.Subtract(startTime.AddMinutes(this.systemCfg.Interval));   //开始时间加一个小时与结束时间进行对比
                //1个小时时间差
                int span = ts.Days * 24 * 60 * 60 * 1000 + ts.Hours * 60 * 60 * 1000 + ts.Minutes * 60 * 1000 + ts.Seconds * 1000 + ts.Milliseconds;
                //double span = ts.TotalMilliseconds;  //TotalMilliseconds 算出来的不是整数,这里需要整数
                if (span < 0)
                {
                    LabPPTNMsg("降雨量数据同步： 休眠中.....");  
                    Thread.Sleep(span*(-1));   //等待一小时剩余的时间
                }
                */

                if (!rec) //同步数据量不足
                {

                    //如果同步数据量不足，休息一下
                    LabPPTNMsg("降雨量数据同步： 休眠中....." + timeA.ToString());
                    int span = 30 * 60 * 1000;   //等1小时
                    Thread.Sleep(span);
                    Global.pptnLastTime = timeA;    //上一次最后一条记录的时间
                    ctls--;
                    if (ctls == 0)
                    {
                        Global.pptnLastTime = timeB;    //上一次最后一条记录的时间
                        ctls = 3;
                    }
                }
                else
                {
                    Global.pptnLastTime = timeB;    //上一次最后一条记录的时间
                }

                //runMutex.WaitOne();
                //TXML.SaveLastTimeOfSystemConfig("Config", "pptnLastTime", this.pptnLastTime.ToString());  //保存到配置文件中,中断后可以继续同步
                //runMutex.ReleaseMutex();

            }
        }

        private void riverRun()
        {
            int ctls = 3;
            while (run)
            {

                TConnect conn_s = new TConnect("SourceDb");
                TConnect conn_d = new TConnect("DestDb");

                if (!(conn_s.Conn.State == ConnectionState.Open))
                {
                    //Global.logMutex.WaitOne();
                    //Global.Log("源数据库(遥测数据库)无法连接 -- thread_river");
                    //Global.logMutex.ReleaseMutex();

                    //如果数据无法联机,一直等待, startTime 不断往前推
                    continue;
                }

                if (!(conn_d.Conn.State == ConnectionState.Open))
                {
                    //Global.logMutex.WaitOne();
                    //Global.Log("目标数据库(实时数据库)无法连接 -- thread_river");
                    //Global.logMutex.ReleaseMutex();
                    //如果数据无法联机,一直等待, startTime 不断往前推
                    continue;
                }

                //timeA启动时间，往后推一个小时
                DateTime timeA = Global.riverLastTime;
                DateTime timeB = Global.riverLastTime.AddHours(1);  //一个小时应该有12个数据，如果不足12个数据，找另一个站码的数据

                Boolean rec = RIVERIntegration(conn_s, conn_d, timeB);

                conn_s.CloseConn();  //关闭数据库
                conn_d.CloseConn();


                if (!rec) //同步数据量不足
                {

                    //如果同步数据量不足，休息一下
                    LabRIVERMsg("河道水情数据同步： 休眠中....." + timeA.ToString());
                    int span = 30 * 60 * 1000;   //等1小时
                    Thread.Sleep(span);
                    Global.riverLastTime = timeA;    //上一次最后一条记录的时间,重复一下
                    ctls--;
                    if (ctls == 0)
                    {
                        Global.pptnLastTime = timeB;    //上一次最后一条记录的时间
                        ctls = 3;
                    }
                }
                else
                {
                    Global.riverLastTime = timeB;    //上一次最后一条记录的时间
                }

              

                //runMutex.WaitOne();
                //TXML.SaveLastTimeOfSystemConfig("Config", "riverLastTime", this.riverLastTime.ToString());  //保存到配置文件中,中断后可以继续同步
                //runMutex.ReleaseMutex();

            }
        }


        private void rvsrRun()
        {
            int ctls = 3;
            while (run)
            {

                TConnect conn_s = new TConnect("SourceDb");
                TConnect conn_d = new TConnect("DestDb");

                if (!(conn_s.Conn.State == ConnectionState.Open))
                {
                    //Global.logMutex.WaitOne();
                   // Global.Log("源数据库(遥测数据库)无法连接 -- thread_rvsr");
                    //Global.logMutex.ReleaseMutex();

                    //如果数据无法联机,一直等待, startTime 不断往前推
                    continue;
                }

                if (!(conn_d.Conn.State == ConnectionState.Open))
                {
                    //Global.logMutex.WaitOne();
                    //Global.Log("目标数据库(实时数据库)无法连接 -- thread_rvsr");
                    //Global.logMutex.ReleaseMutex();
                    //如果数据无法联机,一直等待, startTime 不断往前推
                    continue;
                }

                //timeA启动时间，往后推一个小时
                DateTime timeA = Global.rvsrLastTime;
                DateTime timeB = Global.rvsrLastTime.AddHours(1);  //一个小时应该有12个数据，如果不足12个数据，找另一个站码的数据

                Boolean rec = RSVRIntegration(conn_s, conn_d, timeB);

                conn_s.CloseConn();  //关闭数据库
                conn_d.CloseConn();


                if (!rec) //同步数据量不足
                {

                    //如果同步数据量不足，休息一下
                    this.LabRSVRMsg("水库水情数据同步： 休眠中....." + timeA.ToString());
                    int span = 30 * 60 * 1000;   //等1小时
                    Thread.Sleep(span);
                    Global.rvsrLastTime = timeA;    //上一次最后一条记录的时间,重复一下
                    ctls--;
                    if (ctls == 0)
                    {
                        Global.pptnLastTime = timeB;    //上一次最后一条记录的时间
                        ctls = 3;
                    }
                }
                else
                {
                    Global.rvsrLastTime = timeB;    //上一次最后一条记录的时间
                }


                //runMutex.WaitOne();
                //TXML.SaveLastTimeOfSystemConfig("Config", "rvsrLastTime", this.rvsrLastTime.ToString());  //保存到配置文件中,中断后可以继续同步
                //runMutex.ReleaseMutex();

            }
        }


        private void tideRun()
        {
            int ctls = 3;
            while (run)
            {
                //stcds_tide = TRelationsManager.GetStcdList("D", "DestDb");

                TConnect conn_s = new TConnect("SourceDb");
                TConnect conn_d = new TConnect("DestDb");

                if (!(conn_s.Conn.State == ConnectionState.Open))
                {
                    //Global.logMutex.WaitOne();
                    //Global.Log("源数据库(遥测数据库)无法连接 -- thread_tide");
                    //Global.logMutex.ReleaseMutex();

                    //如果数据无法联机,一直等待, startTime 不断往前推
                    continue;
                }

                if (!(conn_d.Conn.State == ConnectionState.Open))
                {
                    //Global.logMutex.WaitOne();
                    //Global.Log("目标数据库(实时数据库)无法连接 -- thread_tide");
                    //Global.logMutex.ReleaseMutex();
                    //如果数据无法联机,一直等待, startTime 不断往前推
                    continue;
                }

                //timeA启动时间，往后推一个小时
                DateTime timeA = Global.tideLastTime;
                DateTime timeB = Global.tideLastTime.AddHours(1);  //一个小时应该有12个数据，如果不足12个数据，找另一个站码的数据

                Boolean rec = TIDEIntegration(conn_s, conn_d, timeB);

                conn_s.CloseConn();  //关闭数据库
                conn_d.CloseConn();


                if (!rec) //同步数据量不足
                {

                    //如果同步数据量不足，休息一下
                    this.LabTIDEMsg("潮位数据同步： 休眠中....." + timeA.ToString());
                    int span = 30 * 60 * 1000;   //等1小时
                    Thread.Sleep(span);
                    Global.tideLastTime = timeA;    //上一次最后一条记录的时间,重复一下
                    ctls--;
                    if (ctls == 0)
                    {
                        Global.pptnLastTime = timeB;    //上一次最后一条记录的时间
                        ctls = 3;
                    }
                }
                else
                {
                    Global.tideLastTime = timeB;    //上一次最后一条记录的时间
                }
                //runMutex.WaitOne();
                //TXML.SaveLastTimeOfSystemConfig("Config", "tideLastTime", this.tideLastTime.ToString());  //保存到配置文件中,中断后可以继续同步
                //runMutex.ReleaseMutex();

            }
        }

        
        private int GetTimeSpanCount(DateTime beginTime, DateTime nowTime)
        {
            int max = 0;

            TimeSpan timeSpan = nowTime.Subtract(beginTime);   //两个时间之间的间隔
            //int spanMinutes = timeSpan.Days * 24 * 60 + timeSpan.Hours * 60 + timeSpan.Minutes ;  //将时间间隔换算成分钟数
            double spanMinutes = timeSpan.TotalMinutes;
            int count = Convert.ToInt32(spanMinutes / this.systemCfg.Interval);     //计算有多少个60分钟

            if ((count * this.systemCfg.Interval) < spanMinutes)
            {
                max = count + 1;   //有余数的时候也多算一个间隔
            }
            else
            {
                max = count;
            }

            return max;
        }



        
        /// <summary>
        /// 降雨量数据同步
        /// </summary>
        private Boolean PPTNIntegration(TConnect conn_s, TConnect conn_d,  DateTime endTM)
        {
            #region 降雨量数据同步程序体

            LabPPTNMsg("雨量数据同步中...... " + endTM.ToString());
            int flag = -1;//0:正常数据； 1：短波数据；2：关联站数据插补； 3：直线插值；4：二次插值

            //int totalCount = 0;
            DateTime minNewTime = endTM;

            DisplayPPTNProgress(true, this.stcds_pptn.Count);  //需要同步雨量的站数

            foreach (KeyValuePair<string, TRelation> item in this.stcds_pptn)
            {

                try
                {
                    string key = item.Key;                   //key=stcd   统一后的站码
                    string gprscd = item.Value.Gprs_stcd;    //GPRS站码
                    string wavecd = item.Value.Wave_stcd;    //超短波站码 
                    string conncd = item.Value.Conn_stcd;    //关联站码
                    string formula = item.Value.Formula;     //公式

                    string stcd = key;     //有GPRS码的优先使用GPRS码，没有的使用短波码,在数据库中存储的stcd就是这样的
                    string[,] data = null; //装数据的容器 

                    DateTime beginTM = item.Value.LastTime_p;


                    //到源数据库中取数据，包括开始时间,但不包括结束时间，保证时间段之间的没有重叠
                    string sql = "select stcd, tm, drp from ST_PPTN_R where stcd = '" + stcd + "' and tm >= '" + beginTM.ToString() + "' and tm < '" + endTM.ToString() + "' order by tm";

                    DataTable dt = conn_s.GetTable(sql);

                    if ((!(dt == null)) && (dt.Rows.Count>0))
                    {
                        //直接生成对应的data[][]
                        data = new string[dt.Rows.Count, 2];
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            data[i, 0] = dt.Rows[i][1].ToString(); //好像不可能为NULL  tm
                            data[i, 1] = ((dt.Rows[i][2].ToString() == "") || (dt.Rows[i][2] == null)) ? "null" : dt.Rows[i][2].ToString(); //好像可能为NULL  drp   
                        }
                        flag = 0;

                    }

                    //------------------------------------------
                    //totalCount += dt.Rows.Count; //获取了多少条记录，这个是是否休息的依据

                    dt.Clear();
                    dt = null;

                    DateTime mostNewTime = beginTM;
                    if (!(data == null))
                    {

                        //Global.Log("存储数据 " + flag);
                        int dataRows = data.GetLength(0);  //数据的行数

                        for (int i = 0; i < dataRows; i++)
                        {
                            string tm = data[i, 0];  //tm

                            //考察时间记录，如果时间记录比最终时间小，就记录一下
                            //------------------------------------------
                            mostNewTime = Convert.ToDateTime(tm);
                            //------------------------------------------

                            string drp = data[i, 1]; //drp

                            if (!(drp == "null"))  //所有的空值都被改成＂null＂
                            {
                                double v = Convert.ToDouble(drp);
                                if (v < 0)
                                {
                                    //自动删除 雨量负值数据
                                    //Global.Log("自动删除 雨量负值数据 tm:" + tm + ", drp:" + drp);

                                }
                                else if (v > 30)
                                {
                                    //自动删除 5分钟雨量大于30mm数据
                                    //Global.Log("自动删除 5分钟雨量大于30mm数据 tm:" + tm + ", drp:" + drp);

                                }
                                else
                                {
                                    //----------------------------数据是否存在----------------------
                                    Boolean exist = false;
                                    sql = "select count(*) as rcount from ST_PPTN_R where stcd='" + key + "' and tm = '" + tm + "'";
                                    SqlDataReader dr = conn_d.ExecuteReader(sql);
                                    if (dr.Read())
                                    {
                                        if (dr.GetInt32(0) > 0)
                                        {
                                            exist = true;
    
                                        }
                                    }
                                    dr.Close();
                                    //----------------------------数据是否存在----------------------

                                    if (!exist)
                                    {
                                        //数据存储到目标数据库中
                                        sql = "insert into ST_PPTN_R(stcd, tm, drp, flag) values(" +
                                              "'" + key + "', " +   //不管是什么数据都按照统一的站码存储
                                              "'" + tm + "', " +
                                              drp + ", " +
                                              flag + ")";
                                        conn_d.ExecuteQuery(sql);

  

                                        //Global.Log(sql);
                                    }


                                }
                            }
                        }

                    }
                    item.Value.LastTime_p = mostNewTime;

                    minNewTime = mostNewTime;


                    PPTNProgressing();         //每同步一条数据，进度条往前一步
                }
                catch (Exception e)
                {
                    Global.logMutex.WaitOne();
                    Global.Log(e.ToString());
                    Global.logMutex.ReleaseMutex();
                }

            }

            Boolean rec = true;
            TimeSpan timeSpan = endTM.Subtract(minNewTime);   //两个时间之间的间隔
            int spanMinutes = timeSpan.Days * 24 * 60 + timeSpan.Hours * 60 + timeSpan.Minutes ;  //将时间间隔换算成分钟数

            if (spanMinutes > 30) //时间间隔超过30分钟
            {
                rec = false;
            }
            return rec;

            #endregion
        }


        /// <summary>
        /// 河道水情数据同步
        /// </summary>
        private Boolean RIVERIntegration(TConnect conn_s, TConnect conn_d, DateTime endTM)
        {
            #region 河道水情数据同步程序体


            LabRIVERMsg("河道水情数据同步中...... " + endTM.ToString());

            int flag = -1;   //0:正常数据； 1：短波数据；2：关联站数据插补； 3：直线插值；4：二次插值

            DateTime minNewTime = endTM;

            DisplayRIVERProgress(true, this.stcds_river.Count);  //需要同步雨量的站数


            foreach (KeyValuePair<string, TRelation> item in this.stcds_river)
            {

                try
                {

                    string key = item.Key;                   //key=stcd   统一后的站码
                    string gprscd = item.Value.Gprs_stcd;    //GPRS站码
                    string wavecd = item.Value.Wave_stcd;    //超短波站码
                    string conncd = item.Value.Conn_stcd;    //关联站码
                    string formula = item.Value.Formula;     //公式

                    string stcd = key;     //有GPRS码的优先使用GPRS码，没有的使用短波码,在数据库中存储的stcd就是这样的
                    string[,] data = null; //装数据的容器 

                    DateTime beginTM = item.Value.LastTime_z;

                    //到源数据库中取数据，结束时间减1秒，保证时间段之间的没有重叠
                    string sql = "select stcd, tm, z, q, xsa, xsavv from ST_RIVER_R where stcd = '" + stcd + "' and tm >= '" + beginTM.ToString() + "' and tm < '" + endTM.ToString() + "' order by tm";

                    DataTable dt = conn_s.GetTable(sql);

                    if ((!(dt == null)) && (dt.Rows.Count > 0))
                    {
                        //直接生成对应的data[][]
                        //Global.Log("获取的数据超过一半");
                        data = new string[dt.Rows.Count, 5];
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            data[i, 0] = dt.Rows[i][1].ToString(); //好像不可能为NULL  tm
                            data[i, 1] = ((dt.Rows[i][2].ToString() == "") || (dt.Rows[i][2] == null)) ? "null" : dt.Rows[i][2].ToString();  //好像不可能为NULL  z 
                            data[i, 2] = ((dt.Rows[i][3].ToString() == "") || (dt.Rows[i][3] == null)) ? "null" : dt.Rows[i][3].ToString();
                            data[i, 3] = ((dt.Rows[i][4].ToString() == "") || (dt.Rows[i][4] == null)) ? "null" : dt.Rows[i][4].ToString();
                            data[i, 4] = ((dt.Rows[i][5].ToString() == "") || (dt.Rows[i][5] == null)) ? "null" : dt.Rows[i][5].ToString();
                        }
                        flag = 0;

                    }




                    //------------------------------------------
                    dt.Clear();
                    dt = null;

                    DateTime mostNewTime = beginTM;
                    if (!(data == null))
                    {
                        //Global.Log("存储数据");
                        int dataRows = data.GetLength(0);  //数据的行数



                        for (int i = 0; i < dataRows; i++)
                        {
                            //string stcd = dt.Rows[i][0].ToString();
                            string tm = data[i, 0];       //tm
                            string z = data[i, 1];      //z
                            string q = data[i, 2];       //q
                            string xsa = data[i, 3];       //xsa
                            string xsavv = data[i, 4];       //xsavv

                            //考察时间记录，如果时间记录比最终时间小，就记录一下
                            //------------------------------------------
                            mostNewTime = Convert.ToDateTime(tm);
                            //------------------------------------------



                            //----------------------------数据是否存在----------------------
                            Boolean exist = false;
                            sql = "select count(*) as rcount from ST_RIVER_R where stcd='" + key + "' and tm = '" + tm + "'";
                            SqlDataReader dr = conn_d.ExecuteReader(sql);
                            if (dr.Read())
                            {
                                if (dr.GetInt32(0) > 0)
                                {
                                    exist = true;
                                }
                            }
                            dr.Close();
                            //----------------------------数据是否存在----------------------


                            //数据存储到目标数据库中
                            if (!exist)
                            {
                                sql = "insert into ST_RIVER_R(stcd, tm, z, q, xsa, xsavv, flag) values(" +
                                      "'" + key + "', " +   //不管是什么数据都按照统一的站码存储
                                      "'" + tm + "', " +
                                      z + ", " +
                                      q + ", " +
                                      xsa + ", " +
                                      xsavv + ", " +
                                      flag + ")";
                                //Global.Log(sql);
                                conn_d.ExecuteQuery(sql);

                            }

                            //this.stcds_river[key].Last_z = Convert.ToSingle((z == "null") ? "0.0" : z);       //最后一个有效的数据
                            //this.stcds_river[key].LastTime_z = Convert.ToDateTime(tm);  //最后一个有效数据的时间 
                            //this.stcds_river[key].NeedSup_z = false;

                        }
                    }

                    item.Value.LastTime_z = mostNewTime;
                    minNewTime = mostNewTime;
                    RIVERProgressing();         //每同步一条数据，进度条往前一步
                }
                catch (Exception e)
                {
                    Global.logMutex.WaitOne();
                    Global.Log(e.ToString());
                    Global.logMutex.ReleaseMutex();
                }

                

            }
        
            //BtnStratState(true);           //正常恢复按钮状态
            Boolean rec = true;
            TimeSpan timeSpan = endTM.Subtract(minNewTime);   //两个时间之间的间隔
            int spanMinutes = timeSpan.Days * 24 * 60 + timeSpan.Hours * 60 + timeSpan.Minutes;  //将时间间隔换算成分钟数

            if (spanMinutes > 30) //时间间隔超过30分钟
            {
                rec = false;
            }
            return rec;


            #endregion
        }


        /// <summary>
        /// 水库水情数据同步
        /// </summary>
        private Boolean RSVRIntegration(TConnect conn_s, TConnect conn_d, DateTime endTM)
        {
            #region 水库水情数据同步程序体

            //BtnStratState(false); //启动 按钮不可编辑


            LabRSVRMsg("水库水情数据同步中...... " + endTM.ToString());
            int flag = -1;//0:正常数据； 1：短波数据；2：关联站数据插补； 3：直线插值；4：二次插值
            DateTime minNewTime = endTM;


            DisplayRSVRProgress(true, this.stcds_rvsr.Count);  //需要同步雨量的站数

            foreach (KeyValuePair<string, TRelation> item in this.stcds_rvsr)
            {
                try
                {
                    string key = item.Key;                   //key=stcd   统一后的站码
                    string gprscd = item.Value.Gprs_stcd;    //GPRS站码
                    string wavecd = item.Value.Wave_stcd;    //超短波站码
                    string conncd = item.Value.Conn_stcd;    //关联站码
                    string formula = item.Value.Formula;     //公式

                    string stcd = key;     //有GPRS码的优先使用GPRS码，没有的使用短波码,在数据库中存储的stcd就是这样的
                    string[,] data = null; //装数据的容器 


                    DateTime beginTM = item.Value.LastTime_rz;

                    //到源数据库中取数据，包括开始时间,但不包括结束时间，保证时间段之间的没有重叠
                    string sql = "select stcd, tm, rz from ST_RSVR_R where stcd = '" + stcd + "' and tm >= '" + beginTM.ToString() + "' and tm < '" + endTM.ToString() + "' order by tm";
                    //Global.Log(sql);

                    DataTable dt = conn_s.GetTable(sql);


                    if ((!(dt == null)) && (dt.Rows.Count > 0))
                    {
                        //直接生成对应的data[][]
                        //Global.Log("数据超过一半");
                        data = new string[dt.Rows.Count, 2];
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            data[i, 0] = dt.Rows[i][1].ToString(); //好像不可能为NULL  tm
                            data[i, 1] = ((dt.Rows[i][2].ToString() == "") || (dt.Rows[i][2] == null)) ? "null" : dt.Rows[i][2].ToString();  //好像可能为NULL  rz  
                        }
                        flag = 0;
                    
                    }



                    //------------------------------------------
                    dt.Clear();
                    dt = null;

                    DateTime mostNewTime = beginTM;
                    if (!(data == null))
                    {
                        //Global.Log("存储数据 " + flag);
                        int dataRows = data.GetLength(0);  //数据的行数


                        for (int i = 0; i < dataRows; i++)
                        {
                            string tm = data[i, 0];  //tm
                            string rz = data[i, 1];  //rz

                            //考察时间记录，如果时间记录比最终时间小，就记录一下
                            //------------------------------------------
                            mostNewTime = Convert.ToDateTime(tm);
                            //------------------------------------------

                            //----------------------------数据是否存在----------------------
                            Boolean exist = false;
                            sql = "select count(*) as rcount from ST_RSVR_R where stcd='" + key + "' and tm = '" + tm + "'";
                            SqlDataReader dr = conn_d.ExecuteReader(sql);
                            if (dr.Read())
                            {
                                if (dr.GetInt32(0) > 0)
                                {
                                    exist = true;
                                }
                            }
                            dr.Close();
                            //----------------------------数据是否存在----------------------


                            //数据存储到目标数据库中
                            if (!exist)
                            {
                                sql = "insert into ST_RSVR_R(stcd, tm, rz, flag) values(" +
                                      "'" + key + "', " +   //不管是什么数据都按照统一的站码存储
                                      "'" + tm + "', " +
                                      rz + ", " +
                                      flag + ")";
                                conn_d.ExecuteQuery(sql);
                                //Global.Log(sql);
                            }

                        }
                    }

                    item.Value.LastTime_rz = mostNewTime;
                    minNewTime = mostNewTime;

                    RSVRProgressing();         //每同步一条数据，进度条往前一步
                }
                catch (Exception e)
                {
                    Global.logMutex.WaitOne();
                    Global.Log(e.ToString());
                    Global.logMutex.ReleaseMutex();
                }

            }

            Boolean rec = true;
            TimeSpan timeSpan = endTM.Subtract(minNewTime);   //两个时间之间的间隔
            int spanMinutes = timeSpan.Days * 24 * 60 + timeSpan.Hours * 60 + timeSpan.Minutes;  //将时间间隔换算成分钟数

            if (spanMinutes > 30) //时间间隔超过30分钟
            {
                rec = false;
            }
            return rec;


            #endregion
        }


        /// <summary>
        /// 潮位数据同步
        /// </summary>
        private Boolean TIDEIntegration(TConnect conn_s, TConnect conn_d, DateTime endTM)
        {
            #region 潮位数据同步程序体

            //BtnStratState(false); //启动 按钮不可编辑
            LabTIDEMsg("潮位数据同步中...... " + endTM.ToString());
            int flag = -1;//0:正常数据； 1：短波数据；2：关联站数据插补； 3：直线插值；4：二次插值
            DateTime minNewTime = endTM;

            DisplayTIDEProgress(true, this.stcds_tide.Count);  //需要同步雨量的站数

            foreach (KeyValuePair<string, TRelation> item in this.stcds_tide)
            {

                try
                {

                    string key = item.Key;                   //key=stcd   统一后的站码
                    string gprscd = item.Value.Gprs_stcd;    //GPRS站码
                    string wavecd = item.Value.Wave_stcd;    //超短波站码
                    string conncd = item.Value.Conn_stcd;    //关联站码
                    string formula = item.Value.Formula;     //公式

                    string stcd = key;     //有GPRS码的优先使用GPRS码，没有的使用短波码,在数据库中存储的stcd就是这样的
                    string[,] data = null; //装数据的容器 

                    DateTime beginTM = item.Value.LastTime_tdz;

                    //到源数据库中取数据，包括开始时间,但不包括结束时间，保证时间段之间的没有重叠
                    string sql = "select stcd, tm, tdz from ST_TIDE_R where stcd = '" + stcd + "' and tm >= '" + beginTM.ToString() + "' and tm < '" + endTM.ToString() + "' order by tm";
                    //Global.Log(sql);

                    DataTable dt = conn_s.GetTable(sql);

                    if ((!(dt == null)) && (dt.Rows.Count > 0))
                    {
                        //直接生成对应的data[][]
                        //Global.Log("数据超过一半");
                        data = new string[dt.Rows.Count, 2];
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            data[i, 0] = dt.Rows[i][1].ToString(); //好像不可能为NULL  tm
                            data[i, 1] = ((dt.Rows[i][2].ToString() == "") || (dt.Rows[i][2] == null)) ? "null" : dt.Rows[i][2].ToString();  //好像不可能为NULL  tdz  
                        }
                        flag = 0;

                    }
      


                    //------------------------------------------
                    dt.Clear();
                    dt = null;

                    DateTime mostNewTime = beginTM;
                    if (!(data == null))
                    {
                        //Global.Log("存储数据 " + flag);
                        int dataRows = data.GetLength(0);  //数据的行数


                        for (int i = 0; i < dataRows; i++)
                        {
                            string tm = data[i, 0];  //tm
                            string tdz = data[i, 1];  //rz


                            //考察时间记录，如果时间记录比最终时间小，就记录一下
                            //------------------------------------------
                            mostNewTime = Convert.ToDateTime(tm);
                            //------------------------------------------

                            //----------------------------数据是否存在----------------------
                            Boolean exist = false;
                            sql = "select count(*) as rcount from ST_TIDE_R where stcd='" + key + "' and tm = '" + tm + "'";
                            SqlDataReader dr = conn_d.ExecuteReader(sql);
                            if (dr.Read())
                            {
                                if (dr.GetInt32(0) > 0)
                                {
                                    exist = true;
                                }
                            }
                            dr.Close();
                            //----------------------------数据是否存在----------------------

                            //数据存储到目标数据库中
                            if (!exist)
                            {
                                sql = "insert into ST_TIDE_R(stcd, tm, tdz, flag) values(" +
                                      "'" + key + "', " +   //不管是什么数据都按照统一的站码存储
                                      "'" + tm + "', " +
                                      tdz + ", " +
                                      flag + ")";
                                conn_d.ExecuteQuery(sql);
                                //Global.Log(sql);
                            }


                            //this.stcds_tide[key].Last_tdz = Convert.ToSingle((tdz == "null") ? "0.0" : tdz);       //最后一个有效的数据
                            //this.stcds_tide[key].LastTime_tdz = Convert.ToDateTime(tm);  //最后一个有效数据的时间 

                        }
                    }

                    item.Value.LastTime_tdz = mostNewTime;
                    minNewTime = mostNewTime;

                    TIDEProgressing();         //每同步一条数据，进度条往前一步
                }
                catch (Exception e)
                {
                    Global.logMutex.WaitOne();
                    Global.Log(e.ToString());
                    Global.logMutex.ReleaseMutex();
                }
            }


            Boolean rec = true;
            TimeSpan timeSpan = endTM.Subtract(minNewTime);   //两个时间之间的间隔
            int spanMinutes = timeSpan.Days * 24 * 60 + timeSpan.Hours * 60 + timeSpan.Minutes;  //将时间间隔换算成分钟数

            if (spanMinutes > 30) //时间间隔超过30分钟
            {
                rec = false;
            }
            return rec;


            #endregion
        }



        /// <summary>
        /// 降雨量进度条控制
        /// </summary>
        /// <param name="display"></param>
        /// <param name="max"></param>
        private delegate void DisplayProgressPPTNControl(bool display, int max);
        private void DisplayPPTNProgress(bool display, int max)
        {
            if (this.progressPPTN.InvokeRequired)//等待异步
            {
                DisplayProgressPPTNControl c = new DisplayProgressPPTNControl(DisplayPPTNProgress);
                this.Invoke(c, new object[] { display, max });
            }
            else
            {
                //int pWidth = this.Width / 3;
                //this.saveProgress.Width = pWidth;
                //this.saveProgress.Location = new Point(this.Width / 2 - pWidth / 2, this.Height / 2);
                this.progressPPTN.Maximum = max;
                this.progressPPTN.Step = 1;
                this.progressPPTN.Value = 0;
                this.progressPPTN.Visible = display;
            }
        }
        private delegate void PPTNProgressingControl();
        private void PPTNProgressing()
        {
            if (this.progressPPTN.InvokeRequired)//等待异步
            {
                PPTNProgressingControl c = new PPTNProgressingControl(PPTNProgressing);
                this.Invoke(c);
            }
            else
            {
                this.progressPPTN.PerformStep();
            }
        }
        //==============================================================
        /// <summary>
        /// 河道水情进度条控制
        /// </summary>
        /// <param name="display"></param>
        /// <param name="max"></param>
        private delegate void DisplayProgressRIVERControl(bool display, int max);
        private void DisplayRIVERProgress(bool display, int max)
        {
            if (this.progressRIVER.InvokeRequired)//等待异步
            {
                DisplayProgressRIVERControl c = new DisplayProgressRIVERControl(DisplayRIVERProgress);
                this.Invoke(c, new object[] { display, max });
            }
            else
            {
                //int pWidth = this.Width / 3;
                //this.saveProgress.Width = pWidth;
                //this.saveProgress.Location = new Point(this.Width / 2 - pWidth / 2, this.Height / 2);
                this.progressRIVER.Maximum = max;
                this.progressRIVER.Step = 1;
                this.progressRIVER.Value = 0;
                this.progressRIVER.Visible = display;
            }
        }
        private delegate void RIVERProgressingControl();
        private void RIVERProgressing()
        {
            if (this.progressRIVER.InvokeRequired)//等待异步
            {
                RIVERProgressingControl c = new RIVERProgressingControl(RIVERProgressing);
                this.Invoke(c);
            }
            else
            {
                this.progressRIVER.PerformStep();
            }
        }
        //===================================================
        
        /// <summary>
        /// 水库水情进度条控制
        /// </summary>
        /// <param name="display"></param>
        /// <param name="max"></param>
        private delegate void DisplayProgressRSVRControl(bool display, int max);
        private void DisplayRSVRProgress(bool display, int max)
        {
            if (this.progressRSVR.InvokeRequired)//等待异步
            {
                DisplayProgressRSVRControl c = new DisplayProgressRSVRControl(DisplayRSVRProgress);
                this.Invoke(c, new object[] { display, max });
            }
            else
            {
                //int pWidth = this.Width / 3;
                //this.saveProgress.Width = pWidth;
                //this.saveProgress.Location = new Point(this.Width / 2 - pWidth / 2, this.Height / 2);
                this.progressRSVR.Maximum = max;
                this.progressRSVR.Step = 1;
                this.progressRSVR.Value = 0;
                this.progressRSVR.Visible = display;
            }
        }
        private delegate void RSVRProgressingControl();
        private void RSVRProgressing()
        {
            if (this.progressRSVR.InvokeRequired)//等待异步
            {
                RSVRProgressingControl c = new RSVRProgressingControl(RSVRProgressing);
                this.Invoke(c);
            }
            else
            {
                this.progressRSVR.PerformStep();
            }
        }
        //==============================================================
        /// <summary>
        /// 潮位进度条控制
        /// </summary>
        /// <param name="display"></param>
        /// <param name="max"></param>
        private delegate void DisplayProgressTIDEControl(bool display, int max);
        private void DisplayTIDEProgress(bool display, int max)
        {
            if (this.progressTIDE.InvokeRequired)//等待异步
            {
                DisplayProgressTIDEControl c = new DisplayProgressTIDEControl(DisplayTIDEProgress);
                this.Invoke(c, new object[] { display, max });
            }
            else
            {
                //int pWidth = this.Width / 3;
                //this.saveProgress.Width = pWidth;
                //this.saveProgress.Location = new Point(this.Width / 2 - pWidth / 2, this.Height / 2);
                this.progressTIDE.Maximum = max;
                this.progressTIDE.Step = 1;
                this.progressTIDE.Value = 0;
                this.progressTIDE.Visible = display;
            }
        }
        private delegate void TIDEProgressingControl();
        private void TIDEProgressing()
        {
            if (this.progressTIDE.InvokeRequired)//等待异步
            {
                TIDEProgressingControl c = new TIDEProgressingControl(TIDEProgressing);
                this.Invoke(c);
            }
            else
            {
                this.progressTIDE.PerformStep();
            }
        }
        //==================================================
        /// <summary>
        /// 启动按钮状态
        /// </summary>
        /// <param name="enable"></param>
        private delegate void BtnStratStateControl(Boolean enable);
        private void BtnStratState(Boolean enable)
        {
            if (this.btnStart.InvokeRequired)
            {
                BtnStratStateControl c = new BtnStratStateControl(BtnStratState);
                this.Invoke(c, new object[] { enable });
            }
            else
            {
                this.btnStart.Enabled = enable;
            }
        }
        //==================================================
        /// <summary>
        /// 停止按钮状态
        /// </summary>
        /// <param name="enable"></param>
        private delegate void BtnStopStateControl(Boolean enable);
        private void BtnStopState(Boolean enable)
        {
            if (this.btnStop.InvokeRequired)
            {
                BtnStopStateControl c = new BtnStopStateControl(BtnStopState);
                this.Invoke(c, new object[] { enable });
            }
            else
            {
                this.btnStop.Enabled = enable;
            }
        }
        //==================================================
        /// <summary>
        /// 雨量信息提示
        /// </summary>
        /// <param name="enable"></param>
        private delegate void LabPPTNMsgControl(string msg);
        private void LabPPTNMsg(string msg)
        {
            if (this.labelPPTN.InvokeRequired)
            {
                LabPPTNMsgControl c = new LabPPTNMsgControl(LabPPTNMsg);
                this.Invoke(c, new object[] { msg });
            }
            else
            {
                this.labelPPTN.Text = msg;
            }
        }
        //==================================================
        /// <summary>
        /// 河道水情信息提示
        /// </summary>
        /// <param name="enable"></param>
        private delegate void LabRIVERMsgControl(string msg);
        private void LabRIVERMsg(string msg)
        {
            if (this.labelRIVER.InvokeRequired)
            {
                LabRIVERMsgControl c = new LabRIVERMsgControl(LabRIVERMsg);
                this.Invoke(c, new object[] { msg });
            }
            else
            {
                this.labelRIVER.Text = msg;
            }
        }
        //==================================================
        /// <summary>
        /// 水库水情信息提示
        /// </summary>
        /// <param name="enable"></param>
        private delegate void LabRSVRMsgControl(string msg);
        private void LabRSVRMsg(string msg)
        {
            if (this.labelRSVR.InvokeRequired)
            {
                LabRSVRMsgControl c = new LabRSVRMsgControl(LabRSVRMsg);
                this.Invoke(c, new object[] { msg });
            }
            else
            {
                this.labelRSVR.Text = msg;
            }
        }
        //==================================================
        /// <summary>
        /// 潮位信息提示
        /// </summary>
        /// <param name="enable"></param>
        private delegate void LabTIDEMsgControl(string msg);
        private void LabTIDEMsg(string msg)
        {
            if (this.labelTIDE.InvokeRequired)
            {
                LabTIDEMsgControl c = new LabTIDEMsgControl(LabTIDEMsg);
                this.Invoke(c, new object[] { msg });
            }
            else
            {
                this.labelTIDE.Text = msg;
            }
        }





        private void btnStop_Click(object sender, EventArgs e)
        {
            if (this.cb_pptn.Checked)
            {
                try
                {
                    this.thread_pptn.Suspend();
                }
                catch
                {
                    //
                }
            }

            if (this.cb_river.Checked)
            {
                try
                {
                    this.thread_river.Suspend();
                }
                catch
                {
                    //
                }
            }

            if (this.cb_rvsr.Checked)
            {
                try
                {
                    this.thread_rvsr.Suspend();
                }
                catch
                {
                    //
                }
            }

            if (this.cb_tide.Checked)
            {
                try
                {
                    this.thread_tide.Suspend();
                }
                catch
                {
                    //
                }
            }

            this.btnStart.Enabled = true;
            this.btnStart.Text = "恢复";
            this.btnStop.Enabled = false;
            this.btnAbort.Enabled = true;

            this.btnDestSet.Enabled = true;
            this.btnSourceSet.Enabled = true;
            this.btnSystemSet.Enabled = true;


            this.cb_pptn.Enabled = true;
            this.cb_river.Enabled = true;
            this.cb_rvsr.Enabled = true;
            this.cb_tide.Enabled = true;
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            if (this.cb_pptn.Checked)
            {
                try
                {
                    this.thread_pptn.Abort();
                    this.thread_pptn = null;
                }
                catch
                {
                    //
                }
            }

            if (this.cb_river.Checked)
            {
                try
                {
                    this.thread_river.Abort();
                    this.thread_river = null;
                }
                catch
                {
                    //
                }
            }

            if (this.cb_rvsr.Checked)
            {
                try
                {
                    this.thread_rvsr.Abort();
                    this.thread_rvsr = null;
                }

                catch
                {
                    //
                }
            }

            if (this.cb_tide.Checked)
            {
                try
                {
                    this.thread_tide.Abort();
                    this.thread_tide = null;
                }
                catch
                {
                    //
                }
            }


            this.btnStart.Enabled = true;
            this.btnStart.Text = "启动";
            this.btnStop.Enabled = false;
            this.btnAbort.Enabled = false;

            this.btnDestSet.Enabled = true;
            this.btnSourceSet.Enabled = true;
            this.btnSystemSet.Enabled = true;


            this.cb_pptn.Enabled = true;
            this.cb_river.Enabled = true;
            this.cb_rvsr.Enabled = true;
            this.cb_tide.Enabled = true;

        }



        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!(this.thread_pptn == null))
            {
                if (!(this.thread_pptn.ThreadState == ThreadState.Aborted))
                {
                    this.thread_pptn.Abort();
                }
                this.thread_pptn = null;
            }
            if (!(this.thread_river == null))
            {
                if (!(this.thread_river.ThreadState == ThreadState.Aborted))
                {
                    this.thread_river.Abort();
                }
                this.thread_river = null;
            }
            if (!(this.thread_rvsr == null))
            {
                if (!(this.thread_rvsr.ThreadState == ThreadState.Aborted))
                {
                    this.thread_rvsr.Abort();
                }
                this.thread_rvsr = null;
            }
            if (!(this.thread_tide == null))
            {
                if (!(this.thread_tide.ThreadState == ThreadState.Aborted))
                {
                    this.thread_tide.Abort();
                }
                this.thread_tide = null;
            }


            if (!(this.stcds_pptn == null))
            {
                this.stcds_pptn.Clear();
                this.stcds_pptn = null;
            }
            if (!(this.stcds_river == null))
            {
                this.stcds_river.Clear();
                this.stcds_river = null;
            }
            if (!(this.stcds_rvsr == null))
            {
                this.stcds_rvsr.Clear();
                this.stcds_rvsr = null;
            }
            if (!(this.stcds_tide == null))
            {
                this.stcds_tide.Clear();
                this.stcds_tide = null;
            }

            /*
            if (!(this.runMutex == null))
            {
                this.runMutex.Close();
                this.runMutex = null;
            }
            */

            if (!(Global.logMutex == null))
            {
                Global.logMutex.Close();
                Global.logMutex = null;
            }


            //关闭系统时，记录各个数据库上一次同步结束的时间
            TXML.UpdateLastTime(Global.pptnLastTime.ToString(), Global.riverLastTime.ToString(), Global.rvsrLastTime.ToString(), Global.tideLastTime.ToString());

        }

        private void btnSystemSet_Click(object sender, EventArgs e)
        {
             
           
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.tx_beginYear.Visible = false;
            this.tx_endYear.Visible = false;
            this.btn2000.Visible = false;
            this.lab_ytoy.Visible = false;
        }

        private void ClearDataByOneHour()
        {
            string checkTime_pptn = TXML.GetSytemConfig("Config").PptnLastTime;
            string checkTime_river = TXML.GetSytemConfig("Config").RiverLastTime;
            string checkTime_rvsr = TXML.GetSytemConfig("Config").RvsrLastTime;
            string checkTime_tide = TXML.GetSytemConfig("Config").TideLastTime;
            TConnect conn = new TConnect("DestDb");
            try
            {
                string sql = "delete from ST_PPTN_R where tm>= '" + checkTime_pptn + "' and flag in (2,3,4)";
                conn.ExecuteQuery(sql);
                //Global.Log(sql);

                sql = "delete from st_river_r where tm>= '" + checkTime_river + "' and flag in (2,3,4)";
                conn.ExecuteQuery(sql);
                //Global.Log(sql);

                sql = "delete from st_rsvr_r where tm>= '" + checkTime_rvsr + "' and flag in (2,3,4)";
                conn.ExecuteQuery(sql);
                //Global.Log(sql);

                sql = "delete from st_tide_r where tm>= '" + checkTime_tide + "' and flag in (2,3,4)";
                conn.ExecuteQuery(sql);
                //Global.Log(sql);
            }
            catch (Exception e)
            {
                Global.logMutex.WaitOne();
                Global.Log(e.ToString());
                Global.logMutex.ReleaseMutex();
            }
            finally
            {
                conn.CloseConn();
            }
        }

        private void ClearDataByThisTime()
        {

            TConnect conn = new TConnect("DestDb");
            try
            {
                string sql = "delete from ST_PPTN_R where tm>= '" + this.startTM_pptn + "' and flag in (2,3,4)";
                conn.ExecuteQuery(sql);
                //Global.Log(sql);

                sql = "delete from st_river_r where tm>= '" + this.startTM_river + "' and flag in (2,3,4)";
                conn.ExecuteQuery(sql);
                //Global.Log(sql);

                sql = "delete from st_rsvr_r where tm>= '" + this.startTM_rvsr + "' and flag in (2,3,4)";
                conn.ExecuteQuery(sql);
                //Global.Log(sql);

                sql = "delete from st_tide_r where tm>= '" + this.startTM_tide + "' and flag in (2,3,4)";
                conn.ExecuteQuery(sql);
                //Global.Log(sql);
            }
            catch (Exception e)
            {
                Global.logMutex.WaitOne();
                Global.Log(e.ToString());
                Global.logMutex.ReleaseMutex();
            }
            finally
            {
                conn.CloseConn();
            }
        }


        private void ClearDataAll()
        {
            TConnect conn = new TConnect("DestDb");
            try
            {
                string sql = "delete from ST_PPTN_R where flag in (2,3,4)";
                conn.ExecuteQuery(sql);
                //Global.Log(sql);

                sql = "delete from st_river_r where flag in (2,3,4)";
                conn.ExecuteQuery(sql);
                //Global.Log(sql);

                sql = "delete from st_rsvr_r where flag in (2,3,4)";
                conn.ExecuteQuery(sql);
                //Global.Log(sql);

                sql = "delete from st_tide_r where  flag in (2,3,4)";
                conn.ExecuteQuery(sql);
                //Global.Log(sql);
            }
            catch (Exception e)
            {
                Global.logMutex.WaitOne();
                Global.Log(e.ToString());
                Global.logMutex.ReleaseMutex();
            }
            finally
            {
                conn.CloseConn();
            }
        }

        private void btnBackupSet_Click(object sender, EventArgs e)
        {
            DBSourceSetForm setForm = new DBSourceSetForm("BackupDb");
            setForm.ShowDialog();
        }


        //================================================================================
        //以下代码用户数据窗口滑动向备份数据库中迁移

        /// <summary>
        /// 雨量数据迁移
        /// </summary>
        private void PPTNTransfer()
        {
            TConnect conn_real = new TConnect("DestDb");          // 实时数据库
            TConnect conn_bak = new TConnect("BackupDb");        // 备份数据库

            foreach (KeyValuePair<string, TRelation> item in this.stcds_pptn)
            {
                string key = item.Key;
                for (int month = 1; month <= 12; month++)          //一个月的数据迁移一次  考虑到一年的数据量有点大，删除时耗时间
                {
                    string bTime = transferYear + "-" + month + "-1 00:00:00";
                    string eTime = transferYear + "-" + month + "-" + DateTime.DaysInMonth(transferYear, month) + " 23:59:59";

                    //获取每个月要迁移的数据集
                    string sql = "select STCD, TM, DRP, FLAG from ST_PPTN_R where stcd ='" + key + "' tm between '" + bTime + "' and '" + eTime + "'";
                    SqlDataReader dr = conn_real.ExecuteReader(sql);
                    while (dr.Read())
                    {
                        string stcd = dr.GetString(0);
                        string tm = dr.GetDateTime(1).ToString();
                        string drp = dr.IsDBNull(2) ? "null" : dr.GetDouble(2).ToString();
                        string flag = dr.GetString(3);
                        //迁移数据，insert into 备份数据库
                        sql = "insert into ST_PPTN_R(STCD, TM, DRP, FLAG) values('" + stcd + "', '" + tm + "', " + drp + ", " + flag + ")";
                        conn_bak.ExecuteQuery(sql);
                    }
                    dr.Close();
                    //删除实时数据库中相应的数据
                    sql = "delete from ST_PPTN_R where stcd ='" + key + "'  tm between '" + bTime + "' and '" + eTime + "'";
                    conn_real.ExecuteQuery(sql);
                }
            }
            conn_real.CloseConn();   //关闭实时数据库联接
            conn_bak.CloseConn();   //关闭备份数据库联接
        }

        /// <summary>
        /// 河道数据迁移
        /// </summary>
        private void RIVERTransfer()
        {
            TConnect conn_real = new TConnect("DestDb");          // 实时数据库
            TConnect conn_bak = new TConnect("BackupDb");        // 备份数据库

            foreach (KeyValuePair<string, TRelation> item in this.stcds_river)
            {
                string key = item.Key;
                for (int month = 1; month <= 12; month++)          //一个月的数据迁移一次  考虑到一年的数据量有点大，删除时耗时间
                {
                    string bTime = transferYear + "-" + month + "-1 00:00:00";
                    string eTime = transferYear + "-" + month + "-" + DateTime.DaysInMonth(transferYear, month) + " 23:59:59";

                    //获取每个月要迁移的数据集
                    string sql = "select STCD, TM, Z, Q, XSA, XSAVV, FLAG from ST_RIVER_R where stcd ='" + key + "'  tm between '" + bTime + "' and '" + eTime + "'";
                    SqlDataReader dr = conn_real.ExecuteReader(sql);
                    while (dr.Read())
                    {
                        string stcd = dr.GetString(0);
                        string tm = dr.GetDateTime(1).ToString();
                        string z = dr.IsDBNull(2) ? "null" : dr.GetDouble(2).ToString();
                        string q = dr.IsDBNull(3) ? "null" : dr.GetDouble(3).ToString();
                        string xsa = dr.IsDBNull(4) ? "null" : dr.GetDouble(4).ToString();
                        string xsavv = dr.IsDBNull(5) ? "null" : dr.GetDouble(5).ToString();
                        string flag = dr.GetString(6);
                        //迁移数据，insert into 备份数据库
                        sql = "insert into ST_RIVER_R(STCD, TM, Z, Q, XSA, XSAVV,  FLAG) values('" + stcd + "', '" + tm + "', " + z + ", " + q + ", " + xsa + ", " + xsavv + ", " + flag + ")";
                        conn_bak.ExecuteQuery(sql);
                    }
                    dr.Close();
                    //删除实时数据库中相应的数据
                    sql = "delete from ST_RIVER_R where stcd ='" + key + "'  tm between '" + bTime + "' and '" + eTime + "'";
                    conn_real.ExecuteQuery(sql);
                }
            }

            conn_real.CloseConn();   //关闭实时数据库联接
            conn_bak.CloseConn();   //关闭备份数据库联接
        }

        /// <summary>
        /// 水库数据迁移
        /// </summary>
        private void RSVRTransfer()
        {
            TConnect conn_real = new TConnect("DestDb");          // 实时数据库
            TConnect conn_bak = new TConnect("BackupDb");        // 备份数据库

            foreach (KeyValuePair<string, TRelation> item in this.stcds_rvsr)
            {
                string key = item.Key;
                for (int month = 1; month <= 12; month++)          //一个月的数据迁移一次  考虑到一年的数据量有点大，删除时耗时间
                {
                    string bTime = transferYear + "-" + month + "-1 00:00:00";
                    string eTime = transferYear + "-" + month + "-" + DateTime.DaysInMonth(transferYear, month) + " 23:59:59";

                    //获取每个月要迁移的数据集
                    string sql = "select STCD, TM, RZ, FLAG from ST_RSVR_R where stcd ='" + key + "' tm between '" + bTime + "' and '" + eTime + "'";
                    SqlDataReader dr = conn_real.ExecuteReader(sql);
                    while (dr.Read())
                    {
                        string stcd = dr.GetString(0);
                        string tm = dr.GetDateTime(1).ToString();
                        string rz = dr.IsDBNull(2) ? "null" : dr.GetDouble(2).ToString();
                        string flag = dr.GetString(3);
                        //迁移数据，insert into 备份数据库
                        sql = "insert into ST_RSVR_R(STCD, TM, RZ, FLAG) values('" + stcd + "', '" + tm + "', " + rz + ", " + flag + ")";
                        conn_bak.ExecuteQuery(sql);
                    }
                    dr.Close();
                    //删除实时数据库中相应的数据
                    sql = "delete from ST_RSVR_R where stcd ='" + key + "' tm between '" + bTime + "' and '" + eTime + "'";
                    conn_real.ExecuteQuery(sql);
                }
            }
            conn_real.CloseConn();   //关闭实时数据库联接
            conn_bak.CloseConn();   //关闭备份数据库联接
        }


        /// <summary>
        /// 潮位数据迁移
        /// </summary>
        private void TIDETransfer()
        {
            TConnect conn_real = new TConnect("DestDb");          // 实时数据库
            TConnect conn_bak = new TConnect("BackupDb");        // 备份数据库

            foreach (KeyValuePair<string, TRelation> item in this.stcds_tide)
            {
                string key = item.Key;
                for (int month = 1; month <= 12; month++)          //一个月的数据迁移一次  考虑到一年的数据量有点大，删除时耗时间
                {
                    string bTime = transferYear + "-" + month + "-1 00:00:00";
                    string eTime = transferYear + "-" + month + "-" + DateTime.DaysInMonth(transferYear, month) + " 23:59:59";

                    //获取每个月要迁移的数据集
                    string sql = "select STCD, TM, TDZ, FLAG from ST_TIDE_R where stcd ='" + key + "' tm between '" + bTime + "' and '" + eTime + "'";
                    SqlDataReader dr = conn_real.ExecuteReader(sql);
                    while (dr.Read())
                    {
                        string stcd = dr.GetString(0);
                        string tm = dr.GetDateTime(1).ToString();
                        string tdz = dr.IsDBNull(2) ? "null" : dr.GetDouble(2).ToString();
                        string flag = dr.GetString(3);
                        //迁移数据，insert into 备份数据库
                        sql = "insert into ST_TIDE_R(STCD, TM, TDZ, FLAG) values('" + stcd + "', '" + tm + "', " + tdz + ", " + flag + ")";
                        conn_bak.ExecuteQuery(sql);
                    }
                    dr.Close();
                    //删除实时数据库中相应的数据
                    sql = "delete from ST_TIDE_R where stcd ='" + key + "' tm between '" + bTime + "' and '" + eTime + "'";
                    conn_real.ExecuteQuery(sql);
                }
            }
            conn_real.CloseConn();   //关闭实时数据库联接
            conn_bak.CloseConn();   //关闭备份数据库联接
        }

        //============================================================
        //数据迁移线程
        private void Transfer()
        {
            //-------------------------------
            Thread thread_pptn_transfer = new Thread(PPTNTransfer);
            thread_pptn_transfer.IsBackground = true;
            thread_pptn_transfer.Start();
            //-------------------------------
            Thread thread_river_transfer = new Thread(RIVERTransfer);
            thread_river_transfer.IsBackground = true;
            thread_river_transfer.Start();
            //-------------------------------
            Thread thread_rsvr_transfer = new Thread(RSVRTransfer);
            thread_rsvr_transfer.IsBackground = true;
            thread_rsvr_transfer.Start();
            //-------------------------------
            Thread thread_tide_transfer = new Thread(TIDETransfer);
            thread_tide_transfer.IsBackground = true;
            thread_tide_transfer.Start();
            //-------------------------------
        }


        //=============================================================================
        //以下为数据导出程序代码

        /// <summary>
        /// 降雨数据导出
        /// </summary>
        private void PPTNExport()
        {
            TConnect conn_source = new TConnect("DestDb");     // 遥测数据库
            TConnect conn_bak = new TConnect("BackupDb");        // 备份数据库

            int index = 0;
            LabPPTNMsg("雨量数据导出： " + index + "/" + this.stcds_pptn.Count);
            foreach (KeyValuePair<string, TRelation> item in this.stcds_pptn)
            {
                string key = item.Key;
                for (int y = this.beginYear; y <= this.endYear; y++)
                {

                    string bTime = y + "-01-01 00:00:00";
                    string eTime = y + "-12-31 23:59:59";

                    //获取一年数据的记录数
                    int recordCount = 0;
                    string sql = "select count(*) from ST_PPTN_R where stcd ='" + key + "' and tm between '" + bTime + "' and '" + eTime + "'";
                    SqlDataReader countdr = conn_source.ExecuteReader(sql);
                    if (countdr.Read())
                    {
                        recordCount = countdr.GetInt32(0);
                    }
                    countdr.Close();

                    DisplayPPTNProgress(true, recordCount);  //需要同步雨量数据记录数据（一年）

                    //获取每年要导出的数据集
                    sql = "select STCD, TM, DRP from ST_PPTN_R where stcd ='" + key + "' and tm between '" + bTime + "' and '" + eTime + "'";
                    SqlDataReader dr = conn_source.ExecuteReader(sql);
                    while (dr.Read())
                    {
                        string stcd = dr.GetString(0);
                        string tm = dr.GetDateTime(1).ToString();
                        string drp = dr.IsDBNull(2) ? "null" : dr.GetFloat(2).ToString();
                        string flag = "0";
                        //导出数据，insert into 备份数据库
                        sql = "insert into ST_PPTN_R(STCD, TM, DRP, FLAG) values('" + stcd + "', '" + tm + "', " + drp + ", " + flag + ")";
                        conn_bak.ExecuteQuery(sql);

                        //删除原来的数据
                        sql = "delet from ST_PPTN_R where stcd = '" + stcd + "' and tm ='" + tm + "'";
                        conn_source.ExecuteQuery(sql);

                        PPTNProgressing();         //每同步一条数据，进度条往前一步
                    }
                    dr.Close();
                }

                index++;
                LabPPTNMsg("雨量数据导出： " + index + "/" + this.stcds_pptn.Count);
            }
            conn_source.CloseConn();   //关闭遥测数据库联接
            conn_bak.CloseConn();      //关闭备份数据库联接

            stcds_pptn.Clear();
            stcds_pptn = null;

        }


        /// <summary>
        /// 河道数据导出
        /// </summary>
        private void RIVERExport()
        {
            TConnect conn_source = new TConnect("DestDb");     // 遥测数据库
            TConnect conn_bak = new TConnect("BackupDb");        // 备份数据库

            int index = 0;
            LabRIVERMsg("河道数据导出： " + index + "/" + this.stcds_river.Count);
            foreach (KeyValuePair<string, TRelation> item in this.stcds_river)
            {
                string key = item.Key;
                for (int y = this.beginYear; y <= this.endYear; y++)
                {

                    string bTime = y + "-01-01 00:00:00";
                    string eTime = y + "-12-31 23:59:59";

                    //获取一年数据的记录数
                    int recordCount = 0;
                    string sql = "select count(*) from ST_RIVER_R where stcd ='" + key + "' and  tm between '" + bTime + "' and '" + eTime + "'";
                    SqlDataReader countdr = conn_source.ExecuteReader(sql);
                    if (countdr.Read())
                    {
                        recordCount = countdr.GetInt32(0);
                    }
                    countdr.Close();

                    DisplayRIVERProgress(true, recordCount);  //需要同步雨量数据记录数据（一年）

                    //获取每年要导出的数据集
                    sql = "select STCD, TM, Z, Q, XSA, XSAVV from ST_RIVER_R where stcd ='" + key + "' and tm between '" + bTime + "' and '" + eTime + "'";
                    SqlDataReader dr = conn_source.ExecuteReader(sql);
                    while (dr.Read())
                    {
                        string stcd = dr.GetString(0);
                        string tm = dr.GetDateTime(1).ToString();
                        string z = dr.IsDBNull(2) ? "null" : dr.GetFloat(2).ToString();
                        string q = dr.IsDBNull(3) ? "null" : dr.GetFloat(3).ToString();
                        string xsa = dr.IsDBNull(4) ? "null" : dr.GetFloat(4).ToString();
                        string xsavv = dr.IsDBNull(5) ? "null" : dr.GetFloat(5).ToString();
                        string flag = "0";
                        //迁移数据，insert into 备份数据库
                        sql = "insert into ST_RIVER_R(STCD, TM, Z, Q, XSA, XSAVV, FLAG) values('" + stcd + "', '" + tm + "', " + z + ", " + q + ", " + xsa + ", " + xsavv + ", " + flag + ")";
                        conn_bak.ExecuteQuery(sql);

                        //删除原来的数据
                        sql = "delet from ST_RIVER_R where stcd = '" + stcd + "' and tm ='" + tm + "'";
                        conn_source.ExecuteQuery(sql);

                        RIVERProgressing();         //每同步一条数据，进度条往前一步
                    }
                    dr.Close();
                }

                index++;
                LabRIVERMsg("河道数据导出： " + index + "/" + this.stcds_river.Count);
            }

            conn_source.CloseConn();   //关闭遥测数据库联接
            conn_bak.CloseConn();      //关闭备份数据库联接

            this.stcds_river.Clear();
            this.stcds_river = null;
        }


        /// <summary>
        /// 水库数据导出
        /// </summary>
        private void RSVRExport()
        {
            TConnect conn_source = new TConnect("DestDb");     // 遥测数据库
            TConnect conn_bak = new TConnect("BackupDb");        // 备份数据库

            int index = 0;
            LabRSVRMsg("水库数据导出： " + index + "/" + this.stcds_rvsr.Count);
            foreach (KeyValuePair<string, TRelation> item in this.stcds_rvsr)
            {
                string key = item.Key;
                for (int y = this.beginYear; y <= this.endYear; y++)
                {
                    string bTime = y + "-01-01 00:00:00";
                    string eTime = y + "-12-31 23:59:59";

                    //获取一年数据的记录数
                    int recordCount = 0;
                    string sql = "select count(*) from ST_RSVR_R where stcd ='" + key + "' and tm between '" + bTime + "' and '" + eTime + "'";
                    SqlDataReader countdr = conn_source.ExecuteReader(sql);
                    if (countdr.Read())
                    {
                        recordCount = countdr.GetInt32(0);
                    }
                    countdr.Close();

                    DisplayRSVRProgress(true, recordCount);  //需要同步雨量数据记录数据（一年）

                    //获取每年要导出的数据集
                    sql = "select STCD, TM, RZ from ST_RSVR_R where  stcd ='" + key + "' and tm between '" + bTime + "' and '" + eTime + "'";
                    SqlDataReader dr = conn_source.ExecuteReader(sql);
                    while (dr.Read())
                    {
                        string stcd = dr.GetString(0);
                        string tm = dr.GetDateTime(1).ToString();
                        string rz = dr.IsDBNull(2) ? "null" : dr.GetFloat(2).ToString();
                        string flag = "0";
                        //导出数据，insert into 备份数据库
                        sql = "insert into ST_RSVR_R(STCD, TM, RZ, FLAG) values('" + stcd + "', '" + tm + "', " + rz + ", " + flag + ")";
                        conn_bak.ExecuteQuery(sql);

                        //删除原来的数据
                        sql = "delet from ST_RSVR_R where stcd = '" + stcd + "' and tm ='" + tm + "'";
                        conn_source.ExecuteQuery(sql);

                        RSVRProgressing();         //每同步一条数据，进度条往前一步
                    }
                    dr.Close();
                }

                index++;
                LabRSVRMsg("水库数据导出： " + index + "/" + this.stcds_rvsr.Count);
            }
            conn_source.CloseConn();   //关闭遥测数据库联接
            conn_bak.CloseConn();      //关闭备份数据库联接

            this.stcds_rvsr.Clear();
            this.stcds_rvsr = null;
        }


        /// <summary>
        /// 潮位数据导出
        /// </summary>
        private void TIDEExport()
        {
            TConnect conn_source = new TConnect("DestDb");     // 遥测数据库
            TConnect conn_bak = new TConnect("BackupDb");        // 备份数据库

            int index = 0;
            LabTIDEMsg("潮位数据导出： " + index + "/" + this.stcds_tide.Count);
            foreach (KeyValuePair<string, TRelation> item in this.stcds_tide)
            {
                string key = item.Key;
                for (int y = this.beginYear; y <= this.endYear; y++)
                {

                    string bTime = y + "-01-01 00:00:00";
                    string eTime = y + "-12-31 23:59:59";

                    //获取一年数据的记录数
                    int recordCount = 0;
                    string sql = "select count(*) from ST_TIDE_R where stcd='" + key + "' and tm between '" + bTime + "' and '" + eTime + "'";
                    SqlDataReader countdr = conn_source.ExecuteReader(sql);
                    if (countdr.Read())
                    {
                        recordCount = countdr.GetInt32(0);
                    }
                    countdr.Close();

                    DisplayTIDEProgress(true, recordCount);  //需要同步雨量数据记录数据（一年）

                    //获取每年要导出的数据集
                    sql = "select STCD, TM, TDZ from ST_TIDE_R where stcd='" + key + "' and tm between '" + bTime + "' and '" + eTime + "'";
                    SqlDataReader dr = conn_source.ExecuteReader(sql);
                    while (dr.Read())
                    {
                        string stcd = dr.GetString(0);
                        string tm = dr.GetDateTime(1).ToString();
                        string tdz = dr.IsDBNull(2) ? "null" : dr.GetFloat(2).ToString();
                        string flag = "0";
                        //导出数据，insert into 备份数据库
                        sql = "insert into ST_TIDE_R(STCD, TM, TDZ, FLAG) values('" + stcd + "', '" + tm + "', " + tdz + ", " + flag + ")";
                        conn_bak.ExecuteQuery(sql);

                        //删除原来的数据
                        sql = "delet from ST_TIDE_R where stcd = '" + stcd + "' and tm ='" + tm + "'";
                        conn_source.ExecuteQuery(sql);

                        TIDEProgressing();         //每同步一条数据，进度条往前一步
                    }
                    dr.Close();
                }

                index++;
                LabTIDEMsg("潮位数据导出： " + index + "/" + this.stcds_tide.Count);
            }

            conn_source.CloseConn();   //关闭遥测数据库联接
            conn_bak.CloseConn();      //关闭备份数据库联接

            this.stcds_tide.Clear();
            this.stcds_tide = null;
        }

        //==================================================================
        //数据迁移线程
        private void Export()
        {
            //-------------------------------
            Thread thread_pptn_export = new Thread(PPTNExport);
            thread_pptn_export.IsBackground = true;
            thread_pptn_export.Start();
            //-------------------------------
            Thread thread_river_export = new Thread(RIVERExport);
            thread_river_export.IsBackground = true;
            thread_river_export.Start();
            //-------------------------------
            Thread thread_rsvr_export = new Thread(RSVRExport);
            thread_rsvr_export.IsBackground = true;
            thread_rsvr_export.Start();
            //-------------------------------
            Thread thread_tide_export = new Thread(TIDEExport);
            thread_tide_export.IsBackground = true;
            thread_tide_export.Start();
            //-------------------------------
        }


        private void btn2000_Click(object sender, EventArgs e)
        {
            this.beginYear = Convert.ToInt32(this.tx_beginYear.Text.Trim());
            this.endYear = Convert.ToInt32(this.tx_endYear.Text.Trim());

            stcds_pptn = TRelationsManager.GetStcdList("A", "BackupDb");  //多少个站，及每个站的关联信息和相关控制数据
            stcds_river = TRelationsManager.GetStcdList("B", "BackupDb");
            stcds_rvsr = TRelationsManager.GetStcdList("C", "BackupDb");
            stcds_tide = TRelationsManager.GetStcdList("D", "BackupDb");

            Export();
        }

        private void cb_export_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cb_export.Checked)
            {
                this.tx_beginYear.Visible = true;
                this.tx_endYear.Visible = true;
                this.btn2000.Visible = true;
                this.lab_ytoy.Visible = true;
            }
            else
            {
                this.tx_beginYear.Visible = false;
                this.tx_endYear.Visible = false;
                this.btn2000.Visible = false;
                this.lab_ytoy.Visible = false;
            }
        }

        private void btn_test_Click(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            MessageBox.Show(dt.ToString());
            DateTime newdt = dt.AddHours(30);
            MessageBox.Show(newdt.ToString());
            MessageBox.Show(dt.ToString());
        }
    }
}
