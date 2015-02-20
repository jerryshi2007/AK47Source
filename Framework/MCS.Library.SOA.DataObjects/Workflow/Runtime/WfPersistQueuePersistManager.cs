using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow.Runtime
{
	/// <summary>
	/// 保存流程需要变更的队列信息
	/// </summary>
	internal class WfPersistQueuePersistManager : IWfExtraProcessPersistManager
	{
		#region IWfExtraProcessPersistManager Members

		public void SaveData(IWfProcess process, Dictionary<object, object> context)
		{
			WfPersistQueueAdapter.Instance.UpdatePersistQueue(process);
		}

		public void DeleteData(WfProcessCurrentInfoCollection processesInfo, Dictionary<object, object> context)
		{
			WfPersistQueueAdapter.Instance.DeletePersistQueue(processesInfo);
		}

		#endregion
	}
}
