using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Executors;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Actions
{
	[Serializable]
	public class AUObjectOperationActionCollection : EditableDataObjectCollectionBase<IAUObjectOperationAction>
	{
		/// <summary>
		/// 执行之前
		/// </summary>
		/// <param name="obj">一个<see cref="SchemaObjectBase"/>实例</param>
		public void BeforeExecute(AUOperationType operationType)
		{
			this.ForEach(action => action.BeforeExecute(operationType));
		}

		/// <summary>
		/// 执行之后
		/// </summary>
		/// <param name="operationType"></param>
		public void AfterExecute(AUOperationType operationType)
		{
			this.ForEach(action => action.AfterExecute(operationType));
		}
	}
}
