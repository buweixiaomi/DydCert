using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XXF.Api
{
    /// <summary>
    /// 返回对象
    /// </summary>
    public class ServerResult
    {
        /// <summary>
        /// 返回值 1成功 -1失败 33无权限
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 消息返回
        /// </summary>
        private string _msg = string.Empty;
        public string msg { get { return _msg; } set { _msg = value; } }
        /// <summary>
        /// 接受对象
        /// </summary>
        public object response { get; set; }
        /// <summary>
        /// 如列表，列表总数
        /// </summary>
        public int total { get; set; }

        public long servertime { get { return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds; } }
    }
}
