using MCS.Library.Core;
using MCS.Library.Passport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace ResponsivePassportService
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            RouteTable.Routes.RouteExistingFiles = false;
            RouteTable.Routes.MapPageRoute(
                "RoutePrincipalTest",
                "TestPages/PrincipalTest/{TenantCode}",
                "~/TestPages/PrincipalTest.aspx",
                true,
                new RouteValueDictionary { { TenantExtensions.TenantCodeParamName, string.Empty } });
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}