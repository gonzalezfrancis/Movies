using System.Web;
using System.Web.Optimization;

namespace Movies
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                        "~/Scripts/knockout-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/bootstrap-datepicker.js",
                      "~/Scripts/jquery-customselect.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-datepicker.css",
                      "~/Content/jquery-customselect.css",
                      "~/Content/site.css"));
            //Jquery UI
            bundles.Add(new StyleBundle("~/Content/jquery-ui").Include(
                        "~/Content/themes/base/jquery-ui.css"));
            bundles.Add(new ScriptBundle("~/bundles/jquery-UI").Include(
                        "~/Scripts/jquery-ui-{version}.js"));
            //FancyBox
            bundles.Add(new StyleBundle("~/Content/fancyBox").Include(
                    "~/Content/jquery.fancybox.css",
                    "~/Content/jquery.fancybox-thumbs.css",
                    "~/Content/jquery.fancybox-buttons.css"));

            bundles.Add(new ScriptBundle("~/bundles/fancybox").Include(
                    "~/Scripts/jquery.fancybox.js",
                    "~/Scripts/jquery.fancybox-media.js",
                    "~/Scripts/jquery.fancybox-buttons.js",
                    "~/Scripts/jquery.fancybox-thumbs.js",
                    "~/Scripts/jquery.fancybox.pack.js"));

            //Movies Css
            bundles.Add(new StyleBundle("~/Content/moviesIndex").Include(
                      "~/Content/moviesIndex.css"));
        }
    }
}
