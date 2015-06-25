using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CertCenter.Controllers
{
    [Authorize]
    public class AppgradeController : Controller
    {
        //
        // GET: /Appgrade/

        public ActionResult Index()
        {
            return View();
        }


        public JsonResult GetAppGradeData(string id, string lv, string n)
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
                    tempnode.open = "true";
                    tempnode.id = i.ToString();
                    List<Models.DbModels.appgrade> tempgrade = Models.AppGradeDal.Instance.GetGrades(PubConn, i);
                    tempnode.children = new List<Models.Z_TreeModel>();
                    foreach (var a in tempgrade)
                    {
                        tempnode.children.Add(new Models.Z_TreeModel() { id = i + "_" + a.appgradeno.ToString(), name = a.appgradename });
                    }
                    tempnode.isParent = tempnode.children.Count == 0 ? "false" : "true";
                    ztree.Add(tempnode);
                }
                return Json(ztree);
            }
        }


        public JsonResult GetAppGradeInfo(int apptype, int appgradeno)
        {
            using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
            {
                PubConn.Open();
                Models.DbModels.appgrade model = Models.AppGradeDal.Instance.GetGradeInfo(PubConn, apptype, appgradeno);
                if (model == null)
                {
                    return Json(new { code = -1, msg = "不存在此等级信息。" });
                }
                return Json(new { code = 1, response = model });
            }
        }

        /// <summary>
        /// 添加或修改 如果appgradeno >0 ，则为修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult Edit(Models.DbModels.appgrade model)
        {
            if (string.IsNullOrEmpty(model.appgradename))
            {
                return Json(new { code = -1, msg = "分类名称不能为空" });
            }
            using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
            {
                PubConn.Open();
                try
                {
                    if (model.appgradeno < 1)//添加
                    {
                        int r = Models.AppGradeDal.Instance.AddGrade(PubConn, model);
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
                        int r = Models.AppGradeDal.Instance.UpdateGrade(PubConn, model);
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
                    log.opecontent = "修改应用等级。";
                    Models.CertCenterLogDal.Instance.Add(PubConn, log);
                }
            }
        }

        public JsonResult Delete(int apptype, int appgradeno)
        {
            using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
            {
                PubConn.Open();

                int r = Models.AppGradeDal.Instance.DeleteGrade(PubConn, apptype, appgradeno);
                Models.DbModels.certcenterlog log = new Models.DbModels.certcenterlog(this);
                log.opecontent = "删除应用等级。" + r;
                Models.CertCenterLogDal.Instance.Add(PubConn, log);
                if (r > 0)
                {
                    return Json(new { code = 1 });
                }
                else
                {
                    if (r == -2)
                    {
                        return Json(new { code = -2, msg = "此级别下存在接口，无法删除。" });
                    }
                    else
                    {
                        return Json(new { code = -1, msg = "删除失败，不存在或已被删除" });
                    }
                }
            }

        }

        public JsonResult UseApptypeGetGrades(int apptype)
        {
            using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
            {
                PubConn.Open();
                List<Models.DbModels.appgrade> grades = Models.AppGradeDal.Instance.GetGrades(PubConn, apptype);
                return Json(new { code = 1, response = grades });
            }
        }
    }
}
