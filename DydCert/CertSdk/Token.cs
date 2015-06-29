using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CertSdk
{
    public class Token
    {
        /// <summary>
        ///授权token
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// 唯一标识 用户时为：自增id，其它为userid
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public string userid { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// 应用id
        /// </summary>
        public string appid { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime createtime { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public System.DateTime expires { get; set; }
    }
}
