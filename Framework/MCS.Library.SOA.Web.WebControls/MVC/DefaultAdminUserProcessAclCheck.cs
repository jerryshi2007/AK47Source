using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Passport;

namespace MCS.Web.Library.MVC
{
    /// <summary>
    /// 管理员的Acl检查
    /// </summary>
    internal class DefaultAdminUserProcessAclChecker : IUserProcessAclChecker
    {
        public static readonly DefaultAdminUserProcessAclChecker Instance = new DefaultAdminUserProcessAclChecker();

        private DefaultAdminUserProcessAclChecker()
        {
        }

        public void CheckUserInAcl(IUser user, IWfProcess process, ref bool continueCheck)
        {
            user.NullCheck("user");

            continueCheck = WfClientContext.IsProcessAdmin(user, process) == false;

            if (continueCheck)
                continueCheck = WfClientContext.IsProcessViewer(user, process) == false;
        }
    }
}
