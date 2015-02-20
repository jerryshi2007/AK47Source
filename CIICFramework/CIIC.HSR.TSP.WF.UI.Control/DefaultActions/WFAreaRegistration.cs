using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CIIC.HSR.TSP.WF.UI.Control.DefaultActions
{
    public class WFAreaRegistration : AreaRegistration 
    {
        public override string AreaName
        {
            get { return "Workflow"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
               "Workflow_default",
                "Workflow/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
