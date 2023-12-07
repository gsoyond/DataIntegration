using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using DataIntegration.lib;
using DataIntegration.util;

namespace DataIntegration.table
{
    /// <summary>
    /// river河道水情表数据传输
    /// </summary>
    public class RiverTableTrans
    {
        /// 基本原则：按一个小时为间隔段进行同步，若一个小时内数据量少于6条，则加入下一次的队列继续同步；
        /// 如果重试了3次仍然有数据缺失，则忽略丢失的数据
        /// 每过6个小时，对前前6个小时的数据同步一次
        /// 每天8点，对前24小时的数据同步一次
        
        // 等待进行数据传输的队列
        public Queue<TransObject> TodoTransQueue { get; private set; }

        public event EventHandler transProgressEvent;

        /// <summary>
        /// 初始化数据传输队列，将所有雨量站指定的数据同步时间压入队列
        /// </summary>
        /// <returns></returns>
        public bool InitTodoTransQueue(List<string> stcds, DateTime bgTime, DateTime edTime, int retry=0)
        {
            if (stcds == null || bgTime==null || edTime==null || bgTime>edTime)
            {
                return false;
            }

            if (TodoTransQueue==null)
            {
                TodoTransQueue = new Queue<TransObject>(stcds.Count);
            }

            foreach (var stcd in stcds)
            {
                TodoTransQueue.Enqueue(new TransObject() { Stcd = stcd, BgTime = bgTime, EdTime = edTime, RetryCount = retry });
            }
            return true;
        }

        public bool AddTodoTrans(TransObject todo)
        {
            if (todo==null || TodoTransQueue == null)
            {
                return false;
            }

            TodoTransQueue.Enqueue(todo);
            return true;
        }

        public bool AddTodoTrans(Queue<TransObject> todoQueue)
        {
            if (todoQueue == null || TodoTransQueue == null)
            {
                return false;
            }

            foreach (var todo in todoQueue)
            {
                TodoTransQueue.Enqueue(todo);
            }            
            return true;
        }
        /// 获取读取数据的sql
        /// 时间范围是开区间
        public string GetSelectSql(string stcd, DateTime bgtime, DateTime edtime)
        {
            if (string.IsNullOrEmpty(stcd) || bgtime == null || edtime ==null || bgtime > edtime)
            {
                return null;
            }

            string bg_tmp = bgtime.ToString("yyyy-MM-dd HH:mm:ss");
            string ed_tmp = edtime.AddSeconds(1).ToString("yyyy-MM-dd HH:mm:ss");

            return string.Format("select stcd, tm, z, q, xsa, xsavv from ST_RIVER_R where stcd='{0}' and tm>'{1}' and tm<'{2}'", stcd, bg_tmp, ed_tmp);
        }

        /// <summary>
        /// 根据雨量数据去目标数据库检索对比，决定是插入还是更新
        /// </summary>
        /// <param name="stcd"></param>
        /// <param name="tm"></param>
        /// <param name="drp"></param>
        /// <param name="cmd_des"></param>
        /// <returns></returns>
        public int InsertOrUpdateData(string stcd, DateTime tm, double z, double q, double xsa, double xsavv, TConnect conn_des)
        {
            if (string.IsNullOrEmpty(stcd) || tm==null || conn_des == null || conn_des.Conn==null || conn_des.Conn.State == ConnectionState.Closed)
            {
                return 0;
            }
            string exec_sql;
            string tm_str = tm.ToString("yyyy-MM-dd HH:mm:ss");
            string z_str = z == -999999 ? "null" : z.ToString(); // drp可能为null值
            string q_str = q == -999999 ? "null" : q.ToString();
            string xsa_str = xsa == -999999 ? "null" : xsa.ToString();
            string xsavv_str = xsavv == -999999 ? "null" : xsavv.ToString();

            string check_sql = string.Format("select z, q, xsa, xsavv from ST_RIVER_R where stcd='{0}' and tm='{1}'", stcd, tm_str);
            DataTable dt = conn_des.GetTable(check_sql);
            if (dt==null)
            {
                return 0;
            }
            if (dt.Rows.Count==0)
            {
                // 不存在，需要插入
                exec_sql = string.Format("insert into ST_RIVER_R(stcd, tm, z, q, xsa, xsavv, flag) values('{0}','{1}',{2},{3},{4},{5},0)", stcd, tm_str, z_str, q_str, xsa_str, xsavv_str);
            }
            else
            {
                DataRow row = dt.Rows[0];
                // 已存在，则先比较，drp雨量不同则进行更新
                double z_old = row.IsNull("z") ? -999999 : double.Parse(row["z"].ToString());
                double q_old = row.IsNull("q") ? -999999 : double.Parse(row["q"].ToString());
                double xsa_old = row.IsNull("xsa") ? -999999 : double.Parse(row["xsa"].ToString());
                double xsavv_old = row.IsNull("xsavv") ? -999999 : double.Parse(row["xsavv"].ToString());
                if (z_old == z && q_old == q && xsa_old == xsa && xsavv_old == xsavv) return 1; // 数据是一样的，不用处理
                // 不一样则进行更新
                exec_sql = string.Format("update ST_RIVER_R set z={0}, q={1}, xsa={2}, xsavv={3} where stcd='{4}' and tm='{5}'", z_str, q_str, xsa_str, xsavv_str, stcd, tm_str);
            }

            return conn_des.ExecuteQuery(exec_sql);
        }

        /// <summary>
        /// 执行雨量数据传输同步
        /// </summary>
        /// <param name="bgtime"></param>
        /// <param name="edtime"></param>
        /// <param name="conn_source"></param>
        /// <param name="conn_des"></param>
        /// <returns></returns>
        public DateTime ExecTableTrans(List<string> stcds, DateTime bgtime, DateTime edtime, TConnect conn_source, TConnect conn_des)
        {
            if (bgtime == null || edtime == null || bgtime > edtime || conn_source == null || conn_des==null || 
                conn_source.Conn.State!=ConnectionState.Open || conn_des.Conn.State!=ConnectionState.Open)
            {
                return DateTime.MinValue;
            }

            DateTime maxDateTime = DateTime.MinValue; // 传输数据中最大的时间值

            /// 1、首先初始化传输队列，把所有雨量站指定时间段都压入队列
            /// 2、遍历队列，获取数据，对于时间段内数据不完整的，重试次数+1，当重试次数不超过3时，将对应的站码和时间段压入一个临时队列，这部分需要在下个同步中再做一次
            /// 3、对比数据，进行插入或更新
            /// 4、遍历完后，将临时队列合并进来，便于下一次继续同步

            InitTodoTransQueue(stcds, bgtime, edtime);

            // 临时队列
            Queue<TransObject> nextTodoQueue = new Queue<TransObject>();
            while (TodoTransQueue.Count>0)
            {
                TransObject todo = TodoTransQueue.Dequeue();
                string selectSql = GetSelectSql(todo.Stcd, todo.BgTime, todo.EdTime);
                if (string.IsNullOrEmpty(selectSql))
                {
                    continue;
                }

                DataTable dt = conn_source.GetTable(selectSql);
                if (dt==null || dt.Rows.Count==0)
                {
                    // 没有获取到数据，等待下一次重试                    
                    if (todo.RetryCount < 3)
                    {
                        todo.RetryCount++;
                        nextTodoQueue.Enqueue(todo);
                    }                    
                    continue;
                }

                int count_source = dt.Rows.Count;
                int count_trans = 0;
                int count_theory = getTheoryDataCount(todo.BgTime, todo.EdTime);

                foreach (DataRow row in dt.Rows)
                {
                    string stcd = row["stcd"].ToString();
                    DateTime tm = Convert.ToDateTime(row["tm"].ToString());
                    double z = row.IsNull("z") ? -999999 : double.Parse(row["z"].ToString());
                    double q = row.IsNull("q") ? -999999 : double.Parse(row["q"].ToString());
                    double xsa = row.IsNull("xsa") ? -999999 : double.Parse(row["xsa"].ToString());
                    double xsavv = row.IsNull("xsavv") ? -999999 : double.Parse(row["xsavv"].ToString());

                    int change = InsertOrUpdateData(stcd, tm, z, q, xsa, xsavv, conn_des);
                    count_trans += change;

                    maxDateTime = tm > maxDateTime ? tm : maxDateTime;
                }

                if (count_source < count_theory || count_trans < count_source)
                {
                    // 有部分数据没能传输过去，可能是数据库连接问题
                    // 则加入下一次的重试队列
                    if(todo.RetryCount < 3)
                    {
                        todo.RetryCount++;
                        nextTodoQueue.Enqueue(todo);
                    }
                }

                // 触发事件，通知更新进度条
                transProgressEvent(null, null);
            }
            // 将临时重试队列添加到todo队列
            AddTodoTrans(nextTodoQueue);
            return maxDateTime;
        }

        private int getTheoryDataCount(DateTime bgtime, DateTime edtime)
        {
            int span = 5; // 5分钟为间隔
            if (bgtime == null || edtime == null || bgtime > edtime)
            {
                return 0;
            }
            TimeSpan time = edtime.Subtract(bgtime);
            //int spanMinutes = time.Days * 24 * 60 + time.Hours * 60 + time.Minutes;  //将时间间隔换算成分钟数
            int spanMinutes = (int)time.TotalMinutes;
            return spanMinutes / span;
        }
    }
}
