using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CertCenter.Models
{
    public class CertCenterComm
    {
        public static readonly string[] APPTYPENAME = { "后台管理应用", "商户应用", "消费者应用" };
        public static string[] signfields = { "appid", "userid", "pwd", "timespan", "sign" };
        public static JsonResult Visit(Func<XXF.Db.DbConn, object> action, Controller controller)
        {
            //msgs.Add(-100, "认证失败");
            //msgs.Add(-905, "token不存在或已过期");
            //msgs.Add(-102, "请求超时");
            //msgs.Add(-103, "	sign不正确");
            //msgs.Add(-104, "无权操作该接口");
            //msgs.Add(-105, "接口已被冻结");
            //msgs.Add(-106, "应用被冻结");

            //msgs.Add(-111, "参数不完整");
            //msgs.Add(-112, "用户不存在");
            //msgs.Add(-113, "pwd不正确");
            //msgs.Add(-114, "用户被冻结");

            CertComm.ServerResult sr = new CertComm.ServerResult();
            Dictionary<string, string> para = GetRequestPara(controller);
            if (para.ContainsKey("appid") && string.IsNullOrEmpty(para["appid"]))
            {
                sr.code = -111;
                sr.msg = "appid不能为空";
            }
            else if (para.ContainsKey("timespan") && string.IsNullOrEmpty(para["timespan"]))
            {
                sr.code = -111;
                sr.msg = "timespan不能为空";
            }
            else if (para.ContainsKey("sign") && string.IsNullOrEmpty(para["sign"]))
            {
                sr.code = -111;
                sr.msg = "sign不能为空";
            }
            else if (para.ContainsKey("userid") && string.IsNullOrEmpty(para["userid"]))
            {
                sr.code = -111;
                sr.msg = "userid不能为空";
            }
            else if (para.ContainsKey("pwd") && string.IsNullOrEmpty(para["pwd"]))
            {
                sr.code = -111;
                sr.msg = "pwd不能为空";
            }
            else
            {
                if (!CertComm.Authcomm.TestTimeSpanOk(para["timespan"], 10 * 60))
                {
                    sr.code = -102;
                    sr.msg = AUTH_CODE_MSG.Get(sr.code);
                }
                else
                {
                    using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
                    {
                        PubConn.Open();
                        Models.DbModels.app appitem = Models.AppDal.Instance.GetAppInfo(PubConn, para["appid"]);
                        if (appitem == null)
                        {
                            sr.code = -103;
                            sr.msg = AUTH_CODE_MSG.Get(sr.code);
                        }
                        else if (appitem.freeze == 1)
                        {
                            sr.code = -107;
                            sr.msg = AUTH_CODE_MSG.Get(sr.code);
                        }
                        else
                        {
                            string nowsign = CertComm.Authcomm.ToSign(para, appitem.appsecret);
                            if (nowsign.ToLower() != para["sign"].ToLower())
                            {
                                sr.code = -104;
                                sr.msg = AUTH_CODE_MSG.Get(sr.code);
                            }
                            else//用户相关验证
                            {
                                Models.DbModels.manage manager = Models.AccountDal.Instance.getManage(PubConn, para["userid"]);
                                if (manager == null)
                                {
                                    sr.code = -112;
                                    sr.msg = AUTH_CODE_MSG.Get(sr.code);
                                }
                                else if (manager.freeze == 1)
                                {
                                    sr.code = -114;
                                    sr.msg = AUTH_CODE_MSG.Get(sr.code);
                                }
                                else if (CertComm.Authcomm.ToMD5String(manager.pwd) != para["pwd"])
                                {
                                    sr.code = -113;
                                    sr.msg = AUTH_CODE_MSG.Get(sr.code);
                                }
                            }
                        }
                    }
                }
            }
            if (sr.code < -100)
            {
                return new JsonResult() { Data = sr };
            }
            return null;
        }



        public static Dictionary<string, string> GetRequestPara(Controller c)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            foreach (var a in c.Request.Form.AllKeys)
            {
                para.Add(a, c.Request.Form[a]);
            }
            return para;
        }
        public static Dictionary<string, string> GetRequestPara(Controller c,string[] fileds)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            foreach (var a in fileds)
            {
                para.Add(a, c.Request.Form[a]);
            }
            return para;
        }


        public static int ValiFields(Controller controller,out string msg)
        {
            int code = 1;
            msg = string.Empty;
            Dictionary<string, string> para = GetRequestPara(controller);
            if (para.ContainsKey("appid") && string.IsNullOrEmpty(para["appid"]))
            {
                code = -111;
                msg = "appid不能为空";
            }
            else if (para.ContainsKey("timespan") && string.IsNullOrEmpty(para["timespan"]))
            {
                code = -111;
                msg = "timespan不能为空";
            }
            else if (para.ContainsKey("sign") && string.IsNullOrEmpty(para["sign"]))
            {
                code = -111;
                msg = "sign不能为空";
            }
            else if (para.ContainsKey("userid") && string.IsNullOrEmpty(para["userid"]))
            {
                code = -111;
               msg = "userid不能为空";
            }
            else if (para.ContainsKey("pwd") && string.IsNullOrEmpty(para["pwd"]))
            {
                code = -111;
                msg = "pwd不能为空";
            }
            return code;
        }

        public static int ValiFields(Controller controller, string[] fields, out string msg)
        {
            //string appid, string userid, string timespan, string sign, string pwd,
            msg = "";
            foreach (string a in fields)
            {
                if (string.IsNullOrEmpty(controller.Request[a]))
                {
                    msg = a+"不能为空";
                    return -111;
                }
            }
            return 1;
        }

        public static int ValiFields(Dictionary<string, string> para,out string msg)
        {
            msg = "";
            foreach (KeyValuePair<string, string> a in para)
            {
                if (string.IsNullOrEmpty(a.Value))
                {
                    msg = a.Key + "不能为空";
                    return -111;
                }
            }
            return 1;
        }

        public static int ValiFields(Dictionary<string, string> para,string[] fields, out string msg)
        {
            msg = "";
            foreach (string s in fields)
            {
                if(!para.ContainsKey(s)||string.IsNullOrEmpty(para[s]))
                {
                    msg = s + "不能为空";
                    return -111;
                }
            }
            return 1;
        }
    }


    //public class AuthException : Exception
    //{
    //    public UnAuthException(
    //}

    public class AUTH_CODE_MSG
    {
        public static Dictionary<int, string> msgs = new Dictionary<int, string>();

        static AUTH_CODE_MSG()
        {
            msgs.Add(-905, "token不存在或已过期");
            msgs.Add(-102, "请求超时");
            msgs.Add(-103, "appid不存在");
            msgs.Add(-104, "	sign不正确");
            msgs.Add(-105, "无权操作该接口");
            msgs.Add(-106, "接口已被冻结");
            msgs.Add(-107, "应用被冻结");
            msgs.Add(-108, "接口不存在");

            msgs.Add(-111, "参数不完整");
            msgs.Add(-112, "用户不存在");
            msgs.Add(-113, "密码不正确");
            msgs.Add(-114, "用户被冻结");
        }

        public static string Get(int index)
        {
            if (msgs.ContainsKey(index))
                return msgs[index];
            else
            {
                if (index > 0)
                {
                    return "success";
                }
                else
                {
                    return "faild";
                }
            }
        }
    }


}