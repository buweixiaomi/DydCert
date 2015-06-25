using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CertCenter.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Login(string appid, string returnurl)
        {
            if (!string.IsNullOrEmpty(appid))
            {
                using (XXF.Db.DbConn pubconn = XXF.Db.DbConfig.CreateConn())
                {
                    pubconn.Open();
                    string constr = CertCenter.Areas.CertApi.Models.ApiCommDal.GetConnStr(Models.DbModels.TokenType.managetoken);
                    if (!string.IsNullOrEmpty(appid))
                    {
                        CertCenter.Models.DbModels.app app = CertCenter.Models.AppDal.Instance.GetAppInfo(pubconn, appid);
                        if (app == null)
                        {
                            ViewBag.msg = "应用不存在";
                        }
                        else
                        {
                            ViewBag.appname = app.appname;
                        }
                    }
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(string userid, string pwd, string returnurl, string appid)
        {
            XXF.Db.DbConn pubconn = null;
            XXF.Db.DbConn dbconn = null;
            try
            {
                pubconn = XXF.Db.DbConfig.CreateConn();
                pubconn.Open();
                ViewBag.userid = userid;
                ViewBag.pwd = pwd;
                string constr = CertCenter.Areas.CertApi.Models.ApiCommDal.GetConnStr( Models.DbModels.TokenType.managetoken);
                if (!string.IsNullOrEmpty(appid))
                {
                    CertCenter.Models.DbModels.app app = CertCenter.Models.AppDal.Instance.GetAppInfo(pubconn, appid);
                    if (app == null)
                    {
                        ViewBag.msg = "应用不存在";
                        return View();
                    }
                }

                dbconn = XXF.Db.DbConfig.CreateConn(constr);
                dbconn.Open();
                Models.DbModels.manage model = Models.AccountDal.Instance.getManage(dbconn, userid);
                if (model == null)
                {
                    ViewBag.msg = "用户名不存在";
                    return View();
                }
                if (model.freeze == 1)
                {
                    ViewBag.msg = "用户已被冻结";
                    return View();
                }
                if (pwd != model.pwd)
                {
                    ViewBag.msg = "密码不正确";
                    return View();
                }
                //if (string.IsNullOrEmpty(appid))
                //{

                FormsAuthentication.SetAuthCookie(userid + " " + model.username, false);
                return RedirectToAction("Index", "Home");
                //}
                //else
                //{
                //    if (returnurl.Contains("?")&&returnurl.Contains("&"))
                //    {
                //        returnurl = returnurl + "&token="+;
                //    }
                //    return Redirect(returnurl);
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (pubconn != null)
                    pubconn.Dispose();
                if (dbconn != null)
                {
                    dbconn.Dispose();
                }
            }
        }


        public ActionResult Signout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("login", "account");
        }
    }

}
