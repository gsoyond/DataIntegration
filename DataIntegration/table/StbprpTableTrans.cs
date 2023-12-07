using DataIntegration.lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DataIntegration.table
{
    /// <summary>
    /// 测站基本信息同步
    /// 1天同步1次
    /// </summary>
    public class StbprpTableTrans
    {
        /// 通过先删除后插入的方式来同步
        /// 1、truncate table，删除数据
        /// 2、通过sql bulk进行批量插入
        /// 3、以事务方式运行

        public string GetSelectSql()
        {
            string sql = "select STCD,STNM,RVNM,HNNM,BSNM,LGTD,LTTD,STLC,ADDVCD,DTMNM,DTMEL,DTPR,STTP,FRGRD,ESSTYM,BGFRYM,ATCUNIT,ADMAUTH,LOCALITY,STBK,STAZT,DSTRVM,DRNA,PHCD,USFL,COMMENTS,MODITIME from ST_STBPRP_B";

            return sql;
        }

        public bool ExecTableTrans(TConnect conn_source, TConnect conn_des)
        {
            if (conn_source == null || conn_des == null ||
                conn_source.Conn.State != ConnectionState.Open || conn_des.Conn.State != ConnectionState.Open)
            {
                return false;
            }

            // 先取数据
            string selectSql = GetSelectSql();
            DataTable dt = conn_source.GetTable(selectSql);
            if (dt == null || dt.Rows.Count == 0)
            {
                return false;
            }

            using (SqlTransaction transaction = conn_des.Conn.BeginTransaction())
            {

                // 删除数据
                string truncateSql = "truncate table ST_STBPRP_B";
                conn_des.ExecuteQuery(truncateSql , transaction);
                // 使用bulk批量插入
                try
                {
                    SqlBulkCopy sqlbulkcopy = new SqlBulkCopy(conn_des.Conn, SqlBulkCopyOptions.Default, transaction);
                    sqlbulkcopy.BatchSize = 100;
                    sqlbulkcopy.DestinationTableName = "ST_STBPRP_B";
                    sqlbulkcopy.WriteToServer(dt);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return true;
        }

    }
}
