using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using XXF.Api;
//using XXF;
using XXF.Db;
//using XXF.Api;

namespace CertSdk.old.CertCenter
{

    /// <summary>
    /// 使用api认证，请在配置文件里配置 UseLocalCert=fase （默认是false），并保证appid appsecret certcenterurl也进行了对应的配置
    /// 使用本地认证，请在配置文件里配置 UseLocalCert=true ，并保证 appid，appsecret ，crmdyconnectstring,Mainconnectstring,qxconnectstring 也进行了对应的配置 
    /// <add key="appid" value="Customer"/>
    /// <add key="appsecret" value="1234"/>
    ///<add key="CertCenterUrl" value="http://192.168.17.234:3388"/>
    /// <add key="UseLocalCert" value="false"/>
    /// <add key ="CrmdyConnectString" value="server=192.168.17.232;Initial Catalog=dyd_new_crmdy;User ID=sa;Password=Xx~!@#;" />
    ///<add key ="MainConnectString" value="server=192.168.17.232;Initial Catalog=dyd_new_main;User ID=sa;Password=Xx~!@#;" />
    ///<add key="QxConnectString" value="server=192.168.17.232;database=dyd_new_qx;uid=sa;pwd=Xx~!@#;"/>
    /// </summary>
    public class CertCenterProvider : ICertProvider
    {
        ICertProvider currprovider = null;
        public CertCenterProvider(ServiceCertType certtype)
            : this(certtype, true, false) { }

        public CertCenterProvider(ServiceCertType certtype, bool usetokencache)
            : this(certtype, usetokencache, false) { }
        /// <summary>
        ///  
        /// </summary>
        /// <param name="certtype">验证类型</param>
        /// <param name="usetokencache">是否使用token缓存</param>
        /// <param name="useapilistcache">为所有有权接口缓存，进行本地验证。当此值为true时，usertokencache自动为true.不建议使用</param>
        public CertCenterProvider(ServiceCertType certtype, bool usetokencache, bool useapilistcache)
        {
            if (XXF.Db.DbConfig.GetConfig("UseLocalCert") == "true")
            {
                currprovider = new LocalCert(certtype, usetokencache, useapilistcache);
            }
            else
            {
                currprovider = new OnlineCert(certtype, usetokencache, useapilistcache);
            }
        }
        public XXF.Api.ClientResult result
        {
            get { return currprovider.result; }
            set { currprovider.result = value; }
        }

        public AuthToken Login(string userid, string pwd)
        {
            return currprovider.Login(userid, pwd);
        }

        public AuthToken Login(string userid, string pwd, string appid, string appsecret)
        {
            return currprovider.Login(userid, pwd, appid, appsecret);
        }

        public AuthToken Login(Controller controller)
        {
            return currprovider.Login(controller);
        }

        public bool Auth(string token, int apiid)
        {
            return currprovider.Auth(token, apiid);
        }

        public bool Auth(string token, string apiname)
        {
            return currprovider.Auth(token, apiname);
        }

        public bool Auth(string token, string area, string controller, string action)
        {
            return currprovider.Auth(token, area, controller, action);
        }

        public bool Auth(string token, Controller c)
        {
            return currprovider.Auth(token, c);
        }

        /// <summary>
        /// 验证Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool Auth(string token)
        {
            return currprovider.Auth(token);
        }

        public AuthToken RefreshToken(string token)
        {
            return currprovider.RefreshToken(token);
        }

        public List<api> GetApiList(string token)
        {
            return currprovider.GetApiList(token);
        }

        public AuthToken GetTokenInfo(string token)
        {
            return currprovider.GetTokenInfo(token);
        }

        public string GetUserName(string token)
        {
            return currprovider.GetUserName(token);
        }

        public bool TestSignAndTime(Controller controller)
        {
            return currprovider.TestSignAndTime(controller);
        }

        public bool TestSign(Controller controller)
        {
            return currprovider.TestSign(controller);
        }

        public string GetAppSecret(string appid)
        {
            return currprovider.GetAppSecret(appid);
        }

        public bool TestTokenExist(Controller controller)
        {
            return currprovider.TestTokenExist(controller);
        }

        public bool LogOut(Controller controller)
        {
            return currprovider.LogOut(controller);
        }

        public bool LogOut(string token)
        {
            return currprovider.LogOut(token);
        }

        public string GetTokenFromReq(Controller controller)
        {
            return currprovider.GetTokenFromReq(controller);
        }

        public bool SuperAuth(Controller controller)
        {
            return currprovider.SuperAuth(controller);
        }

        /// <summary>
        /// 删除Token
        /// </summary>
        /// <param name="token"></param>
        public void DeleteCacheInfo(string token)
        {
            currprovider.DeleteCacheInfo(token);
        }
    }

    public class LocalCert : OnlineCert
    {
        public LocalCert(ServiceCertType certtype)
            : this(certtype, true, false) { }

        public LocalCert(ServiceCertType certtype, bool usetokencache)
            : this(certtype, usetokencache, false) { }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="certtype">验证类型</param>
        /// <param name="usetokencache">是否使用token缓存</param>
        /// <param name="useapilistcache">为所有有权接口缓存，进行本地验证。当此值为true时，usertokencache自动为true.不建议使用</param>
        public LocalCert(ServiceCertType certtype, bool usetokencache, bool useapilistcache)
            : base(certtype, usetokencache, useapilistcache)
        { }

        protected override bool RequestLogout(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                result = new XXF.Api.ClientResult()
                {
                    code = (int)CertCodeEnum.NotExist,
                    msg = "token不能为空。"
                };
                return false;
            }

            using (XXF.Db.DbConn PubConn = CreateQxConn())
            {
                PubConn.Open();
                DeleteToken(PubConn, token, this.certtype);
                result = new XXF.Api.ClientResult()
                {
                    code = (int)CertCodeEnum.NormalError,
                    msg = "退出成功"
                };
                return true;
            }
        }

        protected override string ReqGetAppSecret(string appid)
        {
            if (string.IsNullOrEmpty(appid))
            {
                result = new XXF.Api.ClientResult()
                {
                    code = (int)CertCodeEnum.MisParameter,
                    msg = "请求参数appid不能为空"
                };
                return null;
            }
            try
            {
                using (XXF.Db.DbConn PubConn = CreateQxConn())
                {
                    PubConn.Open();//打开基本
                    string se = GetAppSecretInDB(PubConn, appid);
                    if (string.IsNullOrEmpty(se))
                    {
                        result = new XXF.Api.ClientResult()
                        {
                            code = (int)CertCodeEnum.NormalError,
                            msg = "appid不存在或被冻结"
                        };
                        return null;
                    }
                    result = new XXF.Api.ClientResult()
                     {
                         code = (int)CertCodeEnum.Success,
                         msg = "OK"
                     };
                    return se;
                }
            }
            catch (Exception ex)
            {
                result = new XXF.Api.ClientResult()
                {
                    code = (int)CertCodeEnum.NormalError,
                    msg = "服务器正忙"
                };
                return null;
            }
        }

        protected override AuthToken ReqGetTokenInfo(string token)
        {
            result = new XXF.Api.ClientResult();
            if (string.IsNullOrEmpty(token))
            {
                result.code = (int)CertCodeEnum.NotExist;
                result.msg = "token不能为空。";
                return null;
            }
            try
            {
                using (XXF.Db.DbConn PubConn = CreateQxConn())
                {
                    PubConn.Open();//打开基本
                    AuthToken Ttoken = GetToken(PubConn, token, this.certtype);
                    if (Ttoken == null)
                    {
                        result.code = (int)CertCodeEnum.NotExist;
                        result.msg = "token不存在。";
                        return null;
                    }
                    if (Ttoken.expires.CompareTo(DateTime.Now) <= 0)
                    {
                        result.code = (int)CertCodeEnum.NotExist;
                        result.msg = "token过期。";
                        return null;
                    }
                    #region 修改名字,除去修改了用户名后不同步的bug
                    using (XXF.Db.DbConn priconn = XXF.Db.DbConn.CreateConn(GetConnStr(certtype, Ttoken.userid)))
                    {
                        Ttoken.username = getUserName(priconn, Ttoken.userid, certtype);
                    }
                    EditToken(PubConn, Ttoken, certtype);
                    #endregion

                    return Ttoken;
                }
            }
            catch (Exception ex)
            {
                result.code = (int)CertCodeEnum.NormalError;
                result.msg = ex.Message;
                return null;
            }
        }

        public override AuthToken RefreshToken(string token)
        {
           
            result = new XXF.Api.ClientResult();
            if (string.IsNullOrEmpty(token))
            {
                result.code = (int)CertCodeEnum.NotExist;
                result.msg = "token不能为空。";
                return null;
            }
            try
            {
                using (XXF.Db.DbConn PubConn = CreateQxConn())
                {
                    PubConn.Open();//打开基本


                    AuthToken Ttoken = RefreshToken(PubConn, token, "", certtype);
                    if (Ttoken == null)
                    {
                        result.code = (int)CertCodeEnum.NotExist;
                        result.msg = "token不存在或已过期";
                        return null;
                    }

                    #region 修改名字,除去修改了用户名后不同步的bug
                    using (XXF.Db.DbConn priconn = XXF.Db.DbConn.CreateConn(GetConnStr(certtype, Ttoken.userid)))
                    {
                        Ttoken.username = getUserName(priconn, Ttoken.userid, certtype);
                    }
                    EditToken(PubConn, Ttoken, certtype);
                    #endregion
                    result.code = (int)CertCodeEnum.Success;
                    return Ttoken;
                }
            }
            catch (Exception ex)
            {
                result.code = (int)CertCodeEnum.NormalError;
                result.msg = ex.Message;
                return null;
            }
        }

        public override List<api> GetApiList(string token)
        {
            return base.GetApiList(token);
        }

        public override AuthToken Login(string userid, string pwd, string appid, string appsecret)
        {
            try
            {
                using (XXF.Db.DbConn PubConn = CreateQxConn())
                {
                    pwd = XXF.Db.LibCrypto.MD5(pwd);
                    PubConn.Open();//打开基本
                    result = new XXF.Api.ClientResult();
                    string dbappsecret = GetAppSecretInDB(PubConn, appid);
                    if (string.IsNullOrEmpty(dbappsecret))
                    {
                        result.code = (int)CertCodeEnum.NormalError;
                        result.msg = "appid不存在";
                        return null;
                    }
                    if (appsecret != dbappsecret)
                    {
                        result.code = (int)CertCodeEnum.NormalError;
                        result.msg = "appsecet错误";
                        return null;
                    }
                    string username = "";
                    string identityid = "";
                    string msg = "";
                    string bsconstr = GetConnStr(certtype, userid);
                    using (XXF.Db.DbConn dydpubConn = XXF.Db.DbConfig.CreateConn(XXF.Db.DbType.SQLSERVER, bsconstr))
                    {
                        dydpubConn.Open();
                        ////用户相关验证
                        switch (certtype)
                        {
                            case ServiceCertType.manage:
                                result.code = ManageAccountVali(dydpubConn, userid, pwd, out username, out identityid, out msg);
                                break;
                            case ServiceCertType.shop:
                                string tt = "";
                                result.code = ShopAccountVali(dydpubConn, userid, pwd, out username, out tt, out identityid, out msg);
                                userid = tt;
                                break;
                            case ServiceCertType.user:
                                result.code = UserAccountVali(dydpubConn, userid, pwd, out username, out identityid, out msg);
                                break;
                            default:
                                break;
                        }
                    }
                    if (result.code < 0)
                    {
                        result.msg = msg;
                        return null;
                    }

                    AuthToken Ttoken = GetToken(PubConn, userid, appid, certtype);
                    if (Ttoken == null || Ttoken.expires.CompareTo(DateTime.Now) < 0)
                    {
                        if (Ttoken != null)
                        {
                            DeleteToken(PubConn, Ttoken.token, certtype);
                        }
                        Ttoken = new AuthToken();
                        Ttoken.appid = appid;
                        Ttoken.token = Guid.NewGuid().ToString().Replace("-", "");
                        Ttoken.userid = userid;
                        Ttoken.id = identityid;
                        Ttoken.username = username;
                        Add(PubConn, Ttoken, certtype);
                    }
                    else
                    {
                        Ttoken = RefreshToken(PubConn, Ttoken.token, username, certtype);
                    }
                    result.code = (int)CertCodeEnum.Success;
                    return Ttoken;

                }
            }
            catch (Exception ex)
            {
                result.code = (int)CertCodeEnum.NormalError;
                result.msg = "登录失败";
                return null;
            }
        }

        public override AuthToken Login(System.Web.Mvc.Controller controller)
        {
            return base.Login(controller);
        }

        protected override bool RequestAuthApi(string token, int apiid, string apiname, string area, string controller, string action)
        {
            return base.RequestAuthApi(token, apiid, apiname, area, controller, action);
        }

        private XXF.Db.DbConn CreateQxConn()
        {
            return XXF.Db.DbConfig.CreateConn(XXF.Db.DbConfig.GetConfig("QxConnectString"));
        }

        #region dbact
        private bool DeleteToken(DbConn PubConn, string id, ServiceCertType tokentype)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>();
            Par.Add(new ProcedureParameter("@token", id));

            string Sql = "delete from " + GetTokentype(tokentype) + " where token=@token";
            int rev = PubConn.ExecuteSql(Sql, Par);
            if (rev == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string GetTokentype(ServiceCertType tokentype)
        {
            switch (tokentype)
            {
                case ServiceCertType.manage:
                    return "managetoken";
                case ServiceCertType.shop:
                    return "shoptoken";
                case ServiceCertType.user:
                    return "usertoken";
            }
            throw new Exception("nontypetoken");
        }


        private string GetAppSecretInDB(XXF.Db.DbConn PubConn, string appid)
        {
            string basesql = " select {0} from app,appgrade where app.appgradeno=appgrade.appgradeno and app.apptype=appgrade.apptype  ";

            string sql = string.Format(basesql, "  appid,appname,appsecret,app.apptype,appgrade.appgradeno,appgrade.appgradename,appdesc,freeze ");
            XXF.Db.SimpleProcedureParameter para = new XXF.Db.SimpleProcedureParameter();
            para.Add("@appid", appid);
            sql += " and freeze=0  and appid=@appid";
            DataSet ds = new DataSet();
            PubConn.SqlToDataSet(ds, sql, para.ToParameters());
            if (ds.Tables[0].Rows.Count == 0)
                return null;
            return ds.Tables[0].Rows[0]["appsecret"].ToString();
        }

        private AuthToken GetToken(DbConn PubConn, string token, ServiceCertType tokentype)
        {
            string sql = " select token,userid,id,username,appid,createtime,expires from " + GetTokentype(tokentype) + " where token=@token";
            List<ProcedureParameter> Par = new List<ProcedureParameter>();
            Par.Add(new ProcedureParameter("@token", token));
            DataSet ds = new DataSet();
            PubConn.SqlToDataSet(ds, sql, Par);

            if (ds.Tables[0].Rows.Count != 1)
                return null;
            return CreateModel(ds.Tables[0].Rows[0]);
        }

        private AuthToken GetToken(XXF.Db.DbConn PubConn, string userid, string appid, ServiceCertType tokentype)
        {
            string sql = " select token,userid,id,username,appid,createtime,expires from " + GetTokentype(tokentype) + " where userid=@userid and appid=@appid";
            List<ProcedureParameter> Par = new List<ProcedureParameter>();
            Par.Add(new ProcedureParameter("@userid", userid));
            Par.Add(new ProcedureParameter("@appid", appid));
            DataSet ds = new DataSet();
            PubConn.SqlToDataSet(ds, sql, Par);

            if (ds.Tables[0].Rows.Count == 0)
                return null;
            return CreateModel(ds.Tables[0].Rows[0]);
        }

        private AuthToken CreateModel(DataRow dr)
        {
            return new AuthToken
            {
                //
                token = dr["token"].ToString(),
                //
                userid = dr["userid"].ToString(),
                //
                id = dr["id"].ToString(),
                //
                username = dr["username"].ToString(),
                //
                appid = dr["appid"].ToString(),
                //
                createtime = Convert.ToDateTime(dr["createtime"]),
                //
                expires = Convert.ToDateTime(dr["expires"])
            };
        }

        private static string GetConnStr(ServiceCertType tokentype, string userid = "")
        {
            string constr = "";
            switch (tokentype)
            {
                case ServiceCertType.user:
                    constr = XXF.Db.DbConfig.GetConfig("MainConnectString");
                    break;
                case ServiceCertType.shop:
                    using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn(XXF.Db.DbType.SQLSERVER, XXF.Db.DbConfig.GetConfig("MainConnectString")))
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
                        string t = "server={dbserver};Initial Catalog={dbname}{partitionno};User ID={dbuser};Password={dbpass};";
                        string dqbm = ds.Tables[0].Rows[0]["f_shdqbm"].ToString();
                        throw new Exception("no shop");
                      //  constr = XXF.Db.DbAreaRule.ShopAreaPartitionRule(t, Convert.ToInt32(dqbm));
                    }
                    break;
                case ServiceCertType.manage:
                    constr = XXF.Db.DbConfig.GetConfig("CrmdyConnectString");
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



        /// <summary>根据userid得到管理员工信息</summary>
        private manage getManage(XXF.Db.DbConn PubConn, string userid)
        {
            string sql = "select ygbh,ygmc,ygmm,ygzt,sfsc from tb_userinfo where ygbh=@userid";
            DataSet ds = new DataSet();
            XXF.Db.SimpleProcedureParameter para = new XXF.Db.SimpleProcedureParameter();
            para.Add("@userid", userid);
            PubConn.SqlToDataSet(ds, sql, para.ToParameters());
            if (ds.Tables[0].Rows.Count != 1)
                return null;
            manage model = new manage();
            model.userid = ds.Tables[0].Rows[0]["ygbh"].ToString();
            model.id = model.userid;
            model.username = ds.Tables[0].Rows[0]["ygmc"].ToString();
            model.pwd = privateToDeDES(ds.Tables[0].Rows[0]["ygmm"].ToString());
            model.freeze = Convert.ToInt32(ds.Tables[0].Rows[0]["sfsc"]);
            if (model.freeze == 0)
            {
                model.freeze = 1;
            }
            else
            {
                model.freeze = 0;
            }
            return model;
        }

        /// <summary>根据userid得到管理员工信息</summary>
        private manage getShop(XXF.Db.DbConn PubConn, string userid)
        {
            string sql = "SELECT  f_shzh,f_shsj,f_shmc,f_dlmm,f_sfdj FROM [tb_shop] where f_shzh=@userid or f_shsj=@userid";
            DataSet ds = new DataSet();
            XXF.Db.SimpleProcedureParameter para = new XXF.Db.SimpleProcedureParameter();
            para.Add("@userid", userid);
            PubConn.SqlToDataSet(ds, sql, para.ToParameters());
            if (ds.Tables[0].Rows.Count != 1)
                return null;
            manage model = new manage();
            model.userid = ds.Tables[0].Rows[0]["f_shzh"].ToString();
            model.id = model.userid;
            model.username = ds.Tables[0].Rows[0]["f_shmc"].ToString();
            model.pwd = privateToDeDES(ds.Tables[0].Rows[0]["f_dlmm"].ToString());
            model.freeze = Convert.ToInt32(ds.Tables[0].Rows[0]["f_sfdj"]);
            return model;
        }
        /// <summary></summary>
        private manage getUser(XXF.Db.DbConn PubConn, string userid)
        {
            string sql = "select f_id, f_yhzh,f_yhxm,f_dlmm,f_sfdj from tb_customer where f_yhzh=@userid";
            DataSet ds = new DataSet();
            XXF.Db.SimpleProcedureParameter para = new XXF.Db.SimpleProcedureParameter();
            para.Add("@userid", userid);
            PubConn.SqlToDataSet(ds, sql, para.ToParameters());
            if (ds.Tables[0].Rows.Count != 1)
                return null;
            manage model = new manage();
            model.userid = ds.Tables[0].Rows[0]["f_yhzh"].ToString();
            model.id = ds.Tables[0].Rows[0]["f_id"].ToString();
            model.username = ds.Tables[0].Rows[0]["f_yhxm"].ToString();
            model.pwd = privateToDeDES(ds.Tables[0].Rows[0]["f_dlmm"].ToString());
            model.freeze = Convert.ToInt32(ds.Tables[0].Rows[0]["f_sfdj"]);
            return model;
        }

        private string privateToDeDES(string s)
        {
            try
            {
                string r = XXF.Db.LibCrypto.DeDES(s);
                return r;
            }
            catch (Exception ex)
            {
                return s;
                throw new Exception("密码不是密文密码");
            }
        }
        private string getUserName(XXF.Db.DbConn PubConn, string userid, ServiceCertType tokentype)
        {
            manage model = null;
            switch (tokentype)
            {
                case ServiceCertType.user:
                    model = getUser(PubConn, userid); break;
                case ServiceCertType.manage:
                    model = getManage(PubConn, userid);
                    break;
                case ServiceCertType.shop:
                    model = getShop(PubConn, userid);
                    break;
            }
            if (model == null)
                return "";
            return model.username;
        }


        private bool EditToken(DbConn PubConn, AuthToken model, ServiceCertType tokentype)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>()
            {
					//
					new ProcedureParameter("@token",    model.token),
					//
					new ProcedureParameter("@userid",    model.userid),
                    //
					new ProcedureParameter("@id",    model.id),
					//
					new ProcedureParameter("@username",    model.username),
					//
					new ProcedureParameter("@appid",    model.appid),
					//
					new ProcedureParameter("@createtime",    model.createtime),
					//
					new ProcedureParameter("@expires",    model.expires)
            };
            int rev = PubConn.ExecuteSql("update " + GetTokentype(tokentype) + " set userid=@userid,id=@id,username=@username,appid=@appid,createtime=@createtime,expires=@expires where token=@token", Par);
            return rev == 1;

        }

        private AuthToken RefreshToken(XXF.Db.DbConn PubConn, string token, string username, ServiceCertType tokentype)
        {
            AuthToken Token = GetToken(PubConn, token, tokentype);
            if (Token == null)
                return null;
            if (Token.expires.CompareTo(DateTime.Now) < 0)
            {
                DeleteToken(PubConn, token, tokentype);
                return null;
            }
            Token.expires = DateTime.Now.AddMinutes(GetExpiresminutes(tokentype));
            if (!string.IsNullOrEmpty(username))
            {
                Token.username = username;
            }

            EditToken(PubConn, Token, tokentype);
            return Token;
        }

        /// <summary>  得到过期时间 </summary>
        /// <param name="tokentype"></param>
        /// <returns></returns>
        private int GetExpiresminutes(ServiceCertType tokentype)
        {
            int minute = 60;
            string stringminutes = "";
            switch (tokentype)
            {
                case ServiceCertType.user:
                    stringminutes = XXF.Db.DbConfig.GetConfig("UserTokenExpires");
                    if (string.IsNullOrEmpty(stringminutes))
                    {
                        return 432000;//一个月
                    }
                    minute = Convert.ToInt32(stringminutes);
                    break;
                case ServiceCertType.shop:
                    stringminutes = XXF.Db.DbConfig.GetConfig("ShopTokenExpires");
                    if (string.IsNullOrEmpty(stringminutes))
                    {
                        return 14400;//十天
                    }
                    minute = Convert.ToInt32(stringminutes);
                    break;
                case ServiceCertType.manage:
                    stringminutes = XXF.Db.DbConfig.GetConfig("ManageTokenExpires");
                    if (string.IsNullOrEmpty(stringminutes))
                    {
                        return 1440;//一天
                    }
                    minute = Convert.ToInt32(stringminutes);
                    break;
                default:
                    break;
            }
            return minute;
        }

        private int ShopAccountVali(XXF.Db.DbConn PubConn, string userid, string md5pwd, out string username, out string ouserid, out string id, out string msg)
        {
            username = "";
            ouserid = "";
            id = "";
            msg = "";
            manage manager = getShop(PubConn, userid);
            if (manager == null)
            {
                msg = "用户不存在";
                return -112;

            }
            else if (manager.freeze == 1)
            {
                msg = "用户被冻结";
                return -114;
            }
            else if (XXF.Db.LibCrypto.MD5(manager.pwd).ToLower() != md5pwd.ToLower())
            {
                msg = "密码不正确";
                return -113;
            }
            username = manager.username;
            ouserid = manager.userid;
            id = manager.id;
            return 1;
        }

        private int UserAccountVali(XXF.Db.DbConn PubConn, string userid, string md5pwd, out string username, out string id, out string msg)
        {
            username = "";
            id = "";
            msg = "";
            manage manager = getUser(PubConn, userid);
            if (manager == null)
            {
                msg = "用户不存在";
                return -112;

            }
            else if (manager.freeze == 1)
            {
                msg = "用户被冻结";
                return -114;
            }
            else if (XXF.Db.LibCrypto.MD5(manager.pwd).ToLower() != md5pwd.ToLower())
            {
                msg = "密码不正确";
                return -113;
            }
            username = manager.username;
            id = manager.id;
            return 1;
        }

        private int ManageAccountVali(XXF.Db.DbConn PubConn, string userid, string md5pwd, out string username, out string id, out string msg)
        {
            username = "";
            id = "";
            msg = "";
            manage manager = getManage(PubConn, userid);
            if (manager == null)
            {
                msg = "用户不存在";
                return -112;

            }
            else if (manager.freeze == 1)
            {
                msg = "用户被冻结";
                return -114;
            }
            else if (XXF.Db.LibCrypto.MD5(manager.pwd).ToLower() != md5pwd.ToLower())
            {
                msg = "密码不正确";
                return -113;
            }
            username = manager.username;
            id = manager.id;
            return 1;
        }

        /// <summary>
        /// createtime expiresetime不用传。
        /// </summary>
        /// <param name="PubConn"></param>
        /// <param name="model"></param>
        /// <param name="tokentype"></param>
        /// <returns></returns>
        private bool Add(DbConn PubConn, AuthToken model, ServiceCertType tokentype)
        {
            DateTime nowtime = PubConn.GetServerDate();
            model.createtime = nowtime;
            model.expires = nowtime.AddMinutes(GetExpiresminutes(tokentype));
            List<ProcedureParameter> Par = new List<ProcedureParameter>()
                {
					
					//
					new ProcedureParameter("@token",    model.token),
					//
					new ProcedureParameter("@userid",    model.userid),
					new ProcedureParameter("@id",    model.id),
					//
					new ProcedureParameter("@username",    model.username),
					//
					new ProcedureParameter("@appid",    model.appid),
					//
					new ProcedureParameter("@createtime",   model.createtime),
					//
					new ProcedureParameter("@expires", model.expires  )   
                };
            int rev = PubConn.ExecuteSql("insert into " + GetTokentype(tokentype) + " (token,userid,id,username,appid,createtime,expires)" +
                                         "  values(@token,@userid,@id,@username,@appid,@createtime,@expires)", Par);
            return rev == 1;

        }

        #endregion dbact
    }

    public class OnlineCert : ICertProvider
    {
        protected ServiceCertType certtype;
        protected CertApiConfig apiconfig { get; set; }
        public XXF.Api.ClientResult result { get; set; }
        private bool usetokencache = true;
        private bool useapilistcache = false;
        private CertCache CacheAuthModel;
        //new System.Runtime.Caching.MemoryCache("managecache");
        /// <summary>
        /// 默认使用本地token验证。
        /// </summary>
        /// <param name="certtype"></param>
        public OnlineCert(ServiceCertType certtype)
            : this(certtype, true, false) { }

        public OnlineCert(ServiceCertType certtype, bool usetokencache)
            : this(certtype, usetokencache, false) { }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="certtype">验证类型</param>
        /// <param name="usetokencache">是否使用token缓存</param>
        /// <param name="useapilistcache">为所有有权接口缓存，进行本地验证。当此值为true时，usertokencache自动为true.不建议使用</param>
        public OnlineCert(ServiceCertType certtype, bool usetokencache, bool useapilistcache)
        {
            if (useapilistcache)
            {
                usetokencache = true;
            }
            this.certtype = certtype;
            this.useapilistcache = useapilistcache;
            this.usetokencache = usetokencache;
            apiconfig = new CertApiConfig(this.certtype);
            if (usetokencache)
            {
                try
                {
                    CacheAuthModel = new CertCache(certtype);
                }
                catch (Exception ex)//添加缓存失败时自动转为非缓存型。
                {
                    useapilistcache = false;
                    usetokencache = false;
                }
            }
        }

        /// <summary>
        /// 登录，从config中得到appid appsecret ，直接登录(指当前应用为非api，不调用api登录接口，直接请求权限中心登录)
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public AuthToken Login(string userid, string pwd)
        {
            string appid = Authcomm.GetAppConfig("appid");
            string appsecret = Authcomm.GetAppConfig("appsecret");
            return Login(userid, pwd, appid ?? "", appsecret ?? "");
        }

        /// <summary>
        /// 登录 直接登录(指当前应用为非api，不调用api登录接口，直接请求权限中心登录)
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="pwd"></param>
        /// <param name="appid"></param>
        /// <param name="appsecret"></param>
        /// <returns></returns>
        public virtual AuthToken Login(string userid, string pwd, string appid, string appsecret)
        {
            List<ParmField> para = new List<ParmField>();
            para.Add(new StringField("userid", userid ?? ""));
            para.Add(new StringField("pwd", Authcomm.ToMD5String(pwd ?? "")));
            para.Add(new StringField("appid", appid ?? ""));
            para.Add(new StringField("timespan", Authcomm.GetTimeSpan()));
            Authcomm.ToSign(para, appsecret ?? "");
            AuthToken Ttoken = ApiHelper<AuthToken>(() =>
            {
                return HttpServer.InvokeApi(apiconfig.tokenurl, para);
            });
            if (Ttoken != null && usetokencache)
            {
                CacheAuthModel.AddOrUpdateToken(Ttoken);
            }
            return Ttoken;
        }

        /// <summary>
        /// app请求登录，直接转certcenter进行登录验证。
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public virtual AuthToken Login(Controller controller)
        {
            List<ParmField> para = XXF.Api.Authcomm.GetRequestPara(controller.Request);
            AuthToken Ttoken = ApiHelper<AuthToken>(() =>
            {
                return HttpServer.InvokeApi(apiconfig.tokenurl, para);
            });
            if (Ttoken != null && usetokencache)
            {
                CacheAuthModel.AddOrUpdateToken(Ttoken);
            }
            return Ttoken;
        }

        public bool Auth(string token, int apiid)
        {
            return Auth(token, apiid, "", "", "", "");
        }

        public bool Auth(string token, string apiname)
        {
            return Auth(token, 0, apiname, "", "", "");
        }

        public bool Auth(string token, string area, string controller, string action)
        {
            return Auth(token, 0, "", area, controller, action);
        }

        public bool Auth(string token, Controller c)
        {
            string _area = "";
            string _controller = "";
            string _action = "";
            foreach (var a in RouteTable.Routes)
            {
                RouteData rd = a.GetRouteData(c.HttpContext);
                if (rd == null)
                {
                    continue;
                }
                _controller = rd.Values["controller"].ToString();
                try
                {
                    _area = rd.DataTokens["area"].ToString();
                }
                catch (Exception e)
                {
                    try
                    {
                        _area = rd.Values["area"].ToString();
                    }
                    catch (Exception ee)
                    {

                    }
                }
                _action = rd.Values["action"].ToString();
                break;
            }
            return Auth(token, 0, "", _area, _controller, _action);
        }

        /// <summary>验证token是否有效或是否已登录。</summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool Auth(string token)
        {
            if (usetokencache)
            {
                return cacheAuth(token, 0, "", "", "", "", true);
            }
            AuthToken Ttoken = ReqGetTokenInfo(token);
            return Ttoken != null;
        }

        private bool Auth(string token, int apiid, string apiname, string area, string controller, string action)
        {
            if (useapilistcache)//本地验证
            {
                return cacheAuth(token, apiid, apiname, area, controller, action, false);
            }

            return RequestAuthApi(token, apiid, apiname, area, controller, action);
        }

        protected virtual bool RequestAuthApi(string token, int apiid, string apiname, string area, string controller, string action)
        {
            List<ParmField> para = new List<ParmField>();
            para.Add(new StringField("token", token ?? ""));

            para.Add(new StringField("apiid", apiid));
            para.Add(new StringField("apiname", apiname ?? ""));

            para.Add(new StringField("area", area ?? ""));
            para.Add(new StringField("controller", controller ?? ""));
            para.Add(new StringField("action", action ?? ""));
            return ApiHelper<bool>(() =>
            {
                return HttpServer.InvokeApi(apiconfig.authurl, para);
            });
        }

        /// <summary> 刷新token过期时间 返回新token信息 </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual AuthToken RefreshToken(string token)
        {
            List<ParmField> para = new List<ParmField>();
            para.Add(new StringField("token", token ?? ""));
            return ApiHelper<AuthToken>(() =>
            {
                return HttpServer.InvokeApi(apiconfig.tokeninfourl, para);
            });
        }

        /// <summary>
        /// 缓存验证过 初次或过期时重新得到所有有权接口列表。 配置参数名:AuthCacheExpiresMin 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="apiid"></param>
        /// <param name="apiname"></param>
        /// <param name="area"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private bool cacheAuth(string token, int apiid, string apiname, string area, string controller, string action, bool justauth)
        {
            if (CacheAuthModel == null)
                return false;
            CertCacheItem cacheauthitem = CacheAuthModel.GetCacheToken(token);
            if (cacheauthitem == null)//未加入缓存或不存在
            {
                AuthToken tempauth = ReqGetTokenInfo(token);
                if (tempauth == null)
                    return false;
                cacheauthitem = new CertCacheItem();
                cacheauthitem.Token = tempauth;
                CacheAuthModel.AddOrUpdateToken(tempauth);
            }
            if (justauth)
            {
                return true;
            }
            if (cacheauthitem.apis == null)//不存在权限接口列表
            {
                cacheauthitem.apis = GetApiList(token);
                if (cacheauthitem.apis == null)
                    return false;
                CacheAuthModel.SetCacheApis(token, cacheauthitem.apis);
            }
            if (((cacheauthitem.Token.expires - cacheauthitem.Token.lastauth).TotalSeconds / 2) >= ((cacheauthitem.Token.expires - DateTime.Now).TotalSeconds))//距过期还有一半时间时重新得到。
            {
                //如果当前
                CacheAuthModel.DeleteToken(token);
                cacheauthitem.Token = RefreshToken(token);
                if (cacheauthitem.Token == null)
                    return false;
                CacheAuthModel.AddOrUpdateToken(cacheauthitem.Token);
                return cacheAuth(token, apiid, apiname, area, controller, action, justauth);
            }
            return cacheTestAuth(apiid, apiname, area, controller, action, cacheauthitem);//验证权限
        }

        /// <summary>
        /// 缓存权限验证
        /// </summary>
        /// <param name="apiid"></param>
        /// <param name="apiname"></param>
        /// <param name="area"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private bool cacheTestAuth(int apiid, string apiname, string area, string controller, string action, CertCacheItem tokenandapis)
        {
            if (apiid > 0)
            {
                return tokenandapis.apis.FirstOrDefault(x => x.apiid == apiid && x.freeze == 0) != null;
            }
            if (!string.IsNullOrEmpty(apiname))
            {
                return tokenandapis.apis.FirstOrDefault(x => x.apiname == apiname && x.freeze == 0) != null;
            }
            //if (string.IsNullOrEmpty(area) && string.IsNullOrEmpty(controller) && string.IsNullOrEmpty(action))
            //{
            //    return 
            //}
            return tokenandapis.apis.FirstOrDefault(x => x.area == area && x.controller == controller && x.action == action && x.freeze == 0) != null;

        }
        /// <summary>得到授权接口列表</summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual List<api> GetApiList(string token)
        {
            List<ParmField> para = new List<ParmField>();
            para.Add(new StringField("token", token ?? ""));
            return ApiHelper<List<api>>(() =>
            {
                return HttpServer.InvokeApi(apiconfig.apilisturl, para);
            });
        }

        /// <summary>得到token对应信息</summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual AuthToken ReqGetTokenInfo(string token)
        {
            List<ParmField> para = new List<ParmField>();
            para.Add(new StringField("token", token ?? ""));
            return ApiHelper<AuthToken>(() =>
            {
                return HttpServer.InvokeApi(apiconfig.tokeninfourl, para);
            });
        }
        /// <summary>
        /// 得到Token的详细信息，包含用户名、过期时间、appid等。 无时返回null
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public AuthToken GetTokenInfo(string token)
        {
            if (usetokencache && CacheAuthModel != null)
            {
                CertCacheItem item = CacheAuthModel.GetCacheToken(token);
                if (item != null)
                    return item.Token;
            }
            AuthToken Ttoken = ReqGetTokenInfo(token);
            if (Ttoken != null && usetokencache && CacheAuthModel != null)
            {
                CacheAuthModel.AddOrUpdateToken(Ttoken);
            }
            return Ttoken;
        }

        /// <summary>
        /// 得到用户名， 空时返回空字符串 而非null
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string GetUserName(string token)
        {
            AuthToken Ttoken = GetTokenInfo(token);
            if (Ttoken == null)
                return "";
            return Ttoken.username ?? "";
        }
        /// <summary>
        /// 验证签名，请求是否超时，如果返回为false时，验证结果信息在result变量里。 
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool TestSignAndTime(Controller controller)
        {
            Dictionary<string, string> para = XXF.Api.Authcomm.GetRequestPara<Dictionary<string, string>>(controller.Request);
            if (!para.ContainsKey("sign"))
            {
                result = new ClientResult();
                result.code = -111;
                result.msg = "sign不能为空。";
                return false;
            }

            if (!para.ContainsKey("timespan"))
            {
                result = new ClientResult();
                result.code = -111;
                result.msg = "timespan不能为空。";
                return false;
            }

            string newsign = XXF.Api.Authcomm.ToSign(para, XXF.Api.Authcomm.GetAppConfig("AppSecret"));
            if (newsign.ToUpper() != para["sign"].ToUpper())
            {
                result = new ClientResult();
                result.code = -104;
                //result.msg = Api.AUTH_CODE_MSG.Get(result.code);
                return false;
            }

            long timespan = 0;
            long.TryParse(para["timespan"], out timespan);
            if (timespan > XXF.Api.Authcomm.GetTimeSpan() + (10 * 60) || timespan < XXF.Api.Authcomm.GetTimeSpan() - (10 * 60))
            {
                result = new ClientResult();
                result.code = -102;//请求超时
                //result.msg = Api.AUTH_CODE_MSG.Get(result.code);
                return false;
            }
            return true;
        }

        /// <summary>
        /// appid和token +appsecret md5 老接口验证
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public bool TestSign(Controller controller)
        {
            string appid = controller.Request.Params["appid"];
            string token = controller.Request.Params["token"];
            string sign = controller.Request.Params["sign"];


            if (string.IsNullOrEmpty(appid))
            {
                result = new ClientResult();
                result.code = -111;
                result.msg = "appid不能为空。";
                return false;
            }
            if (string.IsNullOrEmpty(token))
            {
                result = new ClientResult();
                result.code = -905;
                result.msg = "token不能为空。";
                return false;
            }
            if (string.IsNullOrEmpty(sign))
            {
                result = new ClientResult();
                result.code = -111;
                result.msg = "sign不能为空。";
                return false;
            }

            string appsecret = GetAppSecret(appid);

            if (string.IsNullOrEmpty(appsecret))
            {
                result = new ClientResult();
                result.code = -111;
                result.msg = "appid不存在。";
                return false;
            }

            string tomd5 = XXF.Api.Authcomm.ToMD5String("appid" + appid + "appsecret" + appsecret + "token" + token);

            if (tomd5.ToUpper() != sign.ToUpper())
            {
                result = new ClientResult();
                result.code = -104;
                result.msg = "sign不正确";
                return false;
            }
            return true;
        }

        public string GetAppSecret(string appid)
        {
            AppCache ac = new AppCache();
            try
            {
                AppModel model = ac.GetApp(appid);
                if (model == null)
                {
                    string se = ReqGetAppSecret(appid);
                    if (string.IsNullOrEmpty(se))
                    {
                        return null;
                    }
                    else
                    {
                        ac.AddApp(new AppModel() { appid = appid, appsecret = se });
                        return se;
                    }
                }
                else
                {
                    return model.appsecret;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        protected virtual string ReqGetAppSecret(string appid)
        {
            List<ParmField> para = new List<ParmField>();
            para.Add(new StringField("appid", appid ?? ""));
            return ApiHelper<string>(() =>
            {
                return HttpServer.InvokeApi(apiconfig.appsecreturl, para);
            });
        }

        /// <summary>
        /// 检查请求是否传入了token false时返回信息在result中
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public bool TestTokenExist(Controller controller)
        {
            return !string.IsNullOrEmpty(GetTokenFromReq(controller));
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public bool LogOut(Controller controller)
        {
            string token = GetTokenFromReq(controller);
            return LogOut(token);
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public bool LogOut(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                result.code = -905;
                result.msg = "token不能为空";
            }

            //删除本地缓存的认证
            if (CacheAuthModel == null)
            {
                CacheAuthModel = new CertCache(certtype);
            }
            CacheAuthModel.DeleteToken(token);
            return RequestLogout(token);
        }

        protected virtual bool RequestLogout(string token)
        {
            List<ParmField> para = new List<ParmField>();
            para.Add(new StringField("token", token));
            return ApiHelper<bool>(() =>
            {
                return HttpServer.InvokeApi(apiconfig.logouturl, para);
            });
        }

        /// <summary>
        /// 从请求中得到token 字符串
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public string GetTokenFromReq(Controller controller)
        {
            string token = "";
            if (controller.Request.Form.AllKeys.Contains("token"))
            {
                token = controller.Request.Form["token"];
            }
            return token;
        }

        /// <summary>
        /// 当前 businessapi customerapi 应采用的方法 包含 签名和登录验证 为false时 ressult.msg 里包含错误信息。
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public bool SuperAuth(Controller controller)
        {
            if (!TestSign(controller))
            {
                return false;
            }
            string token = GetTokenFromReq(controller);

            return Auth(token);
        }

        /// <summary>
        /// 删除缓存中的认证信息，当前用于修改用户名后重得到用户名，防从缓存中得到。
        /// </summary>
        /// <param name="token"></param>
        public void DeleteCacheInfo(string token)
        {
            //删除本地缓存的认证
            if (CacheAuthModel == null)
            {
                CacheAuthModel = new CertCache(certtype);
            }
            CacheAuthModel.DeleteToken(token);
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

    public interface ICertProvider
    {
        ClientResult result { get; set; }
        AuthToken Login(string userid, string pwd);
        AuthToken Login(string userid, string pwd, string appid, string appsecret);
        AuthToken Login(Controller controller);
        bool Auth(string token, int apiid);
        bool Auth(string token, string apiname);
        bool Auth(string token, string area, string controller, string action);
        bool Auth(string token, Controller c);
        bool Auth(string token);
        AuthToken RefreshToken(string token);
        List<api> GetApiList(string token);
        AuthToken GetTokenInfo(string token);
        string GetUserName(string token);
        bool TestSignAndTime(Controller controller);
        bool TestSign(Controller controller);
        string GetAppSecret(string appid);
        bool TestTokenExist(Controller controller);
        bool LogOut(Controller controller);
        bool LogOut(string token);
        string GetTokenFromReq(Controller controller);
        bool SuperAuth(Controller controller);
        void DeleteCacheInfo(string token);
    }

    public class manage
    {
        public string id { get; set; }
        public string userid { get; set; }
        public string username { get; set; }
        public string pwd { get; set; }
        public int freeze { get; set; }
    }
}
