using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CertCenter.Areas.CertApi.Controllers
{
    public class UserTokenController : Controller
    {
        CertCenter.Models.DbModels.TokenType tokentype = CertCenter.Models.DbModels.TokenType.usertoken;
        public ActionResult Get(string appid, string userid, string timespan, string sign, string pwd)
        {

            //JsonResult jsonr = new JsonResult();
            //jsonr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //CertComm.ServerResult sr = new CertComm.ServerResult();
            //string msg = "";
            //jsonr.Data = sr;
            //sr.code = 1;
            //sr.response = new CertCenter.Models.DbModels.tb_token()
            //{

            //    appid = "",
            //    createtime = DateTime.Now,
            //    expires = DateTime.Now,
            //    id = "",
            //    token = Guid.NewGuid().ToString().Replace("-", ""),
            //    userid = "",
            //    username = ""
            //};
            //return jsonr;
            return Models.ApiControllerHelper.Visit(() =>
            {
                return Models.ApiCommDal.GetToken(this, tokentype);
            }, this);
        }

        public ActionResult Auth()
        {
            return Models.ApiControllerHelper.Visit(() =>
            {
                return Models.ApiCommDal.TestAuth(this, tokentype);
            }, this);
        }

        public ActionResult GetApiList()
        {
            return Models.ApiControllerHelper.Visit(() =>
            {
                return Models.ApiCommDal.GetApiList(this, tokentype);
            }, this);
        }

        public ActionResult GetTokenInfo()
        {
            return Models.ApiControllerHelper.Visit(() =>
            {
                return Models.ApiCommDal.GetTokenInfo(this, tokentype);
            }, this);
        }
        public ActionResult RefreshToken()
        {
            return Models.ApiControllerHelper.Visit(() =>
            {
                return Models.ApiCommDal.RefreshToken(this, tokentype);
            }, this);
        }
        public ActionResult LogOut()
        {
            return Models.ApiControllerHelper.Visit(() =>
            {
                return Models.ApiCommDal.LogOut(this, tokentype);
            }, this);
        }

    }
}
