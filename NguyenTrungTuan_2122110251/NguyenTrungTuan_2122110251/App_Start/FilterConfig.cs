using System.Web;
using System.Web.Mvc;

namespace NguyenTrungTuan_2122110251
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
