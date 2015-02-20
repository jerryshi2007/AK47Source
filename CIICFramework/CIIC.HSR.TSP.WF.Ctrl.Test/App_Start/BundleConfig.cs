using System.Web;
using System.Web.Optimization;

namespace CIIC.HSR.TSP.WF.Ctrl.Test
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",//XXXX
                      "~/Scripts/respond.js"//XXXX
                      ));


            bundles.Add(new ScriptBundle("~/Scripts/kendo").Include(
                 "~/Scripts/kendo.web.min.js",//YYY
                 "~/Scripts/kendo.aspnetmvc.min.js",
                 "~/Scripts/kendo.zh-CN.js"
                ));



            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",//XXXX
                      "~/Content/kendo/kendo.common.min.css",//YYY
                       "~/Content/j_style.css",
                      "~/Content/site.css"));
                   

            bundles.Add(new StyleBundle("~/Content/themes/NewStyle1/css").Include(
          "~/Content/themes/NewStyle1/ciic.ssp.home.css",
          "~/Content/themes/NewStyle1/ciic.ssp.web.css"
          
          ));

            bundles.Add(new StyleBundle("~/Content/kendo/NewStyle1").Include(
          "~/Content/kendo/kendo.bootstrap.min.css"//YYY
          
          )); 

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = false;
        }
    }
}
