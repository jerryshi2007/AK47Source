using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 委托信息的读取器接口
	/// </summary>
	public interface IWfDelegationReader
	{
		/// <summary>
		/// 得到某个用户在某个流程的委托待办信息
		/// </summary>
		/// <param name="user"></param>
		/// <param name="process"></param>
		/// <returns></returns>
		WfDelegationCollection GetUserActiveDelegations(IUser user, IWfProcess process);
	}
}
