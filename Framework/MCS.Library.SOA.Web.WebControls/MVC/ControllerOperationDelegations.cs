using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Web.Library.MVC
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="process"></param>
	public delegate void ProcessReadyHandler(IWfProcess process);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="process"></param>
	/// <returns></returns>
	public delegate CommandStateBase PrepareCommandStateHandler(IWfProcess process);

	/// <summary>
	/// 当前用户是否在某个流程的Acl中
	/// </summary>
	/// <param name="process">流程</param>
	/// <param name="continueCheck">是否继续检查。检查通过，就不用继续了，如果不通过，要么抛出，要么交给后续操作</param>
	public delegate void IsCurrentUserInAclHandler(IWfProcess process, ref bool continueCheck);
}
