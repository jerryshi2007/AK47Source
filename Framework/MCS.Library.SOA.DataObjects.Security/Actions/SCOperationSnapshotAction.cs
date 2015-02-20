using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace MCS.Library.SOA.DataObjects.Security.Actions
{
	public class SCOperationSnapshotAction : ISCObjectOperationAction
	{
		#region ISCObjectOperationAction Members

		public void BeforeExecute(SCOperationType operationType)
		{
		}

		public void AfterExecute(SCOperationType operationType)
		{
			SCOperationSnapshot snapshot = new SCOperationSnapshot() { OperationType = operationType };

			SCOperationSnapshotAdapter.Instance.Update(snapshot);
		}

		#endregion
	}
}
