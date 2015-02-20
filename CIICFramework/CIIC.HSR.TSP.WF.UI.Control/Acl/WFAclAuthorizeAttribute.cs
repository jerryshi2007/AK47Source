using CIIC.HSR.TSP.WF.UI.Control.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CIIC.HSR.TSP.WF.UI.Control.Acl
{
	public class WFAclAuthorizeAttribute : AuthorizeAttribute
	{
		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			bool result = false;

			WFUIRuntimeContext runtime = httpContext.Request.GetWFContext();

			if (runtime != null && runtime.Process != null && runtime.CurrentUser != null)
			{
				if (runtime.CurrentUser.ID == runtime.Process.AuthorizationInfo.UserID)
				{
					result = runtime.Process.AuthorizationInfo.IsInAcl ||
						runtime.Process.AuthorizationInfo.IsProcessAdmin ||
						runtime.Process.AuthorizationInfo.IsProcessViewer ||
						runtime.Process.AuthorizationInfo.InMoveToMode;
				}
			}

			return result;
		}
	}
}