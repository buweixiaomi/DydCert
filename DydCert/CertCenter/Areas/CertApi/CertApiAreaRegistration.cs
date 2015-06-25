using System.Web.Mvc;

namespace CertCenter.Areas.CertApi
{
    public class CertApiAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "CertApi";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "CertApi_default",
                "CertApi/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
