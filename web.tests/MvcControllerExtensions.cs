using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Skeleton.Web.Tests
{
    public static class MvcControllerExtensions
    {
        public static IEnumerable<string> RouteAreaPrefixes<TController>(this TController controller) where TController : Controller
        {
            return typeof(TController)
                    .GetCustomAttributes(true)
                    .OfType<RoutePrefixAttribute>()
                    .Select(r => r.Prefix);
        }

        public static IEnumerable<string> RoutesOf<TController>(this TController controller, string methodName) where TController : Controller
        {
            return typeof(TController).GetMethod(methodName)
                    .GetCustomAttributes(true)
                    .OfType<RouteAttribute>()
                    .Select(r => r.Template);
        }

        public static bool IsSecure<TController>(this TController controller) where TController : Controller
        {
            return typeof(TController)
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Any();
        }

        public static bool IsCallSecure<TController>(this TController controller, string methodName) where TController : Controller
        {
            return typeof(TController).GetMethod(methodName)
                    .GetCustomAttributes(true)
                    .OfType<AuthorizeAttribute>()
                    .Any();
        }

        public static bool IsAnonymous<TController>(this TController controller) where TController : Controller
        {
            return typeof(TController)
                    .GetCustomAttributes(true)
                    .OfType<AllowAnonymousAttribute>()
                    .Any();
        }

        public static bool IsCallAnonymous<TController>(this TController controller, string methodName) where TController : Controller
        {
            return typeof(TController).GetMethod(methodName)
                    .GetCustomAttributes(true)
                    .OfType<AllowAnonymousAttribute>()
                    .Any();
        }
    }
}
