using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CertCenter.Controllers
{
    [Authorize]
    public class ApiController : Controller
    {
        //
        // GET: /Api/

        public ActionResult Index()
        {
            using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
            {
                PubConn.Open();
                Dictionary<string, List<Models.DbModels.appcategory>> apis = new Dictionary<string, List<Models.DbModels.appcategory>>();
                for (int i = 0; i <= 2; i++)
                {
                    List<Models.DbModels.appcategory> tempcategory = Models.AppcategoryDal.Instance.GetAppTypeCategorys(PubConn, i);
                    apis.Add(Models.CertCenterComm.APPTYPENAME[i], tempcategory);
                }
                return View(apis);
            }
        }

        public ActionResult GetApiList(int apptype = 0, int categoryid = 1)
        {
            using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
            {
                PubConn.Open();
                List<Models.DbModels.api> listapi = Models.ApiDal.Instance.GetCategoryApis(PubConn, apptype, categoryid);
                return Json(new { code = 1, response = listapi });
            }
        }

        public JsonResult edit(Models.DbModels.api model)
        {
            if (string.IsNullOrEmpty(model.apititle))
            {
                return Json(new { code = -1, msg = "请输入接口标题" });
            }
            using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
            {
                PubConn.Open();

                Models.DbModels.certcenterlog log = new Models.DbModels.certcenterlog(this);
                try
                {
                    if (model.apiid == 0)
                    {
                        if (Models.ApiDal.Instance.Add(PubConn, model))
                        {
                            model.apiid = PubConn.GetIdentity();
                            return Json(new { code = 1, response = model });
                        }
                        else
                        {
                            return Json(new { code = -1, msg = "添加失败" });
                        }
                    }
                    else
                    {
                        if (Models.ApiDal.Instance.Edit(PubConn, model))
                        {
                            return Json(new { code = 1, response = model });
                        }
                        else
                        {
                            return Json(new { code = -1, msg = "修改失败" });
                        }
                    }
                }
                finally
                {

                    log.opecontent = "修改接口";
                    Models.CertCenterLogDal.Instance.Add(PubConn, log);
                }
            }

        }

        public JsonResult apidetails(int id)
        {
            using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
            {
                PubConn.Open();
                Models.DbModels.api model = Models.ApiDal.Instance.Get(PubConn, id);
                if (model == null)
                    return Json(new { code = -1, msg = "不存在编号为 [" + id + "] 的接口。" });
                return Json(new { code = 1, response = model });

            }
        }

        public JsonResult deleteapi(int id)
        {
            using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
            {
                PubConn.Open();
                bool r = Models.ApiDal.Instance.Delete(PubConn, id);
                Models.DbModels.certcenterlog log = new Models.DbModels.certcenterlog(this);
                log.opecontent = "删除接口,操作结果：" + r;
                Models.CertCenterLogDal.Instance.Add(PubConn, log);
                if (r)
                    return Json(new { code = 1 });
                return Json(new { code = -1, msg = "不存在编号为 [" + id + "] 的接口或已被删除。" });

            }
        }

    }
}
