using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace XXF.Db.DbPartitionConfig.Model
{
    /// <summary>
    /// tb_dbpartition_config Data Structure.
    /// </summary>
    [Serializable]
    public partial class tb_dbpartition_config_model
    {
	/*代码自动生成工具自动生成 - 车毅*/
        
        /// <summary>
        /// 自增
        /// </summary>
        public int id { get; set; }
        
        /// <summary>
        /// 分区id（分区标识号）
        /// </summary>
        public int partitionno { get; set; }
        
        /// <summary>
        /// 分区类型(1:用户分区,2:商户表分区)
        /// </summary>
        public Byte partitiontype { get; set; }
        
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string dbserver { get; set; }
        
        /// <summary>
        /// 数据库名
        /// </summary>
        public string dbname { get; set; }
        
        /// <summary>
        /// 用户名
        /// </summary>
        public string dbuser { get; set; }
        
        /// <summary>
        /// 密码
        /// </summary>
        public string dbpass { get; set; }

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