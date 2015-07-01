using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CertCenter.Areas.CertApi.Models
{
    public class ApiControllerHelper
    {
        public static ActionResult Visit(Func<ActionResult> action, Controller c)
        {

            var mapapi = ApiInvokeMap.MapCore.GetInstance();
            mapapi.Increase(ConbineUrl(
                XXF.Db.LibConvert.NullToStr(c.ControllerContext.RouteData.Values["area"]).ToString().ToLower(),
                c.ControllerContext.RouteData.Values["controller"].ToString().ToLower(),
                c.ControllerContext.RouteData.Values["action"].ToString().ToLower()));

            //XXF.Log.TimeWatchLog twl = new XXF.Log.TimeWatchLog();
            try
            {
                var r = action();
                //twl.Write(c.Request.Url.ToString());
                return r;
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(c.Server.MapPath("~/error.log"), DateTime.Now.ToString() + "\r\n\tURL:" + c.Request.Url.ToString() + "\r\n\tFormData:" + System.Web.HttpUtility.UrlDecode(c.Request.Form.ToString()) + "\r\n\tException:" + ex.Message + "\r\n" + ex.StackTrace + "\r\n\t");
                JsonResult sresult = new JsonResult();
                CertComm.ServerResult r = new CertComm.ServerResult();
                r.code = -100;
                r.msg = "系统正忙或服务器内部错误，请重试。";
                sresult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                sresult.Data = r;
                return sresult;
            }

            #region old
            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //sw.Start();

            //Task<ActionResult> t = new Task<ActionResult>(action);
            //#region 成功
            //t.ContinueWith((x) =>
            //{
            //    try
            //    {
            //        System.IO.File.AppendAllText(c.Server.MapPath("~/ope" + DateTime.Now.ToString("yyMMdd") + ".log"), DateTime.Now.ToString() + " TIME:" + sw.Elapsed.TotalMinutes + " \r\n\tFormData:" + System.Web.HttpUtility.UrlDecode(c.Request.Form.ToString()) + "\r\n\tURL:" + c.Request.Url.ToString() + "\r\n");
            //    }
            //    catch (Exception ex) { }
            //}, TaskContinuationOptions.OnlyOnRanToCompletion);
            //#endregion

            //try
            //{
            //    t.Start();
            //    if (t.Wait(TimeSpan.FromSeconds(30)))
            //    {
            //        ActionResult actresult = t.Result;
            //        return actresult;
            //    }
            //    throw new Exception("[task time out]");
            //}
            //catch (Exception ex)
            //{
            //    Task.Factory.StartNew(() =>
            //    {
            //        System.IO.File.AppendAllText(c.Server.MapPath("~/error.log"), DateTime.Now.ToString() + "\r\n\tURL:" + c.Request.Url.ToString() + "\r\n\tFormData:" + System.Web.HttpUtility.UrlDecode(c.Request.Form.ToString()) + "\r\n\tException:" + ex.Message + "\r\n" + ex.StackTrace + "\r\n\t");
            //    });
            //    JsonResult sresult = new JsonResult();
            //    CertComm.ServerResult r = new CertComm.ServerResult();
            //    r.code = -100;
            //    r.msg = "系统正忙或服务器内部错误，请重试。";
            //    sresult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //    sresult.Data = r;
            //    return sresult;
            //}
            #endregion
        }

        public static string ConbineUrl(string area, string controller, string action)
        {
            return string.Format("{0}{1}{2}",
                string.IsNullOrEmpty(area) ? "" : "/" + area,
                string.IsNullOrEmpty(controller) ? "" : "/" + controller,
                string.IsNullOrEmpty(action) ? "" : "/" + action
                );
        }
    }
}