using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XXF.Db;

namespace CertCenter.Areas.CertApi.Models
{
    public class ApiCommDal
    {

        #region 接口相关
        public static ActionResult GetToken(Controller c, CertCenter.Models.DbModels.TokenType tokentype)
        {

            JsonResult jsonr = new JsonResult();
            jsonr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            CertComm.ServerResult sr = new CertComm.ServerResult();
            string msg = "";
            jsonr.Data = sr;
            Dictionary<string, string> para = CertCenter.Models.CertCenterComm.GetRequestPara(c, new string[] { "appid", "userid", "timespan", "sign", "pwd" });
            sr.code = CertCenter.Models.CertCenterComm.ValiFields(para, out msg);
            if (sr.code < 0)
            {
                sr.msg = msg;
                return jsonr;
            }
            string userid = para["userid"];
            string appid = para["appid"];
            string pwd = para["pwd"];
            string sign = para["sign"];
            string timespan = para["timespan"];
            if (!CertComm.Authcomm.TestTimeSpanOk(timespan, 10 * 60))
            {
                sr.code = -102;
                sr.msg = CertCenter.Models.AUTH_CODE_MSG.Get(sr.code);
                return jsonr;
            }
            using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
            {
                PubConn.Open();//打开基本

                CertCenter.Models.DbModels.app appitem = CertCenter.Models.AppDal.Instance.GetAppInfo(PubConn, appid);
                if (appitem == null || TokenTypeGetAppType(tokentype) != appitem.apptype)
                {
                    sr.code = -103;
                    sr.msg = CertCenter.Models.AUTH_CODE_MSG.Get(sr.code);
                    return jsonr;
                }
                else if (appitem.freeze == 1)
                {
                    sr.code = -107;
                    sr.msg = CertCenter.Models.AUTH_CODE_MSG.Get(sr.code);
                    return jsonr;
                }

                string nowsign = CertComm.Authcomm.ToSign(para, appitem.appsecret);
                if (nowsign.ToLower() != para["sign"].ToLower())
                {
                    sr.code = -104;
                    sr.msg = CertCenter.Models.AUTH_CODE_MSG.Get(sr.code);
                    return jsonr;
                }

                string bsconstr = GetConnStr(tokentype, userid);
                if (string.IsNullOrEmpty(bsconstr))
                {
                    sr.code = -112;
                    sr.msg = CertCenter.Models.AUTH_CODE_MSG.Get(sr.code);
                    return jsonr;
                }
                string username = "";
                string identityid = "";
                using (XXF.Db.DbConn dydpubConn = XXF.Db.DbConfig.CreateConn(XXF.Db.DbType.SQLSERVER, bsconstr))
                {
                    dydpubConn.Open();
                    ////用户相关验证
                    switch (tokentype)
                    {
                        case CertCenter.Models.DbModels.TokenType.managetoken:
                            sr.code = ManageAccountVali(dydpubConn, userid, pwd, out username, out identityid);
                            break;
                        case CertCenter.Models.DbModels.TokenType.shoptoken:
                            string tt = "";
                            sr.code = ShopAccountVali(dydpubConn, userid, pwd, out username, out tt, out identityid);
                            userid = tt;
                            break;
                        case CertCenter.Models.DbModels.TokenType.usertoken:
                            sr.code = UserAccountVali(dydpubConn, userid, pwd, out username, out identityid);
                            break;
                        default:
                            break;
                    }
                }
                if (sr.code < 0)
                {
                    sr.msg = CertCenter.Models.AUTH_CODE_MSG.Get(sr.code);
                    return jsonr;
                }

                CertCenter.Models.DbModels.tb_token Ttoken = CertCenter.Models.TokenDal.Instance.GetToken(PubConn, userid, appid, tokentype);
                if (Ttoken == null || Ttoken.expires.CompareTo(DateTime.Now) < 0)
                {
                    if (Ttoken != null)
                    {
                        DeleteToken(PubConn, Ttoken.token, tokentype);
                    }
                    Ttoken = new CertCenter.Models.DbModels.tb_token();
                    Ttoken.appid = appid;
                    Ttoken.token = Guid.NewGuid().ToString().Replace("-", "");
                    Ttoken.userid = userid;
                    Ttoken.id = identityid;
                    Ttoken.username = username;
                    CertCenter.Models.TokenDal.Instance.Add(PubConn, Ttoken, tokentype);
                }
                else
                {
                    Ttoken = RefreshToken(PubConn, Ttoken.token, username, tokentype);
                }
                sr.code = 1;
                sr.response = new { appid = Ttoken.appid, userid = Ttoken.userid, username = Ttoken.username, token = Ttoken.token, createtime = Ttoken.createtime.ToString("yyyy-MM-dd HH:mm:ss"), expires = Ttoken.expires.ToString("yyyy-MM-dd HH:mm:ss"), id = Ttoken.id };
                return jsonr;

            }


        }

        public static ActionResult TestAuth(Controller c, CertCenter.Models.DbModels.TokenType tokentype)
        {
            JsonResult jsonresult = new JsonResult();
            jsonresult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            CertComm.ServerResult r = new CertComm.ServerResult();
            jsonresult.Data = r;

            string token = c.Request.Params["token"];
            string _controller = c.Request.Params["controller"];
            string _area = c.Request.Params["area"];
            string _action = c.Request.Params["action"];

            string _apiname = c.Request.Params["apiname"];
            int _apiid = 0;
            int.TryParse(c.Request.Params["apiid"] ?? "", out _apiid);
            if (string.IsNullOrEmpty(token))
            {
                r.code = -905;
                r.msg = "token不能为空。";
                return jsonresult;
            }

            using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
            {
                PubConn.Open();//打开基本

                #region token
                CertCenter.Models.DbModels.tb_token Ttoken = CertCenter.Models.TokenDal.Instance.GetToken(PubConn, token, tokentype);
                if (Ttoken == null || Ttoken.expires.CompareTo(DateTime.Now) < 0)
                {
                    DeleteToken(PubConn, token, tokentype);
                    r.code = -101;
                    r.msg = CertCenter.Models.AUTH_CODE_MSG.Get(r.code);
                    return jsonresult;
                }
                #endregion

                //#region user
                //#endregion

                #region //应用
                CertCenter.Models.DbModels.app appitem = CertCenter.Models.AppDal.Instance.GetAppInfo(PubConn, Ttoken.appid);
                if (appitem == null)
                {
                    r.code = -103;
                    r.msg = CertCenter.Models.AUTH_CODE_MSG.Get(r.code);
                    return jsonresult;
                }
                else if (appitem.freeze == 1)
                {
                    r.code = -107;
                    r.msg = CertCenter.Models.AUTH_CODE_MSG.Get(r.code);
                    return jsonresult;
                }
                #endregion

                #region 接口
                int apptype = TokenTypeGetAppType(tokentype);

                CertCenter.Models.DbModels.api apiinfo = null;
                if (_apiid > 0)
                {
                    apiinfo = CertCenter.Models.ApiDal.Instance.Get(PubConn, _apiid);
                }
                else if (!string.IsNullOrEmpty(_apiname))
                {
                    apiinfo = CertCenter.Models.ApiDal.Instance.Get(PubConn, _apiname, apptype);
                }
                else
                {
                    apiinfo = CertCenter.Models.ApiDal.Instance.Get(PubConn, _area, _controller, _action, apptype);
                }

                if (apiinfo == null)
                {
                    r.code = -108;
                    r.msg = CertCenter.Models.AUTH_CODE_MSG.Get(r.code);
                    return jsonresult;
                }
                if (apiinfo.freeze == 1)
                {
                    r.code = -106;
                    r.msg = CertCenter.Models.AUTH_CODE_MSG.Get(r.code);
                    return jsonresult;
                }
                if (appitem.appgradeno < appitem.appgradeno || apptype != apiinfo.apptype)
                {
                    r.code = -105;
                    r.msg = CertCenter.Models.AUTH_CODE_MSG.Get(r.code);
                    return jsonresult;
                }
                #endregion

                r.code = 1;
                r.response = new { appid = Ttoken.appid, userid = Ttoken.userid, username = Ttoken.username, token = Ttoken.token, createtime = Ttoken.createtime.ToString("yyyy-MM-dd HH:mm:ss"), expires = Ttoken.expires.ToString("yyyy-MM-dd HH:mm:ss") };

                return jsonresult;
            }
        }

        public static ActionResult GetApiList(Controller c, CertCenter.Models.DbModels.TokenType tokentype)
        {
            JsonResult jsonresult = new JsonResult();
            jsonresult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            CertComm.ServerResult r = new CertComm.ServerResult();
            jsonresult.Data = r;

            string token = c.Request.Params["token"];

            if (string.IsNullOrEmpty(token))
            {
                r.code = -905;
                r.msg = "token不能为空。";
                return jsonresult;
            }

            using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
            {
                PubConn.Open();//打开基本

                #region token
                CertCenter.Models.DbModels.tb_token Token = CertCenter.Models.TokenDal.Instance.GetToken(PubConn, token, tokentype);
                if (Token == null || Token.expires.CompareTo(DateTime.Now) < 0)
                {
                    DeleteToken(PubConn, token, tokentype);
                    r.code = -101;
                    r.msg = CertCenter.Models.AUTH_CODE_MSG.Get(r.code);
                    return jsonresult;
                }
                #endregion
                List<CertCenter.Models.DbModels.api> apis = new List<CertCenter.Models.DbModels.api>();
                CertCenter.Models.DbModels.app appinfo = CertCenter.Models.AppDal.Instance.GetAppInfo(PubConn, Token.appid);
                if (appinfo == null)
                {
                    DeleteToken(PubConn, token, tokentype);
                    r.code = -103;
                    r.msg = CertCenter.Models.AUTH_CODE_MSG.Get(r.code);
                    return jsonresult;
                }
                int apptype = 0;
                switch (tokentype)
                {
                    case CertCenter.Models.DbModels.TokenType.managetoken:
                        apptype = 0;
                        break;
                    case CertCenter.Models.DbModels.TokenType.shoptoken:
                        apptype = 2;
                        break;
                    case CertCenter.Models.DbModels.TokenType.usertoken:
                        apptype = 1;
                        break;
                }

                for (int i = appinfo.appgradeno; i > 0; i--)
                {
                    apis.AddRange(CertCenter.Models.ApiDal.Instance.GetGradeApis(PubConn, apptype, i));
                }
                r.code = 1;
                r.response = apis;
                return jsonresult;
            }
        }

        public static ActionResult GetTokenInfo(Controller c, CertCenter.Models.DbModels.TokenType tokentype)
        {
            CertComm.ServerResult sr = new CertComm.ServerResult();
            JsonResult jsonresult = new JsonResult();
            jsonresult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonresult.Data = sr;

            string token = c.Request.Params["token"];

            if (string.IsNullOrEmpty(token))
            {
                sr.code = -905;
                sr.msg = "token不能为空。";
                return jsonresult;
            }

            try
            {
                using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
                {
                    PubConn.Open();//打开基本
                    CertCenter.Models.DbModels.tb_token Ttoken = CertCenter.Models.TokenDal.Instance.GetToken(PubConn, token, tokentype);
                    if (Ttoken == null)
                    {
                        sr.code = -905;
#warning 修改提示
                        sr.msg = "token不存在";
                        return jsonresult;
                    }
                    if (Ttoken.expires.CompareTo(DateTime.Now) <= 0)
                    {
                        sr.code = -905;
                        sr.msg = "token过期。";
                        return jsonresult;
                    }
                    #region 修改名字,除去修改了用户名后不同步的bug
                    using (XXF.Db.DbConn priconn = XXF.Db.DbConn.CreateConn(GetConnStr(tokentype, Ttoken.userid)))
                    {
                        Ttoken.username = CertCenter.Models.AccountDal.Instance.getUserName(priconn, Ttoken.userid, tokentype);
                    }
                    CertCenter.Models.TokenDal.Instance.Edit(PubConn, Ttoken, tokentype);
                    #endregion

                    sr.code = 1;
                    sr.response = new { appid = Ttoken.appid, userid = Ttoken.userid, username = Ttoken.username, token = Ttoken.token, createtime = Ttoken.createtime.ToString("yyyy-MM-dd HH:mm:ss"), expires = Ttoken.expires.ToString("yyyy-MM-dd HH:mm:ss"), id = Ttoken.id };

                    return jsonresult;
                }
            }
            catch (Exception ex)
            {
                sr.code = -1;
                sr.msg = ex.Message;
                return jsonresult;
            }
        }

        public static ActionResult RefreshToken(Controller c, CertCenter.Models.DbModels.TokenType tokentype)
        {
            CertComm.ServerResult r = new CertComm.ServerResult();
            JsonResult jsonresult = new JsonResult();
            jsonresult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonresult.Data = r;

            string token = c.Request.Params["token"] ?? "";
            if (string.IsNullOrEmpty(token))
            {
                r.code = -905;
                r.msg = "token不能为空。";
                return jsonresult;
            }
            try
            {
                using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
                {
                    PubConn.Open();//打开基本


                    CertCenter.Models.DbModels.tb_token Ttoken = RefreshToken(PubConn, token, "", tokentype);
                    if (Ttoken == null)
                    {
                        r.code = -101;
                        r.msg = CertCenter.Models.AUTH_CODE_MSG.Get(r.code);
                        return jsonresult;
                    }

                    #region 修改名字,除去修改了用户名后不同步的bug
                    using (XXF.Db.DbConn priconn = XXF.Db.DbConn.CreateConn(GetConnStr(tokentype, Ttoken.userid)))
                    {
                        Ttoken.username = CertCenter.Models.AccountDal.Instance.getUserName(priconn, Ttoken.userid, tokentype);
                    }
                    CertCenter.Models.TokenDal.Instance.Edit(PubConn, Ttoken, tokentype);
                    #endregion

                    r.code = 1;
                    r.response = Ttoken;
                    return jsonresult;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 退出登录，即删除token
        /// </summary>
        /// <param name="c"></param>
        /// <param name="tokentype"></param>
        /// <returns></returns>
        public static ActionResult LogOut(Controller c, CertCenter.Models.DbModels.TokenType tokentype)
        {

            CertComm.ServerResult r = new CertComm.ServerResult();
            JsonResult jsonresult = new JsonResult();
            jsonresult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonresult.Data = r;

            string token = c.Request.Params["token"] ?? "";
            if (string.IsNullOrEmpty(token))
            {
                r.code = -905;
                r.msg = "token不能为空。";
                return jsonresult;
            }

            using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
            {
                PubConn.Open();
                DeleteToken(PubConn, token, tokentype);

                r.code = 1;
                r.msg = "退出成功。";
                return jsonresult;
            }
        }

        #endregion


        public static string GetConnStr(CertCenter.Models.DbModels.TokenType tokentype, string userid = "")
        {
            string constr = "";
            switch (tokentype)
            {
                case CertCenter.Models.DbModels.TokenType.usertoken:
                    constr = XXF.Db.DbConfig.GetConfig("dydDbConn");
                    break;
                case CertCenter.Models.DbModels.TokenType.shoptoken:
                    using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn(XXF.Db.DbType.SQLSERVER, XXF.Db.DbConfig.GetConfig("dydDbConn")))
                    {
                        PubConn.Open();
                        string sql = "select f_shzh,f_shdqbm,f_shsj from tb_shop_area where f_shzh=@userid or f_shsj=@userid";
                        XXF.Db.SimpleProcedureParameter para = new SimpleProcedureParameter();
                        para.Add("@userid", userid);
                        DataSet ds = new DataSet();
                        PubConn.SqlToDataSet(ds, sql, para.ToParameters());

                        if (ds.Tables[0].Rows.Count == 0)
                        {
                            return "";
                        }
                        string dqbm = ds.Tables[0].Rows[0]["f_shdqbm"].ToString();
                        string t = XXF.Db.DbConfig.GetConfig("ShopAreaConnectString");
                        throw new Exception("no show");
                        // constr = XXF.Db.DbAreaRule.ShopAreaPartitionRule(t, Convert.ToInt32(dqbm));
                    }
                    break;
                case CertCenter.Models.DbModels.TokenType.managetoken:
                    constr = XXF.Db.DbConfig.GetConfig("CrmDbConn");
                    break;
                default:
                    break;
            }
            if (constr == "")
            {
                throw new Exception("车有问题。");
            }
            return constr;
        }


        public static int TokenTypeGetAppType(CertCenter.Models.DbModels.TokenType tokentype)
        {
            switch (tokentype)
            {
                case CertCenter.Models.DbModels.TokenType.usertoken:
                    return 2;
                case CertCenter.Models.DbModels.TokenType.shoptoken:
                    return 1;
                case CertCenter.Models.DbModels.TokenType.managetoken:
                    return 0;
                default:
                    return 0;
            }
        }

        private static int ShopAccountVali(XXF.Db.DbConn PubConn, string userid, string md5pwd, out string username, out string ouserid, out string id)
        {
            username = "";
            ouserid = "";
            id = "";
            CertCenter.Models.DbModels.manage manager = CertCenter.Models.AccountDal.Instance.getShop(PubConn, userid);
            if (manager == null)
            {
                return -112;

            }
            else if (manager.freeze == 1)
            {
                return -114;
            }
            else if (CertComm.Authcomm.ToMD5String(manager.pwd) != md5pwd)
            {
                return -113;
            }
            username = manager.username;
            ouserid = manager.userid;
            id = manager.id;
            return 1;
        }

        private static int UserAccountVali(XXF.Db.DbConn PubConn, string userid, string md5pwd, out string username, out string id)
        {
            username = "";
            id = "";
            CertCenter.Models.DbModels.manage manager = CertCenter.Models.AccountDal.Instance.getUser(PubConn, userid);
            if (manager == null)
            {
                return -112;

            }
            else if (manager.freeze == 1)
            {
                return -114;
            }
            else if (CertComm.Authcomm.ToMD5String(manager.pwd) != md5pwd)
            {
                return -113;
            }
            username = manager.username;
            id = manager.id;
            return 1;
        }

        private static int ManageAccountVali(XXF.Db.DbConn PubConn, string userid, string md5pwd, out string username, out string id)
        {
            username = "";
            id = "";
            CertCenter.Models.DbModels.manage manager = CertCenter.Models.AccountDal.Instance.getManage(PubConn, userid);
            if (manager == null)
            {
                return -112;

            }
            else if (manager.freeze == 1)
            {
                return -114;
            }
            else if (CertComm.Authcomm.ToMD5String(manager.pwd).ToUpper() != md5pwd.ToUpper())
            {
                return -113;
            }
            username = manager.username;
            id = manager.id;
            return 1;
        }



        private static void DeleteToken(XXF.Db.DbConn PubConn, string token, CertCenter.Models.DbModels.TokenType tokentype)
        {
            CertCenter.Models.TokenDal.Instance.Delete(PubConn, token, tokentype);
        }

        private static CertCenter.Models.DbModels.tb_token RefreshToken(XXF.Db.DbConn PubConn, string token, string username, CertCenter.Models.DbModels.TokenType tokentype)
        {
            CertCenter.Models.DbModels.tb_token Token = CertCenter.Models.TokenDal.Instance.GetToken(PubConn, token, tokentype);
            if (Token == null)
                return null;
            if (Token.expires.CompareTo(DateTime.Now) < 0)
            {
                DeleteToken(PubConn, token, tokentype);
                return null;
            }
            Token.expires = DateTime.Now.AddMinutes(CertCenter.Models.TokenDal.Instance.GetExpiresminutes(tokentype));
            if (!string.IsNullOrEmpty(username))
            {
                Token.username = username;
            }
            CertCenter.Models.TokenDal.Instance.Edit(PubConn, Token, tokentype);
            return Token;
        }



    }
}