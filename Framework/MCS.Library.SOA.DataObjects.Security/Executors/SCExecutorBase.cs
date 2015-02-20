using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Transactions;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Logs;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Executors
{
	public abstract class SCExecutorBase
	{
		private bool _AutoStartTransaction = true;

		protected SCExecutorBase(SCOperationType opType)
		{
			this.OperationType = opType;
		}

		public SCOperationType OperationType
		{
			get;
			private set;
		}

		public object Execute()
		{
			object result = null;

			ExecutionWrapper(EnumItemDescriptionAttribute.GetDescription(OperationType),
					() => result = InternalExecute());

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

		protected virtual void PrepareData(SchemaObjectOperationContext context)
		{
		}

		protected virtual void PrepareOperationLog(SchemaObjectOperationContext context)
		{
		}

		protected void CheckObjectStatus(params SchemaObjectBase[] objsToCheck)
		{
			List<SchemaObjectBase> normalizedObjsToCheck = new List<SchemaObjectBase>();

			foreach (SchemaObjectBase obj in objsToCheck)
			{
				if (obj.ID != SCOrganization.RootOrganizationID)
					normalizedObjsToCheck.Add(obj);
			}

			InSqlClauseBuilder idBuilder = new InSqlClauseBuilder("ID");

			normalizedObjsToCheck.ForEach(o => idBuilder.AppendItem(o.ID));

			if (idBuilder.IsEmpty == false)
			{
				SchemaObjectCollection originalDataList = SchemaObjectAdapter.Instance.Load(idBuilder);

				string opName = EnumItemDescriptionAttribute.GetDescription(this.OperationType);

				foreach (SchemaObjectBase objToCheck in normalizedObjsToCheck)
				{
					if (originalDataList.ContainsKey(objToCheck.ID) == false)
						throw new SCStatusCheckException(string.Format("ID为\"{0}\"的对象不存在，不能执行{1}操作", objToCheck.ID, opName));

					SchemaObjectBase originalData = originalDataList[objToCheck.ID];

					if (originalData.Status != SchemaObjectStatus.Normal)
						throw new SCStatusCheckException(originalData, this.OperationType);
				}
			}
		}

		protected abstract object DoOperation(SchemaObjectOperationContext context);

		private object InternalExecute()
		{
			SchemaObjectOperationContext context = new SchemaObjectOperationContext(this.OperationType, this);

			ExecutionWrapper("PrepareData", () => PrepareData(context));
			ExecutionWrapper("PrepareOperationLog", () => PrepareOperationLog(context));

			object result = null;

			if (this.AutoStartTransaction)
			{
				using (TransactionScope scope = TransactionScopeFactory.Create())
				{
					ExecutionWrapper("DoOperation", () => result = DoOperation(context));
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

		private void PersistOperationLog(SchemaObjectOperationContext context)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				context.Logs.ForEach(log => SCOperationLogAdapter.Instance.Insert(log));

				scope.Complete();
			}
		}

		private static void ExecutionWrapper(string operationName, Action action)
		{
			operationName.CheckStringIsNullOrEmpty("operationName");
			action.NullCheck("action");

			SCExecutorLogContextInfo.Writer.WriteLine("\t\t{0}开始：{1:yyyy-MM-dd HH:mm:ss.fff}",
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
				SCExecutorLogContextInfo.Writer.WriteLine("\t\t{0}结束：{1:yyyy-MM-dd HH:mm:ss.fff}；经过时间：{2:#,##0}毫秒",
					operationName, DateTime.Now, sw.ElapsedMilliseconds);
			}
		}
	}
}
