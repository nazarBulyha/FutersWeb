using System.Web;
using System.Web.Optimization;

namespace FuturesWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                     "~/Content/bootstrap.min.css",
                     "~/Scripts/bootstrap.min.js",
                     "~/Scripts/popper.js"));

            bundles.Add(new StyleBundle("~/Content/Future/Index").Include(
                     "~/Content/Fonts/font-awesome-4.7.0/css/font-awesome.min.css",
                     "~/Content/Future/Index/Main.css",
                     "~/Scripts/Future/Index/Main.js",
                     "~/Images/Icons/favicon.ico"));
        }
    }
}