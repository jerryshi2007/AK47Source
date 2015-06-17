using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;
using System.Transactions;
using MCS.Library.Data;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfRelativeProcessAdapter : UpdatableAndLoadableAdapterBase<WfRelativeProcess, WfRelativeProcessCollection>
	{
		public static readonly WfRelativeProcessAdapter Instance = new WfRelativeProcessAdapter();

		private WfRelativeProcessAdapter()
		{
		}

		public WfRelativeProcessCollection Load(string processID)
		{
			return LoadByInBuilder(b =>
			{
				b.DataField = "PROCESS_ID";
				b.AppendItem(processID);
			});
		}

		public void Update(string processID, WfRelativeProcessCollection relativeProcesses)
		{
			processID.CheckStringIsNullOrEmpty("processID");
			relativeProcesses.NullCheck("relativeProcesses");

            WhereSqlClauseBuilder wBuilder = new WhereSqlClauseBuilder();

            wBuilder.AppendItem("PROCESS_ID", processID);
            wBuilder.AppendTenantCodeSqlClause(typeof(WfProcessCurrentActivity));

			ORMappingItemCollection mapping = ORMapping.GetMappingInfo<WfRelativeProcess>();

			string sqlDelete = string.Format(
				"DELETE {0} WHERE {1}",
				mapping.TableName,
                wBuilder.ToSqlString(TSqlBuilder.Instance));

			StringBuilder strB = new StringBuilder();

			foreach (WfRelativeProcess relativeProcess in relativeProcesses)
			{
				if (strB.Length > 0)
					strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

				strB.Append(ORMapping.GetInsertSql(relativeProcess, TSqlBuilder.Instance));
			}

			string sql = sqlDelete;

			if (strB.Length > 0)
				sql += TSqlBuilder.Instance.DBStatementSeperator + strB.ToString();

			DbHelper.RunSqlWithTransaction(sql, GetConnectionName());
		}

		public override void Delete(WfRelativeProcess data)
		{
			ORMappingItemCollection mapping = ORMapping.GetMappingInfo<WfRelativeProcess>();

            WhereSqlClauseBuilder wBuilder = new WhereSqlClauseBuilder();

            wBuilder.AppendItem("PROCESS_ID", data.ProcessID);
            wBuilder.AppendTenantCodeSqlClause(typeof(WfProcessCurrentActivity));

			string sql = string.Format("DELETE {0} WHERE {1}", mapping.TableName, wBuilder.ToSqlString(TSqlBuilder.Instance));
			DbHelper.RunSql(sql, GetConnectionName());
		}

		public void Delete(WfProcessCurrentInfoCollection processesInfo)
		{
			ORMappingItemCollection mapping = ORMapping.GetMappingInfo<WfRelativeProcess>();
			InSqlClauseBuilder items = new InSqlClauseBuilder("PROCESS_ID");

			foreach (var process in processesInfo)
                items.AppendItem(process.InstanceID);

			if (items.Count > 0)
			{
                string sql = string.Format("DELETE {0} WHERE {1}",
                    mapping.TableName,
                    items.AppendTenantCodeSqlClause(typeof(WfRelativeProcess)).ToSqlString(TSqlBuilder.Instance));
				
                DbHelper.RunSql(sql, GetConnectionName());
			}
		}

		protected override string GetConnectionName()
		{
			return WfRuntime.ProcessContext.SimulationContext.GetConnectionName(WorkflowSettings.GetConfig().ConnectionName);
		}
	}
}
