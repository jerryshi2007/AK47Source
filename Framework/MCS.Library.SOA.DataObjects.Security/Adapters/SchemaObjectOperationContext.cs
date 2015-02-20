using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Logs;
using MCS.Library.SOA.DataObjects.Security.Executors;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
	/// <summary>
	/// Action中的操作上下文
	/// </summary>
	[Serializable]
	public class SchemaObjectOperationContext : Dictionary<string, object>
	{
		private SCOperationType _OperationType = SCOperationType.None;
		private SCExecutorBase _Executor = null;
		private SCOperationLogCollection _Logs = null;

		/// <summary>
		/// 根据指定的操作类型和执行器初始化<see cref="SchemaObjectOperationContext"/>的新实例。
		/// </summary>
		/// <param name="opType"><see cref="SCOperationType"/>值之一，表示操作的类型</param>
		/// <param name="executor"><see cref="SCExecutorBase"/>对象，表示操作</param>
		public SchemaObjectOperationContext(SCOperationType opType, SCExecutorBase executor)
		{
			this._OperationType = opType;
			this._Executor = executor;
		}

		/// <summary>
		/// 获取操作日志的集合
		/// </summary>
		public SCOperationLogCollection Logs
		{
			get
			{
				if (this._Logs == null)
					this._Logs = new SCOperationLogCollection();

				return this._Logs;
			}
		}

		/// <summary>
		/// 获取执行器
		/// </summary>
		public SCExecutorBase Executor
		{
			get
			{
				return this._Executor;
			}
		}

		/// <summary>
		/// 获取表示操作类型的<see cref="SCOperationType"/>值之一。
		/// </summary>
		public SCOperationType OperationType
		{
			get
			{
				return this._OperationType;
			}
		}
	}
}
