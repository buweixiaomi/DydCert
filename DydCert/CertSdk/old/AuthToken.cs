using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertSdk.old.CertCenter
{
    public class AuthToken : ICloneable
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

        
        public System.DateTime lastauth { get; set; }

        /// <summary>
        /// close this obj to another instance
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            AuthToken obj = new AuthToken()
            {
                appid = this.appid,
                createtime = this.createtime,
                expires = this.expires,
                lastauth = this.lastauth,
                token = this.token,
                userid = this.userid,
                username = this.username,
                id = this.id
            };
            return obj;
        }

    }
}
