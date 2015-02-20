using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow.Runtime
{
	/// <summary>
	/// 保存流程维度信息的持久化器
	/// </summary>
	internal class WfProcessDimensionPersistManager : IWfExtraProcessPersistManager
	{
		#region IWfExtraProcessPersistManager Members

		public void SaveData(IWfProcess process, Dictionary<object, object> context)
		{
			WfProcessDimension pd = WfProcessDimension.FromProcess(process);

			WfProcessDimensionAdapter.Instance.Update(pd);
		}

		public void DeleteData(WfProcessCurrentInfoCollection processesInfo, Dictionary<object, object> context)
		{
			WfProcessDimensionAdapter.Instance.Delete(processesInfo);
		}

		#endregion
	}
}
