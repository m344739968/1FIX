using System.Web;
using System.Web.Mvc;

namespace _1Fix.Shop.Wap
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}