using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using System.Data;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfProcessCurrentActivityAdapter
	{
		public static readonly WfProcessCurrentActivityAdapter Instance = new WfProcessCurrentActivityAdapter();

		private WfProcessCurrentActivityAdapter()
		{ }

		/// <summary>
		/// 返回流程所有的已流转过的活动节点
		/// </summary>
		/// <param name="processID"></param>
		/// <returns></returns>
		public WfProcessCurrentActivityCollection Load(string processID)
		{
			processID.CheckStringIsNullOrEmpty(processID);

			WfProcessCurrentActivityCollection result = new WfProcessCurrentActivityCollection();

			string sql = string.Format("SELECT * FROM WF.PROCESS_CURRENT_ACTIVITIES WHERE PROCESS_ID = {0}",
				TSqlBuilder.Instance.CheckQuotationMark(processID, true));

			DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

			result.LoadFromDataView(table.DefaultView);

			return result;
		}

		public void Update(string processID, WfProcessCurrentActivityCollection pcas)
		{
			processID.CheckStringIsNullOrEmpty("processID");
			pcas.NullCheck("pcas");

			StringBuilder strB = new StringBuilder();
			InSqlClauseBuilder deleteActivityIDs = new InSqlClauseBuilder();

			foreach (WfProcessCurrentActivity pca in pcas)
			{
				if (strB.Length > 0)
					strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

				strB.Append(ORMapping.GetInsertSql(pca, TSqlBuilder.Instance));

				deleteActivityIDs.AppendItem(pca.ActivityID);
			}

			string sqlDelete = string.Format("DELETE WF.PROCESS_CURRENT_ACTIVITIES WHERE PROCESS_ID = {0}",
				TSqlBuilder.Instance.CheckQuotationMark(processID, true));

			if (deleteActivityIDs.Count > 0)
				sqlDelete += string.Format(" AND ACTIVITY_ID {0}", deleteActivityIDs.ToSqlStringWithInOperator(TSqlBuilder.Instance));

			string sql = sqlDelete;

			if (strB.Length > 0)
				sql += TSqlBuilder.Instance.DBStatementSeperator + strB.ToString();

			DbHelper.RunSqlWithTransaction(sql, GetConnectionName());
		}

		public void UpdateProcessActivities(IWfProcess process)
		{
			process.NullCheck("process");

			WfProcessCurrentActivityCollection pcas = new WfProcessCurrentActivityCollection();

			foreach (IWfActivity activity in process.Activities)
				pcas.Add(WfProcessCurrentActivity.FromActivity(activity));

			Update(process.ID, pcas);
		}

		/// <summary>
		/// 删除流程已活动过的所有的节点
		/// </summary>
		/// <param name="process"></param>
		public void DeleteProcessActivities(WfProcessCurrentInfoCollection processes)
		{
			processes.NullCheck("processes");

			InSqlClauseBuilder builder = new InSqlClauseBuilder();

			processes.ForEach(p => builder.AppendItem(p.InstanceID));

			if (builder.Count > 0)
			{
				string sql = string.Format("DELETE WF.PROCESS_CURRENT_ACTIVITIES WHERE PROCESS_ID {0}",
					builder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

				DbHelper.RunSql(sql, GetConnectionName());
			}
		}

		private static string GetConnectionName()
		{
			return WfRuntime.ProcessContext.SimulationContext.GetConnectionName(WorkflowSettings.GetConfig().ConnectionName);
		}
	}
}
