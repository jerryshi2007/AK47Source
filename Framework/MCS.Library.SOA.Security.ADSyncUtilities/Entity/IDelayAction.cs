using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.Security.ADSyncUtilities.Entity
{
	public interface IDelayAction
	{
		IOguObject OguObject { get; }

		/// <summary>
		/// Shen Review
		/// 不需要返回值
		/// </summary>
		/// <param stringValue="context"></param>
		/// <returns></returns>
		void DoAction(SynchronizeContext synchronizeContext);
	}
}
