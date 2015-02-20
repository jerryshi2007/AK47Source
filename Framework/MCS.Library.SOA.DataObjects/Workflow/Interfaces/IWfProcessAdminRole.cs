using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 判断流程管理员的接口
	/// </summary>
	public interface IWfProcessAdminRole
	{
		/// <summary>
		/// 是否是流程的管理员
		/// </summary>
		/// <param name="process"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		bool IsInAdminRole(IWfProcess process, IUser user);
	}
}
