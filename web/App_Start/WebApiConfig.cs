using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Skeleton.Web.Telemetry;

namespace Skeleton.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.Services.Add(typeof(IExceptionLogger), new AiExceptionLogger());
            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new {id = RouteParameter.Optional}
            );
        }
    }
}