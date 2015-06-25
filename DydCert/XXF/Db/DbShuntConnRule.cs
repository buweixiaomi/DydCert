using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XXF.Db
{
    /// <summary>
    /// 数据库分流（数据库负载均衡）规则
    /// </summary>
    public class DbShuntConnRule
    {
        private static int GetMobileToLinkNum(string yhzh, int divisor)
        {
            int Num = Convert.ToInt32(yhzh.Substring(yhzh.Length - 2, 2)) % divisor + 1;
            return Num;
        }

        /// <summary>
        /// 获取用户分区规则
        /// 默认{PartitionNo}，被替换成分区编号
        /// </summary>
        /// <param name="connectstringTemplate"></param>
        /// <param name="userid">用户自增id</param>
        /// <returns></returns>
        public static string UserAreaPartitionRule(string connectstringTemplate, string yhzh,string shzh)
        {
            int RegionCode = ShzhToPartitionNo(shzh);
            var Cache = Db.DbShuntRuleConnConfig.DbShuntRuleConnConfigHelper.GetCacheConfig();
            if (!string.IsNullOrWhiteSpace(yhzh))
            {
                int Count = Cache.Where(c => c.regioncode == RegionCode && c.isDel != true).Count();
                int Num = GetMobileToLinkNum(yhzh, Count);
                var Connection = Cache.Where(c => c.partitionno == Num && c.regioncode == RegionCode).FirstOrDefault();
                return ReplaceConnectStringTemplate(connectstringTemplate, Connection);
            }
            else
            {
                var Connection = Cache.Where(c => c.partitionno == 1 && c.regioncode == RegionCode).FirstOrDefault();
                return ReplaceConnectStringTemplate(connectstringTemplate, Connection);
            }
        }

        /// <summary>
        /// 获取用户分区规则
        /// 默认{PartitionNo}，被替换成分区编号
        /// </summary>
        /// <param name="connectstringTemplate"></param>
        /// <param name="userid">用户自增id</param>
        /// <returns></returns>
        public static string UserAreaPartitionRule(string connectstringTemplate, string yhzh, int dqbm)
        {
            int RegionCode = dqbm;
            var Cache = Db.DbShuntRuleConnConfig.DbShuntRuleConnConfigHelper.GetCacheConfig();
            if (!string.IsNullOrWhiteSpace(yhzh))
            {
                int Count = Cache.Where(c => c.regioncode == RegionCode && c.isDel != true).Count();
                int Num = GetMobileToLinkNum(yhzh, Count);
                var Connection = Cache.Where(c => c.partitionno == Num && c.regioncode == RegionCode).FirstOrDefault();
                return ReplaceConnectStringTemplate(connectstringTemplate, Connection);
            }
            else
            {
                var Connection = Cache.Where(c => c.partitionno == 1 && c.regioncode == RegionCode).FirstOrDefault();
                return ReplaceConnectStringTemplate(connectstringTemplate, Connection);
            }
        }

        /// <summary>
        /// 商户账号转分区号
        /// </summary>
        /// <param name="shzh"></param>
        /// <returns></returns>
        public static int ShzhToPartitionNo(string shzh)
        {
            return Convert.ToInt32(shzh.Substring(0, 6));
        }

        /// <summary>
        /// 根据连接字符串模板替换连接字符串
        /// </summary>
        /// <param name="connectstringTemplate"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ReplaceConnectStringTemplate(string connectstringTemplate, DbShuntRuleConnConfig.Model.tb_shuntruleconn_config_model o)
        {
            if (o == null)
                throw new Exception("找不到负载均衡数据库连接分库配置");

            return connectstringTemplate.Replace("{partitionno}", o.regioncode + "").Replace("{dbserver}", o.dbserver).Replace("{dbname}", o.dbname)
                            .Replace("{dbuser}", o.dbuser).Replace("{dbpass}", o.dbpass);
        }
    }
}
