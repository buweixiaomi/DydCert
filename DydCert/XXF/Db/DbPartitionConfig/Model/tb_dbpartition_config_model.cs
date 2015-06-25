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
	/*�����Զ����ɹ����Զ����� - ����*/
        
        /// <summary>
        /// ����
        /// </summary>
        public int id { get; set; }
        
        /// <summary>
        /// ����id��������ʶ�ţ�
        /// </summary>
        public int partitionno { get; set; }
        
        /// <summary>
        /// ��������(1:�û�����,2:�̻������)
        /// </summary>
        public Byte partitiontype { get; set; }
        
        /// <summary>
        /// ��������ַ
        /// </summary>
        public string dbserver { get; set; }
        
        /// <summary>
        /// ���ݿ���
        /// </summary>
        public string dbname { get; set; }
        
        /// <summary>
        /// �û���
        /// </summary>
        public string dbuser { get; set; }
        
        /// <summary>
        /// ����
        /// </summary>
        public string dbpass { get; set; }

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