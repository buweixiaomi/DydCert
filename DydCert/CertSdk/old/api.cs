using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertSdk.old.CertCenter
{
    /// <summary>
    /// api Data Structure.
    /// </summary>
    [Serializable]
    public class api : ICloneable
    {
        /*代码自动生成工具自动生成 - 车毅*/

        /// <summary>
        /// 权限id
        /// </summary>
        public int apiid { get; set; }

        /// <summary>
        /// 应用类型
        /// </summary>
        public int apptype { get; set; }

        /// <summary>
        /// 应用级别
        /// </summary>
        public int appgradeno { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int categoryid { get; set; }

        /// <summary>
        /// 接口名
        /// </summary>
        public string apiname { get; set; }

        /// <summary>
        /// 接口标题
        /// </summary>
        public string apititle { get; set; }

        /// <summary>
        /// area
        /// </summary>
        public string area { get; set; }

        /// <summary>
        /// controller
        /// </summary>
        public string controller { get; set; }

        /// <summary>
        /// action
        /// </summary>
        public string action { get; set; }

        /// <summary>
        /// para
        /// </summary>
        public string para { get; set; }

        /// <summary>
        /// api说明
        /// </summary>
        public string apidesc { get; set; }

        /// <summary>
        /// 是否冻结
        /// </summary>
        public int freeze { get; set; }

        public object Clone()
        {
            return new api()
            {
                action = this.action,
                apidesc = this.apidesc,
                apiid = this.apiid,
                apiname = this.apiname,
                apititle = this.apititle,
                appgradeno = this.appgradeno,
                apptype = this.apptype,
                area = this.area,
                categoryid = this.categoryid,
                controller = this.controller,
                freeze = this.freeze,
                para = this.para
            };
        }
    }
}
