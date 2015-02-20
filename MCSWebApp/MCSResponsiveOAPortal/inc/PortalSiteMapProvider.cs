using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MCSResponsiveOAPortal
{
    public class PortalSiteMapProvider : XmlSiteMapProvider
    {
        public override bool IsAccessibleToUser(System.Web.HttpContext context, System.Web.SiteMapNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (this.SecurityTrimmingEnabled)
            {
                bool enabled = false;
                string roleString = node["mcsroles"];
                if (string.IsNullOrEmpty(roleString) == false && ((roleString == "*") || ((context.User != null) && context.User.IsInRole(roleString))))
                {
                    enabled = true;
                    //VirtualPath virtualPath = node.VirtualPath;
                    //return (((virtualPath != null) && virtualPath.IsWithinAppRoot) && Util.IsUserAllowedToPath(context, virtualPath));
                }

                return enabled;
            }
            else
            {
                return true;
            }
        }
    }
}