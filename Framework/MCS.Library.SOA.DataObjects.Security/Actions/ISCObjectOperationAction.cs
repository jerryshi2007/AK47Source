using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Security.Actions
{
	public interface ISCObjectOperationAction
	{
		/// <summary>
		/// 执行之前
		/// </summary>
		/// <param name="operationType"></param>
		void BeforeExecute(SCOperationType operationType);
		
		/// <summary>
		/// 执行之后
		/// </summary>
		/// <param name="operationType"></param>
		void AfterExecute(SCOperationType operationType);
	}

	[Serializable]
	public class SCObjectOperationActionCollection : EditableDataObjectCollectionBase<ISCObjectOperationAction>
	{
		/// <summary>
		/// 执行之前
		/// </summary>
		/// <param name="obj">一个<see cref="SchemaObjectBase"/>实例</param>
		public void BeforeExecute(SCOperationType operationType)
		{
			this.ForEach(action => action.BeforeExecute(operationType));
		}

		/// <summary>
		/// 执行之后
		/// </summary>
		/// <param name="operationType"></param>
		public void AfterExecute(SCOperationType operationType)
		{
			this.ForEach(action => action.AfterExecute(operationType));
		}
	}
}
