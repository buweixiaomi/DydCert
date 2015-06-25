using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XXF.Db.DbPartitionConfig.Model;

namespace XXF.Db
{
    /// <summary>
    /// 分区规则
    /// </summary>
    public class DbAreaRule
    {
        /// <summary>
        /// 获取商户地区分区规则
        /// 默认{PartitionNo}，被替换成分区编号
        /// </summary>
        /// <param name="dqbm">地区编码（邮政编码）</param>
        /// <returns>返回连接字符串</returns>
        public static string ShopAreaPartitionRule(string connectstringTemplate, int dqbm)
        {
            string partitionNo = dqbm.ToString();
            var configs = DbPartitionConfig.DbPartitionConfigHelper.GetCacheConfig();
            var o = configs.FirstOrDefault(c => c.partitiontype == (byte)EnumPartitionType.shop && c.partitionno.ToString() == partitionNo);
            return ReplaceConnectStringTemplate(connectstringTemplate, o);
        }

        /// <summary>
        /// 获取商户地区分区规则
        /// 默认{PartitionNo}，被替换成分区编号
        /// </summary>
        /// <param name="dqbm">商户账号</param>
        /// <returns>返回连接字符串</returns>
        public static string ShopAreaPartitionRule(string connectstringTemplate, string shzh)
        {
            int partitionNo = ShzhToPartitionNo(shzh);
            return ShopAreaPartitionRule(connectstringTemplate, partitionNo);
        }


        /// <summary>
        /// 所有商户分区的链接 
        /// </summary>
        /// <param name="connectstringTemplate"></param>
        /// <returns></returns>
        public static List<string> ShopAreaPartitionRuleList(string connectstringTemplate)
        {
            var configs = DbPartitionConfig.DbPartitionConfigHelper.GetCacheConfig();
            var o = configs.Where(c => c.partitiontype == (byte)EnumPartitionType.shop);
            List<string> results = new List<string>();
            foreach (var m in o)
            {
                results.Add(ReplaceConnectStringTemplate(connectstringTemplate, m));
            }
            return results;
        }
    


        /// <summary>
        /// 商户账号取分区号
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
        /// <param name="tid"></param>
        /// <returns></returns>
        public static int TidToPartitionNo(long tid)
        {
            return Convert.ToInt32(tid.ToString().Substring(0, 6));
        }

        /// <summary>
        /// 获取用户分区规则
        /// 默认{PartitionNo}，被替换成分区编号
        /// </summary>
        /// <param name="connectstringTemplate"></param>
        /// <param name="userid">用户自增id</param>
        /// <returns></returns>
        public static string UserAreaPartitionRule(string connectstringTemplate, long userid)
        {
            //10万一个库
            string partitionNo = userid / 100000 + 1 + "";
            return UserAreaPartitionRule(connectstringTemplate, partitionNo);
        }

        /// <summary>
        /// 获取用户分区规则
        /// 默认{PartitionNo}，被替换成分区编号
        /// </summary>
        /// <param name="connectstringTemplate"></param>
        /// <param name="id">userareaid,第几个userarea库</param>
        /// <returns></returns>
        public static string UserAreaPartitionRule(string connectstringTemplate, string id)
        {
            //10万一个库
            string partitionNo = id;
            var configs = DbPartitionConfig.DbPartitionConfigHelper.GetCacheConfig();
            var o = configs.FirstOrDefault(c => c.partitiontype == (byte)EnumPartitionType.user && c.partitionno.ToString() == partitionNo);
            return ReplaceConnectStringTemplate(connectstringTemplate, o);
        }

        /// <summary>
        /// 所有商户分区的链接 
        /// </summary>
        /// <param name="connectstringTemplate"></param>
        /// <returns></returns>
        public static List<string> UserAreaPartitionRuleList(string connectstringTemplate)
        {
            var configs = DbPartitionConfig.DbPartitionConfigHelper.GetCacheConfig();
            var o = configs.Where(c => c.partitiontype == (byte)EnumPartitionType.user);
            List<string> results = new List<string>();
            foreach (var m in o)
            {
                results.Add(ReplaceConnectStringTemplate(connectstringTemplate, m));
            }
            return results;
        }

        /// <summary>
        /// 根据模板替换连接字符串
        /// </summary>
        /// <param name="connectstringTemplate"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ReplaceConnectStringTemplate(string connectstringTemplate, DbPartitionConfig.Model.tb_dbpartition_config_model o)
        {
            if (o == null)
                throw new Exception("找不到数据库连接分库配置");

            return connectstringTemplate.Replace("{partitionno}", o.partitionno + "").Replace("{dbserver}", o.dbserver).Replace("{dbname}", o.dbname)
                            .Replace("{dbuser}", o.dbuser).Replace("{dbpass}", o.dbpass);
        }

        /// <summary>
        /// 按月分表规则
        /// 返回:_+yyyMM 举例:_201407
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string MonthTable(DateTime date)
        {
            return "_" + date.Date.ToString("yyyyMM");
        }
    }
}
