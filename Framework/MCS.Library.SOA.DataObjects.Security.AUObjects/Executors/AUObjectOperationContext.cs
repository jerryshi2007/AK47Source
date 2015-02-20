using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Logs;


namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Executors
{
	public class AUObjectOperationContext : Dictionary<string, object>
	{
		private AUOperationType _OperationType = AUOperationType.None;
		private AUExecutorBase _Executor = null;
		private AUOperationLogCollection _Logs = null;

		/// <summary>
		/// 根据指定的操作类型和执行器初始化<see cref="SchemaObjectOperationContext"/>的新实例。
		/// </summary>
		/// <param name="opType"><see cref="SCOperationType"/>值之一，表示操作的类型</param>
		/// <param name="executor"><see cref="SCExecutorBase"/>对象，表示操作</param>
		public AUObjectOperationContext(AUOperationType opType, AUExecutorBase executor)
		{
			this._OperationType = opType;
			this._Executor = executor;
		}

		/// <summary>
		/// 获取操作日志的集合
		/// </summary>
		public AUOperationLogCollection Logs
		{
			get
			{
				if (this._Logs == null)
					this._Logs = new AUOperationLogCollection();

				return this._Logs;
			}
		}

		/// <summary>
		/// 获取执行器
		/// </summary>
		public AUExecutorBase Executor
		{
			get
			{
				return this._Executor;
			}
		}

		/// <summary>
		/// 获取表示操作类型的<see cref="SCOperationType"/>值之一。
		/// </summary>
		public AUOperationType OperationType
		{
			get
			{
				return this._OperationType;
			}
		}
	}
}
