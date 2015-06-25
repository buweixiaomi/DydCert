using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.IO;

namespace CertComm
{
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

        public int statuscode { get; set; }

        private JObject _repObject;
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


        public string msg { get; set; }

        public long total { get; set; }
        public Stream responseStream { get; set; }

        /// <summary>
        /// 返回内容类型 小写格式
        /// </summary>
        public string responseContentType { get; set; }
    }
}
