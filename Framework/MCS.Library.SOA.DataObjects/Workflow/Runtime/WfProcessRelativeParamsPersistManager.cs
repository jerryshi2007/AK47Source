using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 保存流程相关参数的持久化器
	/// </summary>
	internal class WfProcessRelativeParamsPersistManager : IWfExtraProcessPersistManager
	{
		#region IWfExtraProcessPersistManager Members

		public void SaveData(IWfProcess process, Dictionary<object, object> context)
		{
			WfProcessRelativeParamsAdapter.Instance.Update(process);
		}

		public void DeleteData(WfProcessCurrentInfoCollection processesInfo, Dictionary<object, object> context)
		{
			WfRelativeProcessAdapter.Instance.Delete(processesInfo);
		}

		#endregion
	}
}
