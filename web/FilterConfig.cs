using System.Web.Mvc;
using Skeleton.Web.Telemetry;

namespace Skeleton.Web{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AiHandleErrorAttribute());
        }
    }
}