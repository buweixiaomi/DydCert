using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CertCenter.Controllers
{
    [Authorize]
    public class WorkLogController : Controller
    {
        //
        // GET: /WorkLog/

        public ActionResult Index(int pno = 1, string keywords = "")
        {
            using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
            {
                PubConn.Open();
                int pagesize = 10;
                int totalcount = 0;
                List<Models.DbModels.certcenterlog> listtoken = Models.CertCenterLogDal.Instance.GetPage(PubConn, pno, pagesize, keywords, out totalcount);
                Webdiyer.WebControls.Mvc.PagedList<Models.DbModels.certcenterlog> pager = new Webdiyer.WebControls.Mvc.PagedList<Models.DbModels.certcenterlog>(listtoken, pno, pagesize, totalcount);
                if (Request.IsAjaxRequest())
                {
                    return PartialView("List", pager);
                }
                return View(pager);

            }
        }


        public JsonResult DeleteLog(int id)
        {
            using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
            {
                PubConn.Open();
                bool r = Models.CertCenterLogDal.Instance.Delete(PubConn, id);
                Models.DbModels.certcenterlog log = new Models.DbModels.certcenterlog(this);
                log.opecontent = "删除操作日志。结果：" + r;
                Models.CertCenterLogDal.Instance.Add(PubConn, log);
                if (r)
                {
                    return Json(new { code = 1 });
                }
                else
                {
                    return Json(new { code = -1, msg = "删除失败，日志不存在或存已被删除。" });
                }
            }
        }


    }
}
