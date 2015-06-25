using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CertCenter.Areas.CertApi.Models
{
    public class AppSeviceDal
    {
        public static readonly AppSeviceDal Instance = new AppSeviceDal();
        public ActionResult GetAppSecret(Controller controller)
        {
            JsonResult jsonr = new JsonResult();
            jsonr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            CertComm.ServerResult r = new CertComm.ServerResult();
            jsonr.Data = r;
            string appid = controller.Request["appid"];
            if (string.IsNullOrEmpty(appid))
            {
                r.code = -3;
                r.msg = "请求参数appid不能为空";
                return jsonr;
            }
            using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
            {
                PubConn.Open();//打开基本
                bool ex = CertCenter.Models.AppDal.Instance.ExitAppid(PubConn, appid);
                if (!ex)
                {
                    r.code = -1;
                    r.msg = "appid不存在";
                    return jsonr;
                }

                CertCenter.Models.DbModels.app app = CertCenter.Models.AppDal.Instance.GetAppInfo(PubConn, appid);
                if (app == null)
                {
                    r.code = -1;
                    r.msg = "appid不存在";
                    return jsonr;
                }
                else
                {
                    r.code = 1;
                    r.msg = "OK";
                    r.response = app.appsecret;
                    return jsonr;
                }
            }

            return jsonr;
        }


    }
}