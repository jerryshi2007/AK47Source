using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// Acl的更新接口
	/// </summary>
	public interface IAclUpdater
	{
		void Update(WfAclItemCollection aclItems);
	}
}
