using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DataIntegration.lib;
using System.Data.SqlClient;

namespace DataIntegration.util
{
    class TRelationsManager
    {

        /// <summary>
        /// 获取测站清单,下面转数据时针对每一个测站进行的
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, TRelation> GetStcdList(String ch, String connName)
        {
            Dictionary<string, TRelation> stcdList = null;
            TConnect conn = new TConnect(connName);
            string sql = "select stcd, gprscd, wavecd, hiszcd, hispcd, hisrcd, conncd, FORUMULA from HD_RELATION_A where sttp like '%"+ch+"%'"; //从0开始数
            try
            {
                SqlDataReader dr = conn.ExecuteReader(sql);

                stcdList = new Dictionary<string, TRelation>();

                while (dr.Read())
                {
                    string stcd    = dr.GetString(0).Trim();
                    if (!stcdList.Keys.Contains(stcd))
                    {
                        string gprscd = (dr.IsDBNull(1) || dr.GetString(1).Trim() == "") ? null : dr.GetString(1).Trim();
                        string wavecd = (dr.IsDBNull(2) || dr.GetString(2).Trim() == "") ? null : dr.GetString(2).Trim();
                        string hiszcd = (dr.IsDBNull(3) || dr.GetString(3).Trim() == "") ? null : dr.GetString(3).Trim();
                        string hispcd = (dr.IsDBNull(4) || dr.GetString(4).Trim() == "") ? null : dr.GetString(4).Trim();
                        string hisrcd = (dr.IsDBNull(5) || dr.GetString(5).Trim() == "") ? null : dr.GetString(5).Trim();
                        string conncd = (dr.IsDBNull(6) || dr.GetString(6).Trim() == "") ? null : dr.GetString(6).Trim();
                        string formula = (dr.IsDBNull(7) || dr.GetString(7).Trim() == "") ? null : dr.GetString(7).Trim();

                        TRelation relation = new TRelation();
                        relation.Stcd = stcd;               //主STCD
                        relation.Gprs_stcd = gprscd;
                        relation.Wave_stcd = wavecd;
                        relation.Hisz_stcd = hiszcd;
                        relation.Hisr_stcd = hispcd;
                        relation.Hisp_stcd = hispcd;
                        relation.Conn_stcd = conncd;
                        relation.Formula = formula;

                        if ((!(gprscd == null)) && (wavecd == null))
                        {
                            relation.State = "10";
                        }
                        else if ((!(gprscd == null)) && (!(wavecd == null)))
                        {
                            relation.State = "11";
                        }
                        else if ((gprscd == null) && (!(wavecd == null)))
                        {
                            relation.State = "01";
                        }

                        relation.Last_p = -1;   //降雨量
                        relation.Last_q = -1;   //流量
                        relation.Last_rz = -1;   //水库水位
                        relation.Last_tdz = -1;   //潮位
                        relation.Last_z = -1;   //河道水位

                        relation.LastTime_p = Global.pptnLastTime;
                        relation.LastTime_z = Global.riverLastTime;
                        relation.LastTime_rz = Global.rvsrLastTime;
                        relation.LastTime_tdz = Global.tideLastTime;


                        stcdList.Add(stcd, relation);
                    }

                    
                }
                dr.Close();
            }
            catch
            {              
                stcdList = null;
            }

            conn.CloseConn();
            return stcdList;
        }
    }
}
