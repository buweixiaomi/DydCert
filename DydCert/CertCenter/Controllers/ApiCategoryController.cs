using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CertCenter.Controllers
{
    [Authorize]
    public class ApiCategoryController : Controller
    {
        //
        // GET: /ApiCategory/

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetApiCategoryData(string id, string lv, string n)
        {
            using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
            {
                List<Models.Z_TreeModel> ztree = new List<Models.Z_TreeModel>();
                PubConn.Open();

                for (int i = 0; i <= 2; i++)
                {
                    Models.Z_TreeModel tempnode = new Models.Z_TreeModel();
                    tempnode.name = Models.CertCenterComm.APPTYPENAME[i];
                    tempnode.iconSkin = "";
                    tempnode.id = i.ToString();
                    tempnode.open = "true";
                    List<Models.DbModels.appcategory> tempgrade = Models.AppcategoryDal.Instance.GetAppTypeCategorys(PubConn, i);
                    tempnode.children = new List<Models.Z_TreeModel>();
                    foreach (var a in tempgrade)
                    {
                        tempnode.children.Add(new Models.Z_TreeModel() { id = i + "_" + a.categoryid.ToString(), name = a.categorytitle });
                    }
                    tempnode.isParent = tempnode.children.Count == 0 ? "false" : "true";
                    ztree.Add(tempnode);
                }
                //Models.DbModels.certcenterlog log = new Models.DbModels.certcenterlog(this);
                //log.opecontent = "得到分类数据据";
                //Models.CertCenterLogDal.Instance.Add(PubConn, log);
                return Json(ztree);
            }
        }

        public JsonResult GetApiCategoryInfo(int apptype, int categoryid)
        {
            using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
            {
                PubConn.Open();

                Models.DbModels.appcategory model = Models.AppcategoryDal.Instance.GetCategoryInfo(PubConn, apptype, categoryid);

                if (model == null)
                {
                    return Json(new CertComm.ServerResult() { code = -1, msg = "无查项" });
                }
                return Json(new CertComm.ServerResult() { code = 1, msg = "", response = model });


            }
        }

        public JsonResult Edit(Models.DbModels.appcategory model)
        {
            if (string.IsNullOrEmpty(model.categorytitle))
            {
                return Json(new { code = -1, msg = "分类名称不能为空" });
            }
            using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
            {
                PubConn.Open();
                try
                {
                    if (model.categoryid < 1)//添加
                    {
                        int r = Models.AppcategoryDal.Instance.Addcategory(PubConn, model);
                        if (r > 0)
                        {
                            return Json(new { code = 1, response = model });
                        }
                        else
                        {
                            return Json(new { code = -1, msg = "新增失败，原因不明" });
                        }
                    }
                    else//修改
                    {
                        int r = Models.AppcategoryDal.Instance.UpdateCategory(PubConn, model);
                        if (r > 0)
                        {
                            return Json(new { code = 1, response = model });
                        }
                        else
                        {
                            return Json(new { code = -1, msg = "修改失败，原因不明" });
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { code = -1, msg = ex.Message });
                }
                finally
                {

                    Models.DbModels.certcenterlog log = new Models.DbModels.certcenterlog(this);
                    log.opecontent = "修改应用分类";
                    Models.CertCenterLogDal.Instance.Add(PubConn, log);
                }
            }

        }

        public JsonResult Delete(int apptype, int categoryid)
        {
            using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
            {
                PubConn.Open();

                int r = Models.AppcategoryDal.Instance.DeleteCategory(PubConn, apptype, categoryid);
                    Models.DbModels.certcenterlog log = new Models.DbModels.certcenterlog(this);
                    log.opecontent = "删除应用分类,操作结果：" + r;
                    Models.CertCenterLogDal.Instance.Add(PubConn, log);
                if (r > 0)
                {
                    return Json(new { code = 1 });
                }
                else
                {



                    if (r == -2)
                    {
                        return Json(new { code = -2, msg = "此分类下存在接口，无法删除。" });
                    }
                    else
                    {
                        return Json(new { code = -1, msg = "删除失败，不存在或已被删除" });
                    }
                }
            }

        }

        public JsonResult UseApptypeGetcategories(int apptype)
        {
            using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
            {
                PubConn.Open();
                List<Models.DbModels.appcategory> grades = Models.AppcategoryDal.Instance.GetAppTypeCategorys(PubConn, apptype);
                return Json(new { code = 1, response = grades });
            }
        }

    }
}
