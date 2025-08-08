using System.Web.Mvc;

namespace NguyenTrungTuan_2122110251.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "NguyenTrungTuan_2122110251.Areas.Admin.Controllers" }
            );
        }
    }
}