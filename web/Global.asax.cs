namespace Skeleton.Web {
    using App_Start;
    using System.Web;
    using System.Web.Http;
    using System.Web.Optimization;
    using System.Web.Routing;
    using System.Web.SessionState;

    public class WebApiApplication : System.Web.HttpApplication {

        protected void Application_Start() {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
        protected void Application_PostAuthorizeRequest() {
            if (HttpContext.Current.Request.Headers["X-Requested-With"] == "XMLHttpRequest") {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            }
        }
    }
}