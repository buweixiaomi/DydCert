using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

namespace CertCenter.Models.DbModels
{
    /// <summary>
    /// certcenterlog Data Structure.
    /// </summary>
    [Serializable]
    public partial class certcenterlog
    {
	/*代码自动生成工具自动生成 - 车毅*/
        
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string url { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string reqdata { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string userid { get; set; }


        /// <summary>
        /// 用户名
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime reqtime { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string ip { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string opecontent { get; set; }

    }

    public partial class certcenterlog
    {
        public certcenterlog()
        {
            //nothing to do
        }

        public certcenterlog(Controller controller)
        {
            this.url = controller.Request.Url.ToString();
            this.reqdata = System.Web.HttpUtility.UrlDecode(controller.Request.Form.ToString());
            this.reqtime = DateTime.Now;
            this.userid = controller.User.Identity.Name.Split(' ').FirstOrDefault();
            this.username = controller.User.Identity.Name.Split(' ').LastOrDefault();
            this.ip = getRequestIp(controller);
            this.opecontent = string.Empty;
        }

        public string getRequestIp(Controller controller)
        {
            string userIP = "";
            if (controller.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != "")
                userIP = controller.Request.ServerVariables["REMOTE_ADDR"];
            else
                userIP = controller.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (userIP == null || userIP == "")
                userIP = controller.Request.UserHostAddress;
            return userIP ?? "";
        }
    }
}