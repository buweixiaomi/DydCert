using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace XXF.Db.DbSubscribeRuleConfig.Model
{
    /// <summary>
    /// tb_dbsubscribe_config Data Structure.
    /// </summary>
    [Serializable]
    public partial class tb_dbsubscribe_config_model
    {
	/*代码自动生成工具自动生成,不要在这里写自己的代码，否则会被自动覆盖哦 - 车毅*/
        
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string partitionno { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int partitiontype { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string dbserver { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string dbname { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string dbuser { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string dbpass { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int dbtype { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime lastUpdateTime { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool isDel { get; set; }
        
    }
}