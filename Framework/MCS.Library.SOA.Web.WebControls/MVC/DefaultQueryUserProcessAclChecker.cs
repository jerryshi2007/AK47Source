using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Passport;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.MVC;

namespace MCS.Web.Library.MVC
{
	internal class DefaultQueryUserProcessAclChecker : IUserProcessAclChecker
	{
		public static readonly DefaultQueryUserProcessAclChecker Instance = new DefaultQueryUserProcessAclChecker();

		private DefaultQueryUserProcessAclChecker()
		{
		}

		public void CheckUserInAcl(IUser user, IWfProcess process, ref bool continueCheck)
		{
			user.NullCheck("user");

			continueCheck = (RolesDefineConfig.GetConfig().IsCurrentUserInRoles(user, "AdminFormQuery") == false);
		}
	}
}
