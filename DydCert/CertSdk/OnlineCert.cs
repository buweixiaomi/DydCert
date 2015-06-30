using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XXF.Api;

namespace CertSdk
{
    public class OnlineCert 
    {
        public XXF.Api.ClientResult result { get; set; }

        /// <summary>
        /// 登录，从config中得到appid appsecret ，直接登录(指当前应用为非api，不调用api登录接口，直接请求权限中心登录)
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public Token Login(string userid, string pwd)
        {
            string appid = Authcomm.GetAppConfig("appid");
            string appsecret = Authcomm.GetAppConfig("appsecret");
            return Login(userid, pwd, appid ?? "", appsecret ?? "");
        }


        /// <summary> 刷新token过期时间 返回新token信息 </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public  Token RefreshToken(string token)
        {
            List<ParmField> para = new List<ParmField>();
            para.Add(new StringField("token", token ?? ""));
            return ApiHelper<Token>(() =>
            {
                return HttpServer.InvokeApi(Config.REFRESH_TOKEN_URL, para);
            });
        }


        /// <summary>得到token对应信息</summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Token ReqGetTokenInfo(string token)
        {
            ApiInvokeMap.MapCore.GetInstance().Increase("ReqGetTokenInfo");
            List<ParmField> para = new List<ParmField>();
            para.Add(new StringField("token", token ?? ""));
            return ApiHelper<Token>(() =>
            {
                return HttpServer.InvokeApi(Config.TOKENINFO_URL, para);
            });
        }

        public bool RequestLogout(string token)
        {
            List<ParmField> para = new List<ParmField>();
            para.Add(new StringField("token", token));
            return ApiHelper<bool>(() =>
            {
                return HttpServer.InvokeApi(Config.LOGOUT_URL, para);
            });
        }

        /// <summary>
        /// 登录 直接登录(指当前应用为非api，不调用api登录接口，直接请求权限中心登录)
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="pwd"></param>
        /// <param name="appid"></param>
        /// <param name="appsecret"></param>
        /// <returns></returns>
        public  Token Login(string userid, string pwd, string appid, string appsecret)
        {
            List<ParmField> para = new List<ParmField>();
            para.Add(new StringField("userid", userid ?? ""));
            para.Add(new StringField("pwd", Authcomm.ToMD5String(pwd ?? "")));
            para.Add(new StringField("appid", appid ?? ""));
            para.Add(new StringField("timespan", Authcomm.GetTimeSpan()));
            Authcomm.ToSign(para, appsecret ?? "");
            Token Ttoken = ApiHelper<Token>(() =>
            {
                return HttpServer.InvokeApi(Config.TOKEN_URL, para);
            });
            return Ttoken;
        }


        private T ApiHelper<T>(Func<ClientResult> action)
        {
            try
            {
                result = action.Invoke();
                if (result.success)
                {
                    if (typeof(T) == typeof(bool))
                    {
                        object c = true;
                        return (T)c;
                    }
                    if (typeof(T) == typeof(int))
                    {
                        object c = result.code;
                        return (T)c;
                    }
                    if (typeof(T) == typeof(string))
                    {
                        object c = result.repObject["response"].ToString();
                        return (T)c;
                    }
                    T actresult = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(result.repObject["response"].ToString());
                    return actresult;
                }
                else
                {
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                result = new ClientResult();
                result.code = -1;
                result.msg = "请求接口或解析返回数据时出错：" + ex.Message;
                return default(T);
            }
        }

    }
}
