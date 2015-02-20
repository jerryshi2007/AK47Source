using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using System.Transactions;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Logs;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using System.Diagnostics;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Adapters;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Executors
{
	public abstract class AUExecutorBase
	{
		private bool _AutoStartTransaction = true;

		public AUExecutorBase(AUOperationType opType)
		{
			this.OperationType = opType;
		}

		public AUOperationType OperationType { get; private set; }

		[DebuggerNonUserCode]
		public object Execute()
		{
			object result = null;

			ExecutionWrapper(EnumItemDescriptionAttribute.GetDescription(OperationType), () => { result = InternalExecute(); });

			return result;
		}

		public bool AutoStartTransaction
		{
			get
			{
				return this._AutoStartTransaction;
			}

			protected set
			{
				this._AutoStartTransaction = value;
			}
		}

		protected virtual void PrepareData(AUObjectOperationContext context)
		{
		}

		protected virtual void PrepareOperationLog(AUObjectOperationContext context)
		{
		}

		[DebuggerStepperBoundary]
		protected void CheckObjectStatus(params SchemaObjectBase[] objsToCheck)
		{
			List<SchemaObjectBase> normalizedObjsToCheck = new List<SchemaObjectBase>();

			foreach (SchemaObjectBase obj in objsToCheck)
			{
				normalizedObjsToCheck.Add(obj);
			}

			InSqlClauseBuilder idBuilder = new InSqlClauseBuilder("ID");

			normalizedObjsToCheck.ForEach(o => idBuilder.AppendItem(o.ID));

			if (idBuilder.IsEmpty == false)
			{
				SchemaObjectCollection originalDataList = null;

				AUCommon.DoDbAction(() =>
				{
					originalDataList = SchemaObjectAdapter.Instance.Load(idBuilder);
				});

				string opName = EnumItemDescriptionAttribute.GetDescription(this.OperationType);

				foreach (SchemaObjectBase objToCheck in normalizedObjsToCheck)
				{
					if (originalDataList.ContainsKey(objToCheck.ID) == false)
						throw new AUStatusCheckException(string.Format("ID为\"{0}\"的对象不存在，不能执行{1}操作", objToCheck.ID, opName));

					SchemaObjectBase originalData = originalDataList[objToCheck.ID];

					if (originalData.Status != SchemaObjectStatus.Normal)
						throw new AUStatusCheckException(originalData, this.OperationType);
				}
			}
		}

		protected abstract object DoOperation(AUObjectOperationContext context);

		[DebuggerStepperBoundary]
		private object InternalExecute()
		{
			AUObjectOperationContext context = new AUObjectOperationContext(this.OperationType, this);

			ExecutionWrapper("PrepareData", () => PrepareData(context));
			ExecutionWrapper("PrepareOperationLog", () => PrepareOperationLog(context));

			object result = null;

			if (this.AutoStartTransaction)
			{
				using (TransactionScope scope = TransactionScopeFactory.Create())
				{
					ExecutionWrapper("DoOperation", () => { result = DoOperation(context); });
					ExecutionWrapper("PersistOperationLog", () => PersistOperationLog(context));

					scope.Complete();
				}
			}
			else
			{
				ExecutionWrapper("DoOperation", () => result = DoOperation(context));
				ExecutionWrapper("PersistOperationLog", () => PersistOperationLog(context));
			}


			return result;
		}

		[DebuggerStepperBoundary]
		private void PersistOperationLog(AUObjectOperationContext context)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				context.Logs.ForEach(log => AUOperationLogAdapter.Instance.Insert(log));

				scope.Complete();
			}
		}

		[DebuggerNonUserCode]
		private static void ExecutionWrapper(string operationName, Action action)
		{
			operationName.CheckStringIsNullOrEmpty("operationName");
			action.NullCheck("action");

			AUExecutorLogContextInfo.Writer.WriteLine("\t\t{0}开始：{1:yyyy-MM-dd HH:mm:ss.fff}",
					operationName, DateTime.Now);

			Stopwatch sw = new Stopwatch();

			sw.Start();
			try
			{
				action();
			}
			finally
			{
				sw.Stop();
				AUExecutorLogContextInfo.Writer.WriteLine("\t\t{0}结束：{1:yyyy-MM-dd HH:mm:ss.fff}；经过时间：{2:#,##0}毫秒",
					operationName, DateTime.Now, sw.ElapsedMilliseconds);
			}
		}
	}
}
