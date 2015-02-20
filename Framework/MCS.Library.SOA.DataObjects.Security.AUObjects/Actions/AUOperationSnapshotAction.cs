using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Adapters;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Actions
{
	/// <summary>
	/// 表示更新AU操作快照的动作
	/// </summary>
	public class AUOperationSnapshotAction : IAUObjectOperationAction
	{
		public void AfterExecute(Executors.AUOperationType operationType)
		{
		}

		public void BeforeExecute(Executors.AUOperationType operationType)
		{
			AUOperationSnapshot snapshot = new AUOperationSnapshot() { OperationType = operationType };

			AUOperationSnapshotAdapter.Instance.Update(snapshot);
		}
	}
}
