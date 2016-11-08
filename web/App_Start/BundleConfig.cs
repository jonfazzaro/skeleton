using System.Web.Optimization;

namespace Skeleton.Web.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            RegisterJavaScriptBundles(bundles);
            RegisterCssBundles(bundles);
        }

        private static void RegisterCssBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/css")
                .Include("~/bower_components/bootstrap/dist/css/bootstrap.min.css", new CssRewriteUrlTransform())
                .Include("~/bower_components/bootstrap-toggle/css/bootstrap-toggle.min.css")
                .Include("~/bower_components/toastr/toastr.min.css")
                .Include("~/app.css", new CssRewriteUrlTransform()));
        }

        private static void RegisterJavaScriptBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/js")
                .Include("~/bower_components/knockoutjs/dist/knockout.js")
                .Include("~/bower_components/jquery/dist/jquery.min.js")
                .Include("~/bower_components/toastr/toastr.min.js")
                .Include("~/bower_components/bootstrap/dist/js/bootstrap.min.js")
                .Include("~/bower_components/bootstrap-toggle/js/bootstrap-toggle.min.js")
                .Include("~/bower_components/linqjs/linq.min.js")
                .Include("~/bower_components/Sortable/Sortable.js")
                .Include("~/scripts/sortable.js")
                .Include("~/scripts/resources.js")
                .Include("~/scripts/api.js")
                .Include("~/scripts/map.js"));
        }
    }
}