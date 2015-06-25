using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace CertCenter.Models.DbModels
{
    /// <summary>
    /// apilog Data Structure.
    /// </summary>
    [Serializable]
    public partial class apilog
    {
	/*�����Զ����ɹ����Զ����� - ����*/
        
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        
        /// <summary>
        /// ������Դ 0:������ 1�̻� 2����
        /// </summary>
        public int reqsource { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string url { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string reqpara { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string ip { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string token { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string appid { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string userid { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string username { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DateTime reqdate { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string opecontent { get; set; }
        
    }
}