using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfPendingActivityInfoAdapter : UpdatableAndLoadableAdapterBase<WfPendingActivityInfo, WfPendingActivityInfoCollection>
	{
		public static readonly WfPendingActivityInfoAdapter Instance = new WfPendingActivityInfoAdapter();

		private WfPendingActivityInfoAdapter()
		{
		}

		/// <summary>
		/// 查询所有挂起的活动
		/// </summary>
		/// <returns></returns>
		public WfPendingActivityInfoCollection LoadAll()
		{
			return Load(string.Empty, string.Empty);
		}

		/// <summary>
		/// 查询所有挂起的活动
		/// </summary>
		/// <param name="applicationName"></param>
		/// <param name="programName"></param>
		/// <returns></returns>
		public WfPendingActivityInfoCollection Load(string applicationName, string programName)
		{
			return Load(builder =>
			{
				if (applicationName.IsNotEmpty())
					builder.AppendItem("APPLICATION_NAME", applicationName);

				if (programName.IsNotEmpty())
					builder.AppendItem("PROGRAM_NAME", programName);
			});
		}

		public void Delete(WfPendingActivityInfoCollection pendingActivities)
		{
			pendingActivities.NullCheck("pendingActivities");

			InSqlClauseBuilder builder = new InSqlClauseBuilder("ACTIVITY_ID");

			pendingActivities.ForEach(pai => builder.AppendItem(pai.ActivityID));

			if (builder.Count > 0)
			{
				string sql = string.Format("DELETE FROM WF.PENDING_ACTIVITIES WHERE {0}",
					builder.ToSqlString(TSqlBuilder.Instance));

				DbHelper.RunSql(sql);
			}
		}

		public void Delete(WfActivityCollection pendingActivities)
		{
			pendingActivities.NullCheck("pendingActivities");

			InSqlClauseBuilder builder = new InSqlClauseBuilder("ACTIVITY_ID");

			pendingActivities.ForEach(pai => builder.AppendItem(pai.ID));

			if (builder.Count > 0)
			{
				string sql = string.Format("DELETE FROM WF.PENDING_ACTIVITIES WHERE {0}",
                    builder.ToSqlString(TSqlBuilder.Instance));

				DbHelper.RunSql(sql);
			}
		}

		public void DeleteByProcesses(IEnumerable<IWfProcess> processes)
		{
			processes.NullCheck("processes");

			InSqlClauseBuilder builder = new InSqlClauseBuilder("PROCESS_ID");

			foreach (IWfProcess process in processes)
				builder.AppendItem(process.ID);

			if (builder.Count > 0)
			{
				string sql = string.Format("DELETE FROM WF.PENDING_ACTIVITIES WHERE {0}",
                    builder.ToSqlString(TSqlBuilder.Instance));

				DbHelper.RunSql(sql);
			}
		}

		protected override string GetConnectionName()
		{
			return WfRuntime.ProcessContext.SimulationContext.GetConnectionName(WorkflowSettings.GetConfig().ConnectionName);
		}
	}
}
