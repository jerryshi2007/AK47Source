using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Web.Library.MVC
{
	/// <summary>
	/// 用户Acl的检查器
	/// </summary>
	public interface IUserProcessAclChecker
	{
		/// <summary>
		/// 检查用户是否在流程相关的Acl中
		/// </summary>
		/// <param name="user"></param>
		/// <param name="process"></param>
		/// <param name="continueCheck">是否继续检查。例如，如果是管理员，就不需要后续检查了</param>
		void CheckUserInAcl(IUser user, IWfProcess process, ref bool continueCheck);
	}
}
