using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using DataIntegration.lib;
using DataIntegration.util;

namespace DataIntegration.xml
{
    class TXML
    {
        private static string configFile = System.Environment.CurrentDirectory + @"\config\config.xml";

        public TXML()
        {
        }


        public static TSystemConfig GetSytemConfig(string name)
        {
            TSystemConfig sysConfig = new TSystemConfig();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(configFile);
            XmlNode xnCom = xmlDoc.SelectSingleNode("System").SelectSingleNode(name);
            if (xnCom != null)
            {
                XmlNode xnLastTime_pptn = xnCom.SelectSingleNode("pptnLastTime");
                sysConfig.PptnLastTime = xnLastTime_pptn.InnerText;
                XmlNode xnLastTime_river = xnCom.SelectSingleNode("riverLastTime");
                sysConfig.RiverLastTime = xnLastTime_river.InnerText;
                XmlNode xnLastTime_rvsr = xnCom.SelectSingleNode("rvsrLastTime");
                sysConfig.RvsrLastTime = xnLastTime_rvsr.InnerText;
                XmlNode xnLastTime_tide = xnCom.SelectSingleNode("tideLastTime");
                sysConfig.TideLastTime = xnLastTime_tide.InnerText;

                XmlNode xnInterval = xnCom.SelectSingleNode("interval");
                sysConfig.Interval = Convert.ToInt32(xnInterval.InnerText);
                XmlNode xnDataItr = xnCom.SelectSingleNode("dataItr");
                sysConfig.DataItr = Convert.ToInt32(xnDataItr.InnerText);
                XmlNode xnRecordRate = xnCom.SelectSingleNode("recordRate");
                sysConfig.RecordRate = Convert.ToSingle(xnRecordRate.InnerText);
                return sysConfig;
            }
            else
            {
                return null;
            }
        }


        public static void SaveLastTimeOfSystemConfig(string name, string tag, string value)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(configFile);

            XmlNode xnCom = xmlDoc.SelectSingleNode("System").SelectSingleNode(name);
            if (xnCom != null)
            {
                XmlNode xn = xnCom.SelectSingleNode(tag);
                xn.InnerText = value;

            }
            xmlDoc.Save(configFile);//保存
        }


        public static void UpdateLastTime(String pptnLastTime, String riverLastTime, String rvsrLastTime, String tideLastTime)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(configFile);

            XmlNode xnCom = xmlDoc.SelectSingleNode("System").SelectSingleNode("Config");
            if (xnCom != null)
            {
                XmlNode xn = xnCom.SelectSingleNode("pptnLastTime");
                xn.InnerText = pptnLastTime;

                xn = xnCom.SelectSingleNode("riverLastTime");
                xn.InnerText = riverLastTime;

                xn = xnCom.SelectSingleNode("rvsrLastTime");
                xn.InnerText = rvsrLastTime;

                xn = xnCom.SelectSingleNode("tideLastTime");
                xn.InnerText = tideLastTime;

            }
            xmlDoc.Save(configFile);//保存
        }


        public static TConnectConfig GetDBConnfig(string name)
        {
            TConnectConfig connConfig = new TConnectConfig();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(configFile);
            XmlNode xnCom = xmlDoc.SelectSingleNode("System").SelectSingleNode(name);
            if (xnCom != null)
            {
                XmlNode xnIp = xnCom.SelectSingleNode("ip");
                connConfig.Ip = xnIp.InnerText;
                XmlNode xnUser = xnCom.SelectSingleNode("user");
                connConfig.User = xnUser.InnerText;
                XmlNode xnPasswd = xnCom.SelectSingleNode("passwd");
                connConfig.Passwd = xnPasswd.InnerText;
                XmlNode xnDbName = xnCom.SelectSingleNode("dbName");
                connConfig.DbName = xnDbName.InnerText;
                return connConfig;
            }
            else
            {
                return null;
            }

        }

        public static void SetDBConnfig(TConnectConfig cfg, string name)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(configFile);

            XmlNode xnCom = xmlDoc.SelectSingleNode("System").SelectSingleNode(name);
            if (xnCom != null)
            {
                XmlNode xnIp = xnCom.SelectSingleNode("ip");
                xnIp.InnerText = cfg.Ip;
                XmlNode xnUser = xnCom.SelectSingleNode("user");
                xnUser.InnerText = cfg.User;
                XmlNode xnPasswd = xnCom.SelectSingleNode("passwd");
                xnPasswd.InnerText = cfg.Passwd;
                XmlNode xnDbName = xnCom.SelectSingleNode("dbName");
                xnDbName.InnerText = cfg.DbName;
            }
            xmlDoc.Save(configFile);//保存
        }
    }
}
