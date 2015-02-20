using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow.Runtime
{
	/// <summary>
	/// 保存流程中，当前活动的当前人的持久化器
	/// </summary>
	internal class WfCurrentAssigneesPersistManager : IWfExtraProcessPersistManager
	{
		#region IWfExtraProcessPersistManager Members

		public void SaveData(IWfProcess process, Dictionary<object, object> context)
		{
			WfProcessCurrentInfoAdapter.Instance.UpdateProcessRelatedUsers(process);
		}

		public void DeleteData(WfProcessCurrentInfoCollection processesInfo, Dictionary<object, object> context)
		{
			WfProcessCurrentInfoAdapter.Instance.DeleteProcessCurrentAssignees(processesInfo);
		}

		#endregion
	}
}
