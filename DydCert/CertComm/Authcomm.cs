using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CertComm
{
    public class Authcomm
    {
        //to use GBK to Decode default is uper
        public static string ToMD5String(string str)
        {
            return ToMD5String(str, Encoding.GetEncoding("GBK"));
        }

        //usr enc to Decode str and md5 default is uper
        public static string ToMD5String(string str, Encoding enc)
        {
            byte[] tobytes = enc.GetBytes(str);
            MD5 md5 = MD5.Create();
            byte[] resultbytes = md5.ComputeHash(tobytes);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in resultbytes)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString().ToUpper();
        }

        public static bool TestTimeSpanOk(string timespan, int safesecond)
        {
            long ts = 0;
            if (Int64.TryParse(timespan, out ts))
            {
                return TestTimeSpanOk(ts, safesecond);
            }
            return false;
        }
        public static bool TestTimeSpanOk(long timespan, int safesecond)
        {
            return (int)Math.Abs(timespan - GetTimeSpan()) <= safesecond;
        }

        public static string ConnUrl(string area, string controller, string action)
        {
            string r = string.Empty;
            if (!string.IsNullOrEmpty(area))
            {
                r += "/" + area;
            }

            if (!string.IsNullOrEmpty(controller))
            {
                r += "/" + controller;
            }

            if (!string.IsNullOrEmpty(action))
            {
                r += "/" + action;
            }
            return string.IsNullOrEmpty(r) ? "/" : r;
        }

        /// <summary>
        /// 得到配置信息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultvalue"></param>
        /// <returns></returns>
        public static string GetAppConfig(string key, string defaultvalue)
        {
            string v = GetAppConfig(key);
            if (string.IsNullOrEmpty(v))
                return defaultvalue;
            return v;
        }
        public static string GetAppConfig(string key)
        {
            string v = System.Configuration.ConfigurationManager.AppSettings[key];
            return v;
        }


        public static void ToSign(List<ParmField> para, string appsecret)
        {
            if (para == null)
            {
                para = new List<ParmField>();
                //   para.Add(new StringField("timespan",GetTimeSpan().ToString()));
            }
            IOrderedEnumerable<ParmField> _newpara = para.OrderBy((a) => { return a.Key; });
            StringBuilder sb = new StringBuilder();
            foreach (ParmField f in _newpara)
            {
                if (f.GetType() == typeof(StringField))
                {
                    sb.Append(f.Key + "=" + f.Value + "&");
                }
            }
            if (sb.Length > 0)
            {
                sb.Append("&appsecret=" + appsecret);
            }
            else
            {
                sb.Append("appsecret=" + appsecret);
            }
            string si = Authcomm.ToMD5String(sb.ToString());
            para.Add(new StringField("sign", si));
        }

        public static string ToSign(Dictionary<string, string> dic, string appsecret)
        {
            if (dic == null)
            {
                return "";
            }

            IOrderedEnumerable<KeyValuePair<string,string>> _newdic = dic.OrderBy(x => x.Key);

            StringBuilder sb = new StringBuilder();
            foreach (var f in _newdic)
            {
                if (f.Key == "sign")
                    continue;
                sb.Append(f.Key + "=" + f.Value + "&");
            }
            if (sb.Length > 0)
            {
                sb.Append("&appsecret=" + appsecret);
            }
            else
            {
                sb.Append("appsecret=" + appsecret);
            }
            string si = Authcomm.ToMD5String(sb.ToString());
            return si;
        }

        public static string BaseSign(string appid, string appsecret, string timespan)
        {
            string tosignstr = "appid=" + appid + "&appsecret=" + appsecret + "&timespan=" + timespan;
            return ToMD5String(tosignstr);
        }

        public static long GetTimeSpan(DateTime dt)
        {
            TimeSpan ts = dt - Convert.ToDateTime("1970-1-1 00:00:00");
            return Convert.ToInt64(ts.TotalSeconds);
        }
        public static long GetTimeSpan()
        {
            return GetTimeSpan(DateTime.Now);
        }

    }
}
