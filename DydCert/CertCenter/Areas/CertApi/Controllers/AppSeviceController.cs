using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CertCenter.Areas.CertApi.Controllers
{
    public class AppSeviceController : Controller
    {
        //
        // GET: /CertApi/AppService/

        public ActionResult GetAppSecret()
        {
            return Models.ApiControllerHelper.Visit(() =>
            {
                return Models.AppSeviceDal.Instance.GetAppSecret(this);
            }, this);
        }

    }
}
