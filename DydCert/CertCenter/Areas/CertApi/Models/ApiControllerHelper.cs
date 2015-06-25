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
            Task<ActionResult> t = new Task<ActionResult>(action);
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            #region 失败
            //t.ContinueWith((x) =>
            //{
            //    try
            //    {
            //        System.IO.File.AppendAllText(c.Server.MapPath("~/error.log"), DateTime.Now.ToString() + "\r\n\tURL:" + c.Request.Url.ToString() + "\r\n\tFormData:" + System.Web.HttpUtility.UrlDecode(c.Request.Form.ToString()) + "\r\n\tException:" + x.Exception.StackTrace + "\r\n\t" + x.Exception.Message + "\r\n");
            //    }
            //    catch { }
            //}, TaskContinuationOptions.OnlyOnFaulted);
            #endregion

            #region 成功
            t.ContinueWith((x) =>
            {
                try
                {
                    System.IO.File.AppendAllText(c.Server.MapPath("~/ope" + DateTime.Now.ToString("yyMMdd") + ".log"), DateTime.Now.ToString() + " TIME:"+sw.Elapsed.TotalMinutes+" \r\n\tFormData:" + System.Web.HttpUtility.UrlDecode(c.Request.Form.ToString()) + "\r\n\tURL:" + c.Request.Url.ToString() + "\r\n");
                }
                catch (Exception ex) { }
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
            #endregion

            try
            {
                t.Start();
                if (t.Wait(TimeSpan.FromSeconds(30)))
                {
                    ActionResult actresult = t.Result;
                    return actresult;
                }
                throw new Exception("[task time out]");
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() =>
                {
                    System.IO.File.AppendAllText(c.Server.MapPath("~/error.log"), DateTime.Now.ToString() + "\r\n\tURL:" + c.Request.Url.ToString() + "\r\n\tFormData:" + System.Web.HttpUtility.UrlDecode(c.Request.Form.ToString()) + "\r\n\tException:" + ex.Message + "\r\n" + ex.StackTrace + "\r\n\t");
                });
                JsonResult sresult = new JsonResult();
                CertComm.ServerResult r = new CertComm.ServerResult();
                r.code = -100;
                r.msg = "系统正忙或服务器内部错误，请重试。";
                sresult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                sresult.Data = r;
                return sresult;
            }
        }
    }
}