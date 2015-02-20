using System;
using System.Web;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Security.Principal;
using MCS.Library.Core;
using MCS.Library.Caching;
using MCS.Library.Principal;
using MCS.Library.OGUPermission;

namespace MCS.Library.Workflow.Engine
{
	public static class WfPrincipalHelper
	{
		private static DeluxePrincipal CurrentPrincipal
		{
			get
			{
				object objPrincipal;
				DeluxePrincipal principal = null;

				if (ObjectContextCache.Instance.TryGetValue("WfPrincipal", out objPrincipal) == false)
				{
					IPrincipal contextPrincipal = GetCurrentPrincipal();

					if (contextPrincipal != null)
					{
						if (contextPrincipal is DeluxePrincipal)
							principal = (DeluxePrincipal)contextPrincipal;
						else
						{
							DeluxeIdentity identity =
								new DeluxeIdentity(contextPrincipal.Identity.Name);

							principal = new DeluxePrincipal(identity);

							ObjectContextCache.Instance.Add("WfPrincipal", principal);
						}
					}
				}
				else
					principal = (DeluxePrincipal)objPrincipal;

				//如果Principal为null，应该抛出异常
				return principal;
			}
		}

		public static IUser CurrentUser
		{
			get
			{
				IUser user = null;

				DeluxePrincipal principal = CurrentPrincipal;

				if (principal != null)
				{
					user = ((DeluxeIdentity)principal.Identity).User;
				}

				return user;
			}
		}

		private static IPrincipal GetCurrentPrincipal()
		{
			IPrincipal principal = null;

			if (EnvironmentHelper.Mode == InstanceMode.Web)
				principal = HttpContext.Current.User;
			else
				principal = Thread.CurrentPrincipal;

			return principal;
		}
	}
}
