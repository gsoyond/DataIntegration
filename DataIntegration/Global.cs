using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.IO;
using DataIntegration.lib;
using System.Threading;

namespace DataIntegration
{
    class Global
    {
        public static Mutex logMutex;

        public static DateTime pptnLastTime;
        public static DateTime riverLastTime;
        public static DateTime rvsrLastTime;
        public static DateTime tideLastTime;

        public static Dictionary<string, string> pptnRedoList;
        public static Dictionary<string, string> riverRedoList;
        public static Dictionary<string, string> rvsrRedoList;
        public static Dictionary<string, string> tideRedoList;

        public Global()
        {
            
        }

        public static void InitailRedoList()
        {
            pptnRedoList = new Dictionary<string,string>();
            riverRedoList = new Dictionary<string,string>();
            rvsrRedoList = new Dictionary<string,string>();
            tideRedoList = new Dictionary<string,string>();
        }


        public static void Log(string msg)
        {
            try
            {
                string fileDir = System.Environment.CurrentDirectory;
                string fileName = DateTime.Now.ToString("yyyy-MM-dd");

                StreamWriter sw = File.AppendText(fileDir + "\\log\\" + fileName+".txt");
                DateTime dt = DateTime.Now;
                
                sw.WriteLine(dt.ToString()+ "." + dt.Ticks + " " + msg);

                sw.Flush();
                sw.Close();

            }
            catch
            {
                //
            }
        }


        public static void LogToDB(string tableName, string stcd, string action, string insStcd, string beginTime, string endTime, string value)
        {
            TConnect conn = new TConnect("AdminDb");
            try
            {
                DateTime dt = DateTime.Now;
                string sql = "insert into HD_LOG_C(recordTM, tableName, stcd, action, insStcd, beginTime, endTime, value) values (" +
                             "'" + tableName + dt.ToString("yyyy-MM-dd HH:mm:ss") + "." + dt.Ticks + "', " +
                             "'" + tableName + "', " +
                             "'" + stcd + "', " +
                             "'" + action + "', " +
                             "'" + insStcd + "', " +
                             "'" + beginTime + "', " +
                             "'" + endTime + "', " +
                             value + ")";
                conn.ExecuteQuery(sql);
                
            }
            catch (Exception e)
            {
                logMutex.WaitOne();
                Log(e.ToString());
                logMutex.ReleaseMutex();
            }
            finally
            {
                conn.CloseConn();
            }
        }


        public static float CptZByFormula(string f, float x)  //f : y=a*x+b
        {
            float s = 0;
            if (f == null)
            {
                s = x;
            }
            else
            {
                float a = Convert.ToSingle(f.Substring(f.IndexOf('=') + 1, f.IndexOf('*') - f.IndexOf('=') - 1));
                float b = 0;
                if (f.IndexOf('+') > 0)
                {
                    b = Convert.ToSingle(f.Substring(f.IndexOf('+') + 1));
                }
                s = a * x + b;
            }
            return s;
        }


        //--------------------------------统计计算-------（数据同步完成之后）
        /*
        int lastDay = startTime.Day;
        int day = nowTime.Day;
        if (!(day == lastDay))   //第二天早晨
        {

            //todo: 计算日累计雨量
            Global.Log("计算日累计雨量");
            string dataTime = (nowTime.AddDays(-1).Date).AddHours(8).ToString();  //前一天8点
            this.CptDayPPTN(conn_s, conn_d, dataTime);



            int year = nowTime.Year;
            int month = nowTime.Month;

            if (((day == 11) || (day == 21)))
            {
                //todo: 计算旬累计雨量
                Global.Log("计算旬累计雨量" + day.ToString());
                string sttdrcd = "4";
                string beginTime = year + "-" + month + (day - 10).ToString() + " 08:00:00";
                string endTime = year + "-" + month + day.ToString() + " 08:00:00";

                this.CptPStat(conn_s, conn_d, beginTime, endTime, sttdrcd);
            }
            else if (day == 1)
            {
                //to do: 计算旬累计雨量 + 计算月累计雨量
                Global.Log("计算旬累计雨量" + day.ToString());
                string sttdrcd = "4";
                DateTime upDate = nowTime.AddDays(-1);
                string beginTime = upDate.Year + "-" + upDate.Month + "-" + "21 08:00:00";  //上个月的21号，最后一个旬的开始
                string endTime = nowTime.ToShortDateString() + " 08:00:00";
                this.CptPStat(conn_s, conn_d, beginTime, endTime, sttdrcd);
                //===============================下面计算月累计雨量===========
                Global.Log("计算月累计雨量" + day.ToString());
                sttdrcd = "5";
                beginTime = upDate.Year + "-" + upDate.Month + "-" + "01 08:00:00";  //上个月的1号，上个月的开始
                endTime = nowTime.ToShortDateString() + " 08:00:00";
                this.CptPStat(conn_s, conn_d, beginTime, endTime, sttdrcd);
            }
        }
        
        //-----------------------------------------------


        /// <summary>
        /// 计算日平均降雨量
        /// </summary>
        /// <param name="conn_s"></param>
        /// <param name="conn_d"></param>
        /// <param name="dateTime"></param>
        private void CptDayPPTN(TConnect conn_s, TConnect conn_d, string dateTime)
        {
            foreach (KeyValuePair<string, TRelation> item in this.stcds)
            {
                string key = item.Key;  //key=stcd   统一后的站码
                string gprscd = item.Value.Gprs_stcd;
                string wavecd = item.Value.Wave_stcd;
                //string hiscd = item.Value.History_stcd;

                string stcd = key;
                //有GPRS码的优先使用GPRS码，没有的使用短波码
                if (item.Value.State == "01")
                {
                    stcd = wavecd;
                }
                else
                {
                    stcd = gprscd;
                }

                string dyp = null;

                string sql = "select stcd, tm, drn from rainday where stcd = '" + stcd + "' and tm='" + dateTime + "'";
                SqlDataReader dr = conn_s.ExecuteReader(sql);
                if (!(dr == null))
                {
                    if (dr.Read() && (!dr.IsDBNull(2)))
                    {
                        dyp = dr.GetFloat(2).ToString();
                    }
                    dr.Close();
                }


                sql = "seelct count(dyp) from ST_PPTN_R where stcd = '" + stcd + "' and tm='" + dateTime + "'";
                SqlDataReader sr = conn_d.ExecuteReader(sql);
                if (sr.Read())
                {
                    int count = sr.GetInt32(0);
                    if (count == 0)  //不存在
                    {
                        //insert
                        sql = "insert stcd, tm, dyp into ST_PPTN_R values('" + stcd + "', '" + dateTime + "', " + dyp + ")";
                        conn_d.ExecuteQuery(sql);
                    }
                    else  //存在
                    {
                        //update
                        sql = "update ST_PPTN_R set dyp=" + dyp + " where stcd = '" + stcd + "' and tm='" + dateTime + "'";
                        conn_d.ExecuteQuery(sql);
                    }
                }
                sr.Close();
            }
        }


        /// <summary>
        /// 计算旬、月平均降雨量  旬=4， 月=5
        /// </summary>
        /// <param name="conn_s"></param>
        /// <param name="conn_d"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="sttdrcd"></param>
        private void CptPStat(TConnect conn_s, TConnect conn_d, string beginTime, string endTime, string sttdrcd)
        {
            foreach (KeyValuePair<string, TRelation> item in this.stcds)
            {
                string key = item.Key;  //key=stcd   统一后的站码
                string gprscd = item.Value.Gprs_stcd;
                string wavecd = item.Value.Wave_stcd;
                //string hiscd = item.Value.History_stcd;

                string stcd = key;
                //有GPRS码的优先使用GPRS码，没有的使用短波码
                if (item.Value.State == "01")
                {
                    stcd = wavecd;
                }
                else
                {
                    stcd = gprscd;
                }

                string accp = null;
                //注意：sql中时间范围不能用betwwen
                string sql = "select sum(drp) as accp from ST_PPTN_R where stcd = '" + stcd + "' and tm>='" + beginTime + "' and tm<'" + endTime + "' group by stcd";
                SqlDataReader dr = conn_s.ExecuteReader(sql);
                if (!(dr == null))
                {
                    if (dr.Read() && (!dr.IsDBNull(2)))
                    {
                        accp = dr.GetFloat(2).ToString();
                    }
                    dr.Close();
                }

                DateTime dt = DateTime.Parse(endTime);
                string idtm = dt.ToShortDateString() + " 08:00:00";
                sql = "insert stcd, idtm, sttdrcd, accp into ST_PSTAT_R values('" + stcd + "', '" + idtm + "', '" + sttdrcd + "'" + accp + ")";
                conn_d.ExecuteQuery(sql);
            }
        }
        */
    }
}
