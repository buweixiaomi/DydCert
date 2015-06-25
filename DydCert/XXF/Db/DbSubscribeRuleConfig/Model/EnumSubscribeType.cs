using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XXF.Db.DbSubscribeRuleConfig.Model
{
    /// <summary>
    /// 订阅库类型
    /// </summary>
    public enum EnumSubscribeType
    {
        /// <summary>
        /// 订阅库类型 Stage订阅
        /// </summary>
        Stage=1,
        Second
    }

    /// <summary>
    /// 库类型枚举
    /// </summary>
    public enum EnumPartitionType
    {
        /// <summary>
        /// 商户库
        /// </summary>
        shop = 2,
        
        /// <summary>
        /// 用户库
        /// </summary>
        user = 1,
        
        /// <summary>
        /// 主库
        /// </summary>
        main=3,

        /// <summary>
        /// crm订阅库
        /// </summary>
        crmdy=4,

        /// <summary>
        ///验证码库 
        /// </summary>
        yzm=5,
        /// <summary>
        /// 日志表
        /// </summary>
        log=6,

        /// <summary>
        /// config库
        /// </summary>
        config=7,

    }
}
