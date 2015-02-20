using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects
{
	public class StartWorkflowJobAdapter :
		UpdatableAndLoadableAdapterBase<StartWorkflowJob, StartWorkflowJobCollection>
	{
		internal static readonly string DELETE_JOB_START_WORKFLOW_SQL_CLAUSE = "DELETE WF.JOB_START_WORKFLOW WHERE ";
		private static readonly string LOAD_VALID_JOB_SQL_CLAUSE = @"SELECT J.*, S.PROCESS_KEY, S.OPERATOR_ID, S.OPERATOR_NAME FROM WF.JOBS J JOIN WF.JOB_START_WORKFLOW S ON J.JOB_ID = S.JOB_ID AND J.ENABLED = 1";

		internal const string SingleData_StartWorkflowJob = "SELECT TOP(1) * FROM WF.JOB_START_WORKFLOW WHERE {0}";

		public static readonly StartWorkflowJobAdapter Instance = new StartWorkflowJobAdapter();

		private StartWorkflowJobAdapter() { }

		public StartWorkflowJob LoadSingleDataByJobID(string jobID)
		{
			jobID.CheckStringIsNullOrEmpty("jobID");

			InSqlClauseBuilder builder = new InSqlClauseBuilder("JOB_ID");

			builder.AppendItem(jobID);

			return LoadSingleData(builder);
		}

		public StartWorkflowJob LoadSingleData(IConnectiveSqlClause whereClause)
		{
			StartWorkflowJob result = null;

			if (whereClause.IsEmpty == false)
			{
				using (DbContext context = DbHelper.GetDBContext(GetConnectionName()))
				{
					using (IDataReader dr = DbHelper.RunSqlReturnDR(string.Format(SingleData_StartWorkflowJob, whereClause.ToSqlString(TSqlBuilder.Instance)), GetConnectionName()))
					{
						while (dr.Read())
						{
							result = new StartWorkflowJob();

							ORMapping.DataReaderToObject(dr, result);
							break;
						}
					}

					if (result != null)
						result.InitJobBaseData(JobBaseAdapter.Instance.LoadSingleDataByJobID(whereClause));
				}
			}

			return result;
		}

		protected override void AfterInnerDelete(StartWorkflowJob data, Dictionary<string, object> context)
		{
			JobBaseAdapter.Instance.Delete(data);
		}

		protected override void AfterLoad(StartWorkflowJobCollection data)
		{
			foreach (var startWfJob in data)
			{
				var baseColl = JobBaseAdapter.Instance.Load(p => p.AppendItem("JOB_ID", startWfJob.JobID));
				if (baseColl.Count == 0)
					return;

				startWfJob.InitJobBaseData(baseColl[0]);
			}
		}

		protected override void InnerInsert(StartWorkflowJob data, Dictionary<string, object> context)
		{
			InsertSqlClauseBuilder builder = new InsertSqlClauseBuilder();

			FillSqlBuilder(builder, data);

			string sql = string.Format("INSERT INTO {0} {1}",
				GetMappingInfo(context).TableName,
				builder.ToSqlString(TSqlBuilder.Instance));

			DbHelper.RunSql(sql, GetConnectionName());

			JobBaseAdapter.Instance.Update(data);
		}

		protected override int InnerUpdate(StartWorkflowJob data, Dictionary<string, object> context)
		{
			UpdateSqlClauseBuilder builder = new UpdateSqlClauseBuilder();

			FillSqlBuilder(builder, data);

			string sql = string.Format("UPDATE {0} SET {1} WHERE [JOB_ID] = {2}",
				GetMappingInfo(context).TableName,
				builder.ToSqlString(TSqlBuilder.Instance),
				TSqlBuilder.Instance.CheckUnicodeQuotationMark(data.JobID));

			int result = DbHelper.RunSql(sql, GetConnectionName());

			if (result > 0)
				JobBaseAdapter.Instance.Update(data);

			return result;
		}

		private static void FillSqlBuilder(SqlClauseBuilderIUW builder, StartWorkflowJob data)
		{
			builder.AppendItem("JOB_ID", data.JobID);
			builder.AppendItem("PROCESS_KEY", data.ProcessKey);

			if (OguBase.IsNotNullOrEmpty(data.Operator))
			{
				builder.AppendItem("OPERATOR_ID", data.Operator.ID);
				builder.AppendItem("OPERATOR_NAME", data.Operator.DisplayName);
			}
		}

		public int Delete(string[] ids)
		{
			InSqlClauseBuilder builder = new InSqlClauseBuilder();

			builder.AppendItem(ids);

			int result = 0;

			if (builder.Count > 0)
			{
				string sql = string.Format("{0} JOB_ID {1}",
					DELETE_JOB_START_WORKFLOW_SQL_CLAUSE, builder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

				using (TransactionScope tran = TransactionScopeFactory.Create())
				{
					JobBaseAdapter.Instance.Delete(ids);
					result = DbHelper.RunSqlWithTransaction(sql, GetConnectionName());
					tran.Complete();
				}
			}

			return result;
		}

		public void SmartDelete(string jobId)
		{
			//InSqlClauseBuilder builder = new InSqlClauseBuilder();

			//builder.AppendItem(ids);

			//int result = 0;

			//if (builder.Count > 0)
			//{
			//    string sql = string.Format("{0} JOB_ID {1}",
			//        DELETE_JOB_START_WORKFLOW_SQL_CLAUSE, builder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

			//    using (TransactionScope tran = TransactionScopeFactory.Create())
			//    {
			//        JobBaseAdapter.Instance.Delete(ids);
			//        result = DbHelper.RunSqlWithTransaction(sql, GetConnectionName());
			//        tran.Complete();
			//    }
			//}

			//return result;
		}

		public StartWorkflowJobCollection LoadValidData()
		{
			var result = new StartWorkflowJobCollection();
			var ds = DbHelper.RunSqlReturnDS(LOAD_VALID_JOB_SQL_CLAUSE);
			ORMapping.DataViewToCollection(result, ds.Tables[0].DefaultView);

			List<string> jobIDs = new List<string>();

			result.ForEach(d => jobIDs.Add(d.JobID));

			JobScheduleWithJobIDCollection schedules = JobScheduleAdapter.Instance.LoadByJobID(jobIDs.ToArray());

			foreach (JobBase job in result)
			{
				IList<JobScheduleWithJobID> list = schedules.FindAll(s => s.JobID == job.JobID);

				foreach (JobScheduleWithJobID schdule in list)
					job.Schedules.Add(schdule);
			}

			return result;
		}

		protected override string GetConnectionName()
		{
			return WorkflowSettings.GetConfig().ConnectionName;
		}
	}
}
