using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Executors;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Actions
{
	public interface IAUObjectOperationAction
	{
		void AfterExecute(AUOperationType operationType);
		void BeforeExecute(AUOperationType operationType);
	}
}
