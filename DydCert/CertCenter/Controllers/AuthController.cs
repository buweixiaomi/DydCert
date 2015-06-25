using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CertCenter.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        //
        // GET: /Toekn/

        public ActionResult Index(int pno = 1, string keywords = "", Models.DbModels.TokenType tokentype = Models.DbModels.TokenType.usertoken)
        {
            using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
            {
                PubConn.Open();
                int pagesize = 10;
                int totalcount = 0;
                List<Models.DbModels.tb_token> listtoken = Models.TokenDal.Instance.GetByPage(PubConn, pno, pagesize, keywords, tokentype, out totalcount);
                Webdiyer.WebControls.Mvc.PagedList<Models.DbModels.tb_token> pager = new Webdiyer.WebControls.Mvc.PagedList<Models.DbModels.tb_token>(listtoken, pno, pagesize, totalcount);
                if (Request.IsAjaxRequest())
                {
                    return PartialView("List", pager);
                }
                return View(pager);

            }
        }

        public JsonResult deletetoken(string id, Models.DbModels.TokenType tokentype)
        {
            using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
            {
                PubConn.Open();

                bool r = Models.TokenDal.Instance.Delete(PubConn, id, tokentype);

                Models.DbModels.certcenterlog log = new Models.DbModels.certcenterlog(this);
                log.opecontent = "删除token。tokentype="+tokentype.ToString()+" result="+r;
                Models.CertCenterLogDal.Instance.Add(PubConn, log);
                if (r)
                {
                    return Json(new { code = 1 });
                }
                else
                {
                    return Json(new { code = -905, msg = "删除失败，token不存在或存已被删除。" });
                }
            }
        }

    }
}
