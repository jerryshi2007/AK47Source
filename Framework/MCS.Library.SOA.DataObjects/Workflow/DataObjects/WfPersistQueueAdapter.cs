using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Transactions;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfPersistQueueAdapter
	{
		public static readonly WfPersistQueueAdapter Instance = new WfPersistQueueAdapter();

		private WfPersistQueueAdapter()
		{
		}

		public WfPersistQueue Load(string processID)
		{
			ORMappingItemCollection mappingInfo = ORMapping.GetMappingInfo(typeof(WfPersistQueue));

			processID.CheckStringIsNullOrEmpty("processID");

			string sql = string.Format("SELECT * FROM {0} WHERE PROCESS_ID = {1}",
				mappingInfo.TableName,
				TSqlBuilder.Instance.CheckUnicodeQuotationMark(processID)
			);

			DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

			WfPersistQueue result = null;

			if (table.Rows.Count > 0)
			{
				result = new WfPersistQueue();

				ORMapping.DataRowToObject(table.Rows[0], result);
			}

			return result;
		}

		public WfPersistQueue LoadArchived(string processID)
		{
			ORMappingItemCollection mappingInfo = ORMapping.GetMappingInfo(typeof(WfPersistQueue));

			processID.CheckStringIsNullOrEmpty("processID");

			string sql = string.Format("SELECT * FROM {0} WHERE PROCESS_ID = {1}",
				"WF.PERSIST_QUEUE_ARCHIEVED",
				TSqlBuilder.Instance.CheckUnicodeQuotationMark(processID)
			);

			DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

			WfPersistQueue result = null;

			if (table.Rows.Count > 0)
			{
				result = new WfPersistQueue();

				ORMapping.DataRowToObject(table.Rows[0], result);
			}

			return result;
		}

		public void UpdatePersistQueue(IWfProcess process)
		{
			process.NullCheck("process");

			WfPersistQueue pq = WfPersistQueue.FromProcess(process);

			string sql = GetUpdatePersistQueueSql(pq);

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				DbHelper.RunSql(sql, GetConnectionName());

				scope.Complete();
			}
		}

		public void DeletePersistQueue(WfProcessCurrentInfoCollection processesInfo)
		{
			processesInfo.NullCheck("processesInfo");

			InSqlClauseBuilder builder = new InSqlClauseBuilder("PROCESS_ID");

			processesInfo.ForEach(pi => builder.AppendItem(pi.InstanceID));

			if (builder.IsEmpty == false)
			{
				ORMappingItemCollection mappingInfo = ORMapping.GetMappingInfo(typeof(WfPersistQueue));

				string sql = string.Format("DELETE {0} WHERE {0}",
					mappingInfo.TableName, builder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

				DbHelper.RunSql(sql, GetConnectionName());
			}
		}

		/// <summary>
		/// 执行一项队列操作
		/// </summary>
		/// <param name="pq"></param>
		public void DoQueueOperation(WfPersistQueue pq)
		{
			pq.NullCheck("pq");

			Dictionary<object, object> context = new Dictionary<object, object>();

			IWfProcess process = WfRuntime.GetProcessByProcessID(pq.ProcessID);
			try
			{
				WfQueuePersistenceSettings.GetConfig().GetPersisters().SaveData(process, context);
			}
			finally
			{
				WfRuntime.ClearCache();
			}
		}

		public void MoveQueueItemToArchived(WfPersistQueue pq)
		{
			pq.NullCheck("pq");

			ORMappingItemCollection mappingInfo = ORMapping.GetMappingInfo(typeof(WfPersistQueue));

			InsertSqlClauseBuilder builder = ORMapping.GetInsertSqlClauseBuilder(pq, mappingInfo);

			builder.AppendItem("SORT_ID", pq.SortID);
			builder.AppendItem("PROCESS_TIME", "GETDATE()", "=", true);

			StringBuilder sql = new StringBuilder();

			sql.AppendFormat("DELETE {0} WHERE SORT_ID = {1}", "WF.PERSIST_QUEUE_ARCHIEVED", pq.SortID);
			sql.Append(TSqlBuilder.Instance.DBStatementSeperator);
			sql.AppendFormat("INSERT INTO {0}{1}", "WF.PERSIST_QUEUE_ARCHIEVED", builder.ToSqlString(TSqlBuilder.Instance));
			sql.Append(TSqlBuilder.Instance.DBStatementSeperator);
			sql.AppendFormat("DELETE {0} WHERE PROCESS_ID = {1}", mappingInfo.TableName, TSqlBuilder.Instance.CheckUnicodeQuotationMark(pq.ProcessID));

			DbHelper.RunSqlWithTransaction(sql.ToString(), GetConnectionName());
		}

		/// <summary>
		/// 执行队列项的操作，并且移动到已完成中
		/// </summary>
		/// <param name="pq"></param>
		public void DoQueueOperationAndMove(WfPersistQueue pq)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				try
				{
					DoQueueOperation(pq);
				}
				catch (System.Exception ex)
				{
					if (ex is TransactionException || ex is DbException)
						throw;

					pq.StatusText = ex.ToString();
				}

				MoveQueueItemToArchived(pq);

				scope.Complete();
			}
		}

		/// <summary>
		/// 从队列中获取n条记录，并进行处理
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		public WfPersistQueueCollection FetchQueueItemsAndDoOperation(int count)
		{
			ORMappingItemCollection mappingInfo = ORMapping.GetMappingInfo(typeof(WfPersistQueue));

			string top = count < 0 ? string.Empty : "TOP " + count;

			string sql = string.Format("SELECT {0} * FROM {1} WITH (UPDLOCK READPAST) ORDER BY SORT_ID", top, mappingInfo.TableName);

			WfPersistQueueCollection result = new WfPersistQueueCollection();

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				DataView view = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0].DefaultView;

				ORMapping.DataViewToCollection(result, view);

				foreach (WfPersistQueue pq in result)
					DoQueueOperationAndMove(pq);

				scope.Complete();
			}

			return result;
		}

		public void ClearQueue()
		{
			string sql = "TRUNCATE TABLE WF.PERSIST_QUEUE";

			DbHelper.RunSql(sql, GetConnectionName());
		}

		public void ClearArchivedQueue()
		{
			string sql = "TRUNCATE TABLE WF.PERSIST_QUEUE_ARCHIEVED";

			DbHelper.RunSql(sql, GetConnectionName());
		}

		protected virtual string GetConnectionName()
		{
			return WfRuntime.ProcessContext.SimulationContext.GetConnectionName(WorkflowSettings.GetConfig().ConnectionName);
		}

		private static string GetUpdatePersistQueueSql(WfPersistQueue pq)
		{
			StringBuilder sql = new StringBuilder();

			//仅仅插入数据
			//sql.Append(GetUpdateSql(pq));
			//sql.Append(TSqlBuilder.Instance.DBStatementSeperator);
			//sql.Append("IF @@ROWCOUNT = 0");
			//sql.Append(TSqlBuilder.Instance.DBStatementBegin);
			sql.Append(GetInsertSql(pq));
			//sql.Append(TSqlBuilder.Instance.DBStatementEnd);

			return sql.ToString();
		}

		private static string GetUpdateSql(WfPersistQueue pq)
		{
			ORMappingItemCollection mappingInfo = ORMapping.GetMappingInfo(pq.GetType());

			UpdateSqlClauseBuilder builder = ORMapping.GetUpdateSqlClauseBuilder(pq, mappingInfo, "UpdateTag");

			builder.AppendItem("UPDATE_TAG", "WF.PROCESS_INSTANCES.UPDATE_TAG", "=", true);

			string sql = string.Format("UPDATE {0} SET {1} FROM {0} INNER JOIN WF.PROCESS_INSTANCES ON INSTANCE_ID = PROCESS_ID WHERE PROCESS_ID = {2}",
				mappingInfo.TableName,
				builder.ToSqlString(TSqlBuilder.Instance),
				TSqlBuilder.Instance.CheckUnicodeQuotationMark(pq.ProcessID));

			return sql;
		}

		private static string GetInsertSql(WfPersistQueue pq)
		{
			ORMappingItemCollection mappingInfo = ORMapping.GetMappingInfo(pq.GetType());

			string sql = string.Format("INSERT INTO {0}(PROCESS_ID, UPDATE_TAG) SELECT INSTANCE_ID, UPDATE_TAG FROM WF.PROCESS_INSTANCES WHERE INSTANCE_ID = {1}",
				mappingInfo.TableName, TSqlBuilder.Instance.CheckUnicodeQuotationMark(pq.ProcessID));

			return sql;
		}
	}
}
