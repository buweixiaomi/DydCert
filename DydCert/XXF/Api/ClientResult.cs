using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace XXF.Api
{
    /// <summary>
    /// 与客户端通信协议
    /// </summary>
    public class ClientResult
    {
        /// <summary>请求回复状态码</summary>
        public int code { get; set; }
        /// <summary>请求是存成功</summary>
        public bool success
        {
            get
            {
                return code > 0;
            }
        }
        /// <summary>
        /// 状态码
        /// </summary>
        public int statuscode { get; set; }

        private JObject _repObject;
        /// <summary>
        /// 请求输出的json object对象
        /// </summary>
        public JObject repObject
        {
            get
            {
                return _repObject;
            }
            set
            {
                _repObject = value;
            }
        }
        private string _resString = string.Empty;
        /// <summary>
        /// 请求输出的json字符串
        /// </summary>
        public string resString
        {
            get
            {
                if (string.IsNullOrEmpty(_resString))
                {
                    if (!string.IsNullOrEmpty(msg))
                    {
                        return msg;
                    }
                }
                return _resString;
            }
            set
            {
                _resString = value;
            }
        }

        /// <summary>
        /// 请求返回的信息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 返回结果的列表数
        /// </summary>
        public long total { get; set; }
        /// <summary>
        /// 请求输出流
        /// </summary>
        public Stream responseStream { get; set; }

        /// <summary>
        /// 返回内容类型 小写格式
        /// </summary>
        public string responseContentType { get; set; }
    }
}
