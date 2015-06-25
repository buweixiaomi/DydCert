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
	/*�����Զ����ɹ����Զ�����,��Ҫ������д�Լ��Ĵ��룬����ᱻ�Զ�����Ŷ - ����*/
        
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
        /// ������ʱ��
        /// </summary>
        public DateTime lastUpdateTime { get; set; }

        /// <summary>
        /// �Ƿ�ɾ��
        /// </summary>
        public bool isDel { get; set; }
        
    }
}