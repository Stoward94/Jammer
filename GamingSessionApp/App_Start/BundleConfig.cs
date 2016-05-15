using System.Web;
using System.Web.Optimization;

namespace GamingSessionApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery.unobtrusive-ajax.js",
                        "~/Scripts/jquery-ui.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                    "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/jquery.caret.min.js",
                      "~/Scripts/jquery.tag-editor.min.js",
                      "~/Scripts/bootstrap-rating.min.js",
                      "~/Scripts/igdb.jquery.js",
                      "~/Scripts/moment.min.js",
                      "~/Scripts/bootstrap-datetimepicker.min.js",
                      "~/Scripts/site.js",
                      "~/Scripts/TriggerWars/authorised.js"));

             bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/jquery-ui.min.css",
                      "~/Content/jquery.tag-editor.css",
                      "~/Content/bootstrap-rating.css",
                      "~/Content/bootstrap-datetimepicker.min.css",
                      "~/Content/TriggerWars/site.css",
                      "~/Content/TriggerWars/feedback.css",
                      "~/Content/TriggerWars/modules.css",
                      "~/Content/TriggerWars/footer.css"));


            //BundleTable.EnableOptimizations = true;
        }
    }
}
