using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Services;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects;
using System.Threading;
using System.Threading.Tasks;
using MCS.Library.Data.Builder;
using MCS.Library.Data;
using System.Transactions;
using MCS.Library.Core;

namespace ActivateWfProcessService
{
	class ActivateWfProcessServiceThread : ThreadTaskBase
	{
		public override void OnThreadTaskStart()
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				JobCollection jobs = FetchStandByJobs(this.Params.BatchCount);

				foreach (JobBase job in jobs)
				{
					try
					{
						if (job.CanStart(TimeSpan.FromSeconds(10)))
						{
							job.SetCurrentJobBeginStatus();
							try
							{
								Task.Factory.StartNew(() => StartJob(job));
							}
							finally
							{
								job.SetCurrentJobEndStatus();
							}
						}
					}
					catch (Exception ex)
					{
						WriteJobException(job, this.Params.Log, "调度", ex);
					}
				}

				scope.Complete();
			}
		}

		private void StartJob(JobBase job)
		{
			try
			{
				JobBase detailJobInfo = LoadDetailJobInfo(job.JobID, job.JobType);
				detailJobInfo.Start();
			}
			catch (ThreadAbortException)
			{
			}
			catch (Exception ex)
			{
				WriteJobException(job, this.Params.Log, "执行", ex);
			}
			finally
			{
				job.SetCurrentJobEndStatus();
				string logDetail = string.Format("定时任务[{0},{1}]在[{2}]时执行]", job.Name,
					job.JobType.ToString(), DateTime.Now.ToString());

				UserOperationLog log = new UserOperationLog()
				{
					ResourceID = job.JobID,
					OperationDateTime = DateTime.Now,
					Subject = "定时任务执行",
					OperationName = job.Name,
					OperationDescription = logDetail
				};
				UserOperationLogAdapter.Instance.Update(log);
			}
		}

		private static JobCollection FetchStandByJobs(int batchCount)
		{
			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			builder.AppendItem("ENABLED", "1");
			builder.AppendItem("JOB_STATUS", (int)JobStatus.StandBy);

			return JobBaseAdapter.Instance.UPDLOCKLoadJobs(batchCount, builder);
		}

		private static JobBase LoadDetailJobInfo(string jobID, JobType jobType)
		{
			JobBase result = null;

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
			builder.AppendItem("JOB_ID", jobID);

			switch (jobType)
			{
				case JobType.StartWorkflow:
					result = StartWorkflowJobAdapter.Instance.LoadSingleData(builder);
					break;
				case JobType.InvokeService:
					result = InvokeWebServiceJobAdapter.Instance.LoadSingleData(builder);
					break;
			}

			return result;
		}

		private static void WriteJobException(JobBase job, ServiceLog log, string op, Exception ex)
		{
			string message = string.Format("{0}作业\"{1}\"({2})错误: {3}", op, job.Name, job.JobID, ex.GetRealException().ToString());

			log.Write(message);
		}
	}
}
