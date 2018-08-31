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

            bundles.Add(new ScriptBundle("~/bundles/bootstrapCss").Include(
                        "~/Content/bootstrap.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrapJs").Include(
                        "~/Scripts/umd/popper.js",
                        "~/Scripts/bootstrap.min.js"));

            bundles.Add(new StyleBundle("~/Content/Future/Css/Index").Include(
                        "~/Content/Fonts/font-awesome-4.7.0/css/font-awesome.min.css",
                        "~/Content/Future/Index/Main.css"));

            bundles.Add(new StyleBundle("~/Content/Future/Js/Index").Include(
                        "~/Scripts/Future/Index/Main.js"));
        }
    }
}