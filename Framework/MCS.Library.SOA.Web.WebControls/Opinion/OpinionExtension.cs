using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library.MVC;
using MCS.Library.Principal;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// Generic Opinion的扩展类
	/// </summary>
	public static class OpinionExtension
	{
		public static void FillPersonInfo(this GenericOpinion opinion)
		{
			if (HttpContext.Current.Request.IsAuthenticated)
			{
				IWfActivity originalActivity = WfClientContext.Current.OriginalActivity;

				opinion.IssuePerson = WfClientContext.Current.User;

				if (originalActivity != null)
				{
					opinion.IssuePerson = originalActivity.Assignees.FindDelegator(opinion.IssuePerson);
				}

				opinion.AppendPerson = DeluxeIdentity.CurrentRealUser;
			}
		}
	}
}
