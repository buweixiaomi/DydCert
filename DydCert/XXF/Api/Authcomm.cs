using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace XXF.Api
{
    /// <summary>
    /// 认证通用类库 
    /// 详情问徐品
    /// </summary>
    public class Authcomm
    {
        /// <summary>
        /// 使用GBK编码字符串转MD5
        /// to use GBK to Decode default is uper
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToMD5String(string str)
        {
            return ToMD5String(str, Encoding.GetEncoding("GBK"));
        }

        /// <summary>
        /// 指定编码方式将字符串转MD5
        /// usr enc to Decode str and md5 default is uper
        /// </summary>
        /// <param name="str"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 测试时间是否在有效期内
        /// </summary>
        /// <param name="timespan"></param>
        /// <param name="safesecond"></param>
        /// <returns></returns>
        public static bool TestTimeSpanOk(string timespan, int safesecond)
        {
            long ts = 0;
            if (Int64.TryParse(timespan, out ts))
            {
                return TestTimeSpanOk(ts, safesecond);
            }
            return false;
        }
        /// <summary>
        /// 测试时间是否在有效期内
        /// </summary>
        /// <param name="timespan"></param>
        /// <param name="safesecond"></param>
        /// <returns></returns>
        public static bool TestTimeSpanOk(long timespan, int safesecond)
        {
            return (int)Math.Abs(timespan - GetTimeSpan()) <= safesecond;
        }
        /// <summary>
        /// 根据控制器信息拼接默认url
        /// </summary>
        /// <returns></returns>
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
        /// 得到配置信息，找不到则使用默认值
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
        /// <summary>
        /// 根据key得到配置信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppConfig(string key)
        {
            string v = System.Configuration.ConfigurationManager.AppSettings[key];
            return v;
        }

        /// <summary>
        /// 进行认证签名
        /// </summary>
        /// <param name="para"></param>
        /// <param name="appsecret"></param>
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
                    if (f.Key == "sign")
                        continue;
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
        /// <summary>
        /// 进行认证签名
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="appsecret"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 获取基础的sign字段信息
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="appsecret"></param>
        /// <param name="timespan"></param>
        /// <returns></returns>
        public static string BaseSign(string appid, string appsecret, string timespan)
        {
            string tosignstr = "appid=" + appid + "&appsecret=" + appsecret + "&timespan=" + timespan;
            return ToMD5String(tosignstr);
        }
        /// <summary>
        /// 获取时间到1970-1-1的时间间隔（秒）
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long GetTimeSpan(DateTime dt)
        {
            TimeSpan ts = dt - Convert.ToDateTime("1970-1-1 00:00:00");
            return Convert.ToInt64(ts.TotalSeconds);
        }
        /// <summary>
        /// 获取当前时间到1970-1-1的时间间隔（秒）
        /// </summary>
        public static long GetTimeSpan()
        {
            return GetTimeSpan(DateTime.Now);
        }
        /// <summary>
        /// 获取请求的参数集合
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static List<Api.ParmField> GetRequestPara(System.Web.HttpRequestBase req)
        {
            List<Api.ParmField> para = new List<ParmField>();
            foreach (string k in req.Form.AllKeys)
            {
                para.Add(new Api.StringField(k, req.Form[k]));
            }
            return para;
        }
        /// <summary>
        /// 获取请求的参数集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="req"></param>
        /// <returns></returns>
        public static T GetRequestPara<T>(System.Web.HttpRequestBase req) where T : Dictionary<string, string>,new()
        {
            T para = new T();
            foreach (string k in req.Form.AllKeys)
            {
                para.Add(k, req.Form[k]);
            }
            return para;
        }
    


    }
}
