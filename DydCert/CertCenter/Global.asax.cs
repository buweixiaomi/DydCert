using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace CertCenter
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

           // WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //清理过过期token的线程
         //   Thread t = new Thread(new ThreadStart(DeleteOldTokenThread));
         //   t.IsBackground = true;
         //   t.Start();
        }

        private void DeleteOldTokenThread()
        {
            Thread.Sleep(TimeSpan.FromMinutes(5));
            try
            {
                while (true)
                {
                    Models.TokenDal.DeleteExpiresToken();
                    Thread.Sleep(TimeSpan.FromHours(24));
                }
            }
            catch (Exception ex) { }
        }
    }
}