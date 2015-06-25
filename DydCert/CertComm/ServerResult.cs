using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CertComm
{
    public class ServerResult
    {
        /// <summary>
        /// 状态码 默认为0
        /// </summary>
        public int code { get; set; }

        private string _msg = string.Empty;

        /// <summary>返回操作结果信息信息，如成功或错误原因</summary>
        public string msg
        {
            get
            {
                return _msg;
            }
            set
            {
                _msg = value;
            }
        }

        /// <summary>返回信息</summary>
        public object response { get; set; }

        /// <summary>如列表分页的返回总数量</summary>
        public int total { get; set; }
    }
}
