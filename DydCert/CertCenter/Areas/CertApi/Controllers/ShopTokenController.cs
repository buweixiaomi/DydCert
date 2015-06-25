using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CertCenter.Areas.CertApi.Controllers
{
    public class ShopTokenController : Controller
    {
        CertCenter.Models.DbModels.TokenType tokentype = CertCenter.Models.DbModels.TokenType.shoptoken;
        public ActionResult Get(string appid, string userid, string timespan, string sign, string pwd)
        {
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
