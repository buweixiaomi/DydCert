using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XXF.Db.DbShuntRuleConnConfig.Model;
using XXF.Log;

namespace XXF.Db.DbShuntRuleConnConfig
{
    /// <summary>
    /// Db分区配置帮助类
    /// </summary>
    public class DbShuntRuleConnConfigHelper
    {
        //缓存key标识
        private static string configKey = "DbShuntRuleConnConfig"; //+ DateTime.Now.ToString("yyyyMMddHHmmss");

        private static readonly int intervalTime = 10;//单位秒
        public static Dictionary<string, tb_shuntruleconn_config_model> configData = new Dictionary<string, tb_shuntruleconn_config_model>();
        public static XXF.Cache.ConfigCacheProvider<List<tb_shuntruleconn_config_model>> configCacheProvider = new XXF.Cache.ConfigCacheProvider<List<tb_shuntruleconn_config_model>>(configKey, RefreshData, RefreshDataByLocalFile, intervalTime);
      


        //static DbShuntRuleConnConfigHelper()
        //{
        //    try
        //    {
        //        //注册缓存刷新回调和初始化缓存
        //        var data = RefreashDbShuntRuleConnConfigCache();
        //        bool r = Cache.CacheFileProvider.Register(cachekey, new Cache.CacheFileInfo { IintervalTime = 1000 * 5, CacheData = data, RefreashMethod = DbShuntRuleConnConfigHelper.RefreashDbShuntRuleConnConfigCache });
        //        if (r == false)
        //        {
        //            ErrorLog.Write("注册db分区配置缓存失败", null);
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        ErrorLog.Write("注册db分区配置出错", exp);
        //    }
        //}


        /// <summary>
        /// 刷新数据从数据库
        /// </summary>
        /// <returns></returns>
        private static List<tb_shuntruleconn_config_model> RefreshData(DateTime lastRefeshTime)
        {
            List<tb_shuntruleconn_config_model> models = new List<tb_shuntruleconn_config_model>();
            using (var conn = Db.DbConn.CreateConn(DbType.SQLSERVER, XXF.Common.XXFConfig.ConfigConnectString))
            {
                conn.Open();
                //   int type=LibConvert.StrToInt(XXF.Common.XXFConfig.MainConnectString):
                models = new Dal.tb_shuntruleconn_config_dal().Get(conn,lastRefeshTime);
            }

            if (configData == null)
            {
                configData = new Dictionary<string, tb_shuntruleconn_config_model>();
            }
            foreach (var m in models)
            {
                string dicKey = GetDicKey(m.id.ToString());
                if (m.isDel)
                {
                    configData.Remove(dicKey);
                }
                else
                {
                    if (configData.ContainsKey(dicKey))
                    {
                        configData[dicKey] = m;
                    }
                    else
                    {
                        configData.Add(dicKey, m);
                    }
                }
            }

            List<tb_shuntruleconn_config_model> data = new List<tb_shuntruleconn_config_model>(configData.Values);
            return data;
        }

        /// <summary>
        /// 刷新数据从本地文件
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        private static void RefreshDataByLocalFile(List<tb_shuntruleconn_config_model> dic)
        {
            if (dic != null && dic.Count > 0)
            {
                configData.Clear();
                foreach (var m in dic)
                {
                    configData.Add(GetDicKey(m.id.ToString()), m);
                }
            }
        }

        /// <summary>
        /// 得到key值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dqbm"></param>
        /// <returns></returns>
        private static string GetDicKey(string key)
        {
            string keyTemp = "DbShuntRuleConnConfig_{0}";
            return string.Format(keyTemp, key);
        }
         

        ///// <summary>
        ///// 缓存刷新回调
        ///// </summary>
        ///// <returns></returns>
        //private static object RefreashDbShuntRuleConnConfigCache()
        //{
        //    using (var conn = Db.DbConn.CreateConn(DbType.SQLSERVER, XXF.Common.XXFConfig.MainConnectString))
        //    {
        //        conn.Open();
        //        var list = new Dal.tb_shuntruleconn_config_dal().Get(conn);
        //        return list;
        //    }
        //}
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <returns></returns>
        public static List<Model.tb_shuntruleconn_config_model> GetCacheConfig()
        {
            if (configData.Count() == 0)
            {
                return new List<tb_shuntruleconn_config_model>();
            }
            List<tb_shuntruleconn_config_model> models = new List<tb_shuntruleconn_config_model>(configData.Values);
            return models;
            //var o = Cache.CacheFileProvider.GetCache(cachekey);
            //if(o == null)
            //    return new List<Model.tb_shuntruleconn_config_model>();
            //else
            //    return (o as Cache.CacheFileInfo).CacheData as List<Model.tb_shuntruleconn_config_model>;
        }
    }
}
