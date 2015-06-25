using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XXF.Db
{
    /// <summary>
    /// 数据库订阅规则
    /// </summary>
    public class DbSubscribeRule
    {
        /// <summary>
        /// 获取主库订阅库连接
        /// </summary>
        /// <returns></returns>
        public static string GetMainConnection(string connectstringTemplate, int subscribeType)
        {
            var configs = DbSubscribeRuleConfig.DbSubscribeRuleConfigHelper.GetCacheConfig(); // .GetCacheConfig();
            var o = configs.Where(c => c.partitiontype == (int)EnumPartitionType.main && c.dbtype==subscribeType).FirstOrDefault();
            return ReplaceConnectStringTemplate(connectstringTemplate, o);
        }

        /// <summary>
        /// 获取crm订阅库的订阅库连接
        /// </summary>
        /// <returns></returns>
        public static string GetCrmdyConnection(string connectstringTemplate, int subscribeType)
        {
            var configs = DbSubscribeRuleConfig.DbSubscribeRuleConfigHelper.GetCacheConfig(); // .GetCacheConfig();
            var o = configs.Where(c => c.partitiontype == (int)EnumPartitionType.crmdy && c.dbtype == subscribeType).FirstOrDefault();
            return ReplaceConnectStringTemplate(connectstringTemplate, o);
        }
        /// <summary>
        /// 获取日志库订阅库连接
        /// </summary>
        /// <returns></returns>
        public static string GetLogConnection(string connectstringTemplate, int subscribeType)
        {
            var configs = DbSubscribeRuleConfig.DbSubscribeRuleConfigHelper.GetCacheConfig(); // .GetCacheConfig();
            var o = configs.Where(c => c.partitiontype == (int)EnumPartitionType.log && c.dbtype == subscribeType).FirstOrDefault();
            return ReplaceConnectStringTemplate(connectstringTemplate, o);
        }

        /// <summary>
        /// 获取商户地区分区规则
        /// 默认{PartitionNo}，被替换成分区编号
        /// </summary>
        /// <param name="dqbm">地区编码（邮政编码）</param>
        /// <returns>返回连接字符串</returns>
        public static string ShopAreaPartitionRule(string connectstringTemplate, int dqbm, int subscribeType)
        {
            string partitionNo = dqbm.ToString();
            var configs = DbSubscribeRuleConfig.DbSubscribeRuleConfigHelper.GetCacheConfig(); 
            var o = configs.Where(c => c.partitiontype == (byte)EnumPartitionType.shop && c.partitionno.ToString() == partitionNo&&c.dbtype==subscribeType).FirstOrDefault();
            return ReplaceConnectStringTemplate(connectstringTemplate, o);
        }

        /// <summary>
        /// 获取商户地区分区规则
        /// 默认{PartitionNo}，被替换成分区编号
        /// </summary>
        /// <param name="dqbm">商户账号</param>
        /// <returns>返回连接字符串</returns>
        public static string ShopAreaPartitionRule(string connectstringTemplate, string shzh,int subscribeType)
        {
            int partitionNo = ShzhToPartitionNo(shzh);
            return ShopAreaPartitionRule(connectstringTemplate, partitionNo, subscribeType);
        }


        /// <summary>
        /// 所有商户分区的链接 
        /// </summary>
        /// <param name="connectstringTemplate"></param>
        /// <param name="subscribeType"></param>
        /// <returns></returns>
        public static List<string> ShopAreaPartitionRuleList(string connectstringTemplate, int subscribeType)
        {
            var configs = DbSubscribeRuleConfig.DbSubscribeRuleConfigHelper.GetCacheConfig();
            var o = configs.Where(c => c.partitiontype == (byte)EnumPartitionType.shop && c.dbtype == subscribeType && !string.IsNullOrWhiteSpace(c.partitionno));
            List<string> results = new List<string>();
            foreach (var m in o)
            {
                results.Add(ReplaceConnectStringTemplate(connectstringTemplate, m));
            }
            return results;
        }
    

        /// <summary>
        /// 根据商户账号获取分区号
        /// </summary>
        /// <param name="shzh"></param>
        /// <returns></returns>
        public static int ShzhToPartitionNo(string shzh)
        {
            return Convert.ToInt32(shzh.Substring(0, 6));
        }
     
        /// <summary>
        /// 根据订单id获取分区号
        /// </summary>
        public static int TidToPartitionNo(long tid)
        {
            return Convert.ToInt32(tid.ToString().Substring(0, 6));
        }

        /// <summary>
        /// 获取用户分区规则 汇总库
        /// </summary>
        /// <param name="connectstringTemplate"></param>
        /// <param name="subscribeType">订阅库类型</param>
        /// <returns></returns>
        public static string UserAreaPartitionRule(string connectstringTemplate, int subscribeType)
        {
            string PartitionNo = "";
            return UserAreaPartitionRule(connectstringTemplate, PartitionNo, subscribeType);
        }

        /// <summary>
        /// 获取用户分区规则
        /// 默认{PartitionNo}，被替换成分区编号
        /// </summary>
        /// <param name="connectstringTemplate"></param>
        /// <param name="userid">用户自增id</param>
        /// <param name="subscribeType">订阅库类型</param>
        /// <returns></returns>
        public static string UserAreaPartitionRule(string connectstringTemplate, long userid, int subscribeType)
        {
            //10万一个库
            string PartitionNo = userid / 100000 + 1 + "";
            return UserAreaPartitionRule(connectstringTemplate, PartitionNo, subscribeType);
        }

        /// <summary>
        /// 获取用户分区规则
        /// 默认{PartitionNo}，被替换成分区编号
        /// </summary>
        /// <param name="connectstringTemplate"></param>
        /// <param name="dqbm">地区编码</param>
        /// <param name="subscribeType">订阅库类型</param>
        /// <returns></returns>
        public static string UserAreaPartitionRule(string connectstringTemplate, string  dqbm, int subscribeType)
        {
            //10万一个库
            var configs = DbSubscribeRuleConfig.DbSubscribeRuleConfigHelper.GetCacheConfig();
            var o = configs.Where(c => c.partitiontype == (byte)EnumPartitionType.user && c.partitionno.ToString() == dqbm && c.dbtype == subscribeType).FirstOrDefault();
            return ReplaceConnectStringTemplate(connectstringTemplate, o);
        }

        /// <summary>
        /// 所有用户分区的链接 去除汇总库
        /// </summary>
        /// <param name="connectstringTemplate"></param>
        /// <param name="subscribeType"></param>
        /// <returns></returns>
        public static List<string> UserAreaPartitionRuleList(string connectstringTemplate, int subscribeType)
        {
            var configs = DbSubscribeRuleConfig.DbSubscribeRuleConfigHelper.GetCacheConfig();
            var o = configs.Where(c => c.partitiontype == (byte)EnumPartitionType.user&&c.dbtype==subscribeType&&!string.IsNullOrWhiteSpace(c.partitionno));
            List<string> results = new List<string>();
            foreach (var m in o)
            {
                results.Add(ReplaceConnectStringTemplate(connectstringTemplate,m));
            }
            return results;
        }
    
        /// <summary>
        /// 根据连接字符串模板获取连接字符串
        /// </summary>
        public static string ReplaceConnectStringTemplate(string connectstringTemplate, DbSubscribeRuleConfig.Model.tb_dbsubscribe_config_model o)
        {
            if (o == null)
                throw new Exception("找不到订阅数据库连接分库配置");

            return connectstringTemplate.Replace("{partitionno}", o.partitionno + "").Replace("{dbserver}", o.dbserver).Replace("{dbname}", o.dbname)
                            .Replace("{dbuser}", o.dbuser).Replace("{dbpass}", o.dbpass);

        }
    }
}
