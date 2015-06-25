using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CertCenter.Controllers
{
    public class MonitorController : Controller
    {
        //
        // GET: /Monitor/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Getdata()
        {
            var moni = ApiInvokeMap.MapCore.GetInstance();
            return Json(new
            {
                code = 1,
                data = new
                {
                    wraptime = moni.GetWrapTime(),
                    maps = moni.GetMap()
                },
                msg = "ok"
            });
        }

        public JsonResult InitMonitor(int miseconds)
        {
            var moni = ApiInvokeMap.MapCore.GetInstance();
            moni.InitWrap(miseconds);
            return Json(new { code = 1, data = "ok", msg = "ok" });
        }

    }
}
