using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Workflow;
using System.Transactions;

namespace MCS.Library.SOA.DataObjects
{
	public class JobBaseAdapter : UpdatableAndLoadableAdapterBase<JobBase, JobCollection>
	{
		internal static readonly string DELETE_JOBS_SQL_CLAUSE = "DELETE WF.JOBS WHERE ";
		internal static readonly string DELETE_JOB_SCHEDULES_SQL_CLAUSE = "DELETE WF.JOB_SCHEDULES WHERE ";

		internal static readonly string SELECT_JOB_SCHEDULES_ID_CLAUSE = "SELECT SCHEDULE_ID FROM WF.JOB_SCHEDULES WHERE ";
		internal static readonly string INSERT_JOB_SCHEDULES_SQL_CLAUSE = "INSERT INTO WF.JOB_SCHEDULES ";

		internal const string UPDLOCK_LOAD_JOBS = "SELECT {0} * FROM WF.JOBS WITH(UPDLOCK READPAST) WHERE {1} ORDER BY CREATE_TIME";

		internal const string SINGLEATA_JOB = "SELECT TOP(1) * FROM WF.JOBS WHERE {0}";

		public static readonly JobBaseAdapter Instance = new JobBaseAdapter();

		private JobBaseAdapter()
		{
		}

		/// <summary>
		/// 读取在时间范围内，没有执行过且没有正在运行任务的作业。然后转换为任务并且入库
		/// </summary>
		/// <param name="batchCount"></param>
		/// <param name="timeOffset"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public SysTaskCollection FetchNotDispatchedJobsAndConvertToTask(int batchCount, TimeSpan timeOffset, Action<JobBase, SysTask> action)
		{
			SysTaskCollection result = new SysTaskCollection();

			FetchNotDispatchedJobs(batchCount, timeOffset, job =>
			{
				JobSchedule matchedSchedule = null;

				if (job.CanStart(timeOffset, out matchedSchedule))
				{
					SysTask task = job.ToSysTask();

					task.FillData(BuildTaskExtraData(job, timeOffset, matchedSchedule));

					job.SetCurrentJobBeginStatus();
					SysTaskAdapter.Instance.Update(task);

					if (action != null)
						action(job, task);

					result.Add(task);
				}
			});

			return result;
		}

		private static Dictionary<string, string> BuildTaskExtraData(JobBase job, TimeSpan timeOffset, JobSchedule matchedSchedule)
		{
			Dictionary<string, string> extraData = new Dictionary<string, string>();

			if (job.LastStartExecuteTime != null)
				extraData.Add("LastStartExecuteTime", job.LastStartExecuteTime.Value.ToString("yyyy-MM-dd HH:mm:ss.fff"));

			if (matchedSchedule != null)
				extraData.Add("ScheduleID", matchedSchedule.ID);

			extraData.Add("TimeOffset", timeOffset.TotalSeconds.ToString("#,##0.00"));

			return extraData;
		}

		///// <summary>
		///// 读取在时间范围内，没有执行过且没有正在运行任务的作业。仅通过最后执行时间进行判断
		///// </summary>
		///// <param name="batchCount"></param>
		///// <param name="timeOffset"></param>
		///// <param name="action"></param>
		///// <returns></returns>
		//public JobCollection FetchNotDispatchedJobs(int batchCount, TimeSpan timeOffset, Action<JobBase> action)
		//{
		//    JobCollection result = new JobCollection();
		//    string sql = "SELECT {0} J.* FROM WF.JOBS J WITH(UPDLOCK READPAST) LEFT JOIN WF.SYS_NOT_RUNNING_TASK T WITH(UPDLOCK READPAST) ON J.JOB_ID = T.RESOURCE_ID WHERE (T.STATUS IS NULL) AND {1} ORDER BY CREATE_TIME";

		//    using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
		//    {
		//        string top = batchCount >= 0 ? "TOP " + batchCount : string.Empty;

		//        WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

		//        builder.AppendItem("ENABLED", "1");

		//        WhereSqlClauseBuilder builder2 = new WhereSqlClauseBuilder(LogicOperatorDefine.Or);
		//        builder2.AppendItem("LAST_START_EXE_TIME", string.Format("DATEADD(SECOND, -{0}, GETDATE())", (int)timeOffset.TotalSeconds), "<", true);
		//        builder2.AppendItem("LAST_START_EXE_TIME", "NULL", "IS", true);

		//        Database db = DatabaseFactory.Create(context);

		//        using (TransactionScope scope = TransactionScopeFactory.Create())
		//        {
		//            using (IDataReader dr = db.ExecuteReader(CommandType.Text,
		//                string.Format(sql, top, new ConnectiveSqlClauseCollection(builder, builder2).ToSqlString(TSqlBuilder.Instance), SysTaskStatus.NotRunning.ToString())))
		//            {
		//                while (dr.Read())
		//                {
		//                    JobBase job = new JobBase();

		//                    ORMapping.DataReaderToObject(dr, job);

		//                    result.Add(job);
		//                }
		//            }

		//            JobScheduleAdapter.Instance.FillJobsSchdules(result);

		//            foreach (JobBase job in result)
		//            {
		//                if (action != null)
		//                    action(job);
		//            }

		//            Console.WriteLine("Read {0}", result.Count);
		//            scope.Complete();
		//        }
		//    }

		//    return result;
		//}

		/// <summary>
		/// 读取在时间范围内，没有执行过且没有正在运行任务的作业。仅通过最后执行时间进行判断
		/// </summary>
		/// <param name="batchCount"></param>
		/// <param name="timeOffset"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public JobCollection FetchNotDispatchedJobs(int batchCount, TimeSpan timeOffset, Action<JobBase> action)
		{
			IList<string> jobIDs = LoadUnTaskedJobs(batchCount, timeOffset);

			JobCollection result = new JobCollection();

			foreach (string jobID in jobIDs)
			{
				using (TransactionScope scope = TransactionScopeFactory.Create())
				{
					JobBase job = LoadOneUnTaskedJob(jobID);

					if (job != null)
					{
						if (action != null)
							action(job);

						result.Add(job);
					}

					scope.Complete();
				}
			}

			return result;
		}

		public JobCollection UPDLOCKLoadJobs(int batchCount, IConnectiveSqlClause whereClause)
		{
			JobCollection result = new JobCollection();

			if (whereClause.IsEmpty == false)
			{
				using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
				{
					string top = batchCount >= 0 ? "TOP " + batchCount : string.Empty;

					Database db = DatabaseFactory.Create(context);

					using (IDataReader dr = db.ExecuteReader(CommandType.Text,
						string.Format(UPDLOCK_LOAD_JOBS, top, whereClause.ToSqlString(TSqlBuilder.Instance))))
					{
						ORMapping.DataReaderToCollection(result, dr);
					}
				}

				AfterLoad(result);
			}

			return result;
		}

		/// <summary>
		/// 根据ID加载JOB
		/// </summary>
		/// <param name="jobID"></param>
		/// <returns></returns>
		public JobBase LoadSingleDataByJobID(string jobID)
		{
			jobID.CheckStringIsNullOrEmpty("jobID");
			InSqlClauseBuilder builder = new InSqlClauseBuilder("JOB_ID");

			builder.AppendItem(jobID);

			return LoadSingleDataByJobID(builder);
		}

		public JobBase LoadSingleDataByJobID(IConnectiveSqlClause whereClause)
		{
			JobBase result = null;

			if (whereClause.IsEmpty == false)
			{
				using (DbContext context = DbHelper.GetDBContext(GetConnectionName()))
				{
					using (IDataReader dr = DbHelper.RunSqlReturnDR(string.Format(SINGLEATA_JOB, whereClause.ToSqlString(TSqlBuilder.Instance)), GetConnectionName()))
					{
						while (dr.Read())
						{
							result = new JobBase();
							ORMapping.DataReaderToObject(dr, result);
							break;
						}
					}
				}
			}

			return result;
		}

		/// <summary>
		/// 读取没有安排到任务中的JobID的集合
		/// </summary>
		/// <param name="batchCount"></param>
		/// <returns></returns>
		public IList<string> LoadUnTaskedJobs(int batchCount, TimeSpan timeOffset)
		{
			string sqlTemplate = "SELECT {0} J.JOB_ID FROM WF.JOBS J LEFT JOIN WF.SYS_NOT_RUNNING_TASK T WITH(UPDLOCK READPAST) ON J.JOB_ID = T.RESOURCE_ID WHERE (T.STATUS IS NULL) AND {1} ORDER BY J.CREATE_TIME";
			string top = batchCount >= 0 ? "TOP " + batchCount : string.Empty;

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			builder.AppendItem("ENABLED", "1");

			WhereSqlClauseBuilder builder2 = new WhereSqlClauseBuilder(LogicOperatorDefine.Or);
			builder2.AppendItem("LAST_START_EXE_TIME", string.Format("DATEADD(SECOND, -{0}, GETDATE())", (int)timeOffset.TotalSeconds), "<", true);
			builder2.AppendItem("LAST_START_EXE_TIME", "NULL", "IS", true);

			string sql = string.Format(sqlTemplate, top, new ConnectiveSqlClauseCollection(builder, builder2).ToSqlString(TSqlBuilder.Instance));

			List<string> result = new List<string>(800);

			using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
			{
				Database db = DatabaseFactory.Create(context);

				DataTable table = db.ExecuteDataSet(CommandType.Text, sql).Tables[0];

				foreach (DataRow row in table.Rows)
				{
					result.Add(row["JOB_ID"].ToString());
				}
			}

			return result;
		}

		public JobBase LoadOneUnTaskedJob(string jobID)
		{
			string sqlTemplate = "SELECT J.* FROM WF.JOBS J WITH(UPDLOCK READPAST) LEFT JOIN WF.SYS_NOT_RUNNING_TASK T WITH(UPDLOCK READPAST) ON J.JOB_ID = T.RESOURCE_ID WHERE (T.STATUS IS NULL) AND J.JOB_ID = {0}";

			string sql = string.Format(sqlTemplate, TSqlBuilder.Instance.CheckUnicodeQuotationMark(jobID));

			JobBase result = null;

			using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
			{
				Database db = DatabaseFactory.Create(context);

				using (IDataReader dr = db.ExecuteReader(CommandType.Text, sql))
				{
					while (dr.Read())
					{
						JobBase job = new JobBase();

						ORMapping.DataReaderToObject(dr, job);

						result = job;
					}
				}

				if (result != null)
					JobScheduleAdapter.Instance.FillJobSchdules(result);
			}

			return result;
		}

		public override void ClearAll()
		{
			base.ClearAll();

			DbHelper.RunSql("DELETE FROM WF.JOB_SCHEDULES", GetConnectionName());
		}

		protected override void AfterLoad(JobCollection data)
		{
			List<string> jobIDs = new List<string>();

			data.ForEach(d => jobIDs.Add(d.JobID));

			JobScheduleWithJobIDCollection schedules = JobScheduleAdapter.Instance.LoadByJobID(jobIDs.ToArray());

			foreach (JobBase job in data)
			{
				IList<JobScheduleWithJobID> list = schedules.FindAll(s => s.JobID == job.JobID);

				foreach (JobScheduleWithJobID schdule in list)
					job.Schedules.Add(schdule);
			}
		}

		protected override void AfterInnerUpdate(JobBase data, Dictionary<string, object> context)
		{
			//更新任务-计划映射表JOB_SCHEDULES
			StringBuilder insertClause = new StringBuilder();

			insertClause.Append(GetJobSchedulesDeleteClause(data.JobID));
			insertClause.Append(TSqlBuilder.Instance.DBStatementSeperator);

			foreach (var schedule in data.Schedules)
			{
				InsertSqlClauseBuilder insertBuilder = new InsertSqlClauseBuilder();
				insertBuilder.AppendItem("JOB_ID", data.JobID);
				insertBuilder.AppendItem("SCHEDULE_ID", schedule.ID);

				insertClause.Append(INSERT_JOB_SCHEDULES_SQL_CLAUSE + insertBuilder.ToSqlString(TSqlBuilder.Instance));
				insertClause.Append(TSqlBuilder.Instance.DBStatementSeperator);
			}

			DbHelper.RunSql(insertClause.ToString(), GetConnectionName());
		}

		protected override void AfterInnerDelete(JobBase data, Dictionary<string, object> context)
		{
			//删除映射
			DbHelper.RunSql(GetJobSchedulesDeleteClause(data.JobID), GetConnectionName());
		}

		protected override string GetConnectionName()
		{
			return WorkflowSettings.GetConfig().ConnectionName;
		}

		public int Delete(string[] ids)
		{
			StringBuilder strBuilder = new StringBuilder();

			InSqlClauseBuilder builder = new InSqlClauseBuilder();
			builder.AppendItem(ids);

			int result = 0;

			if (builder.Count > 0)
			{
				strBuilder.AppendFormat("{0} JOB_ID {1}", DELETE_JOBS_SQL_CLAUSE, builder.ToSqlStringWithInOperator(TSqlBuilder.Instance));
				strBuilder.Append(GetJobSchedulesDeleteClause(ids));

				result = DbHelper.RunSqlWithTransaction(strBuilder.ToString(), GetConnectionName());
			}

			return result;
		}

		internal int UpdateLastExeTime(JobBase data)
		{
			return DbHelper.RunSqlWithTransaction(string.Format("UPDATE WF.JOBS SET LAST_EXE_TIME = {0}, JOB_STATUS = {1} WHERE (JOB_ID = {2})",
				TSqlBuilder.Instance.FormatDateTime(data.LastExecuteTime.Value),
				(int)data.Status, TSqlBuilder.Instance.CheckUnicodeQuotationMark(data.JobID)), this.GetConnectionName());
		}

		internal void SetStartJobStatus(JobBase data)
		{
			DbHelper.RunSqlWithTransaction(string.Format("UPDATE WF.JOBS SET LAST_START_EXE_TIME = {0}, JOB_STATUS = {1} WHERE (JOB_ID = {2})",
				TSqlBuilder.Instance.FormatDateTime(data.LastStartExecuteTime.Value),
				(int)data.Status, TSqlBuilder.Instance.CheckUnicodeQuotationMark(data.JobID)), this.GetConnectionName());
		}

		public JobType GetJobType(string jobid)
		{
			string sql = string.Format(@"select JOB_TYPE from WF.JOBS where JOB_ID = {0}",
				TSqlBuilder.Instance.CheckQuotationMark(jobid, true));

			object obj = DbHelper.RunSqlReturnScalar(sql);

			return (JobType)Enum.Parse(typeof(JobType), obj.ToString());
		}

		private static string GetJobSchedulesDeleteClause(params string[] jobIDs)
		{
			InSqlClauseBuilder builder = new InSqlClauseBuilder();

			builder.AppendItem(jobIDs);

			string sql = string.Empty;

			if (builder.Count > 0)
				sql = string.Format("{0} JOB_ID {1}", DELETE_JOB_SCHEDULES_SQL_CLAUSE, builder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

			return sql;
		}
	}
}
