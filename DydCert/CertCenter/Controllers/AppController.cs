using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CertCenter.Controllers
{
    [Authorize]
    public class AppController : Controller
    {
        //
        // GET: /App/

        public ActionResult Index(int pno = 1, string keywords = "")
        {
            using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
            {
                PubConn.Open();
                int pagesize = 10;
                int totalcount = 10;
                pno = pno < 1 ? 1 : pno;
                List<Models.DbModels.app> listapp = Models.AppDal.Instance.GetList(PubConn, pno, pagesize, keywords, out totalcount);
                Webdiyer.WebControls.Mvc.PagedList<Models.DbModels.app> pager = new Webdiyer.WebControls.Mvc.PagedList<Models.DbModels.app>(listapp, pno, pagesize, totalcount);
                if (Request.IsAjaxRequest())
                {
                    return PartialView("List", pager);
                }
                return View(pager);
            }
        }

        public ActionResult Edit(string id)
        {
            using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
            {
                PubConn.Open();
                if (string.IsNullOrEmpty(id))
                {
                    ViewBag.appgrades = Models.AppGradeDal.Instance.GetGrades(PubConn, 0);
                    return View();
                }
                else
                {
                    ViewBag.act = "edit";
                    Models.DbModels.app model = Models.AppDal.Instance.GetAppInfo(PubConn, id);
                    ViewBag.appgrades = Models.AppGradeDal.Instance.GetGrades(PubConn, model.apptype);
                    return View(model);
                }
            }
        }

        [HttpPost]
        public ActionResult Edit(Models.DbModels.app model, string act)
        {
            using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
            {
                PubConn.Open();
                ViewBag.appgrades = Models.AppGradeDal.Instance.GetGrades(PubConn, model.apptype);
                ViewBag.act = act;
                if (string.IsNullOrEmpty(model.appname))
                {
                    ModelState.AddModelError("appname", "应用名不能为空。");
                    return View(model);
                }
                if (model.appgradeno <= 0)
                {
                    ModelState.AddModelError("appgradeno", "请选择应用级别。");
                    return View(model);
                }

                try
                {
                    if (string.IsNullOrEmpty(model.appid))
                    {
                        model.appid = XXF.Db.LibString.MakeRandomNumber(16);
                    }
                    if (string.IsNullOrEmpty(model.appsecret))
                    {
                        model.appsecret = Guid.NewGuid().ToString().Replace("-", "");
                    }
                    if (act == "edit")
                    {
                        int r = Models.AppDal.Instance.UpdateApp(PubConn, model);
                        if (r > 0)
                        {
                            return RedirectToAction("index");
                        }
                        else
                        {
                            ViewBag.act = "edit";
                            return View(model);
                        }
                    }
                    else
                    {
                        int r = Models.AppDal.Instance.AddApp(PubConn, model);
                        if (r > 0)
                        {
                            return RedirectToAction("index");
                        }
                        else
                        {
                            if (r == -1)
                            {
                                ModelState.AddModelError("appid", "appid已存在，请更换");
                            }
                            return View(model);
                        }
                    }

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(model);
                }
                finally
                {
                    Models.DbModels.certcenterlog log = new Models.DbModels.certcenterlog(this);
                    log.opecontent = "修改应用";
                    Models.CertCenterLogDal.Instance.Add(PubConn, log);
                }
            }
        }

        public JsonResult DeleteApp(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { code = -1, msg = "appid为能为空。" });
            }
            using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
            {
                PubConn.Open();
                try
                {
                    int r = Models.AppDal.Instance.DeletApp(PubConn, id);
                    if (r == 0)
                    {
                        return Json(new { code = -1, msg = "appid [" + id + "]不存在或已被删除" });
                    }
                    return Json(new { code = 1 });
                }
                finally
                {
                    Models.DbModels.certcenterlog log = new Models.DbModels.certcenterlog(this);
                    log.opecontent = "删除应用。";
                    Models.CertCenterLogDal.Instance.Add(PubConn, log);
                }
            }


        }


    }
}
