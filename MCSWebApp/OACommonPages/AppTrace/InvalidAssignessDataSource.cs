using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Data.Builder;
using MCS.Library.OGUPermission;
using MCS.Library.Data.DataObjects;

namespace MCS.OA.CommonPages.AppTrace
{
	public class InvalidAssignessDataSource : ObjectDataSourceQueryAdapterBase<InvalidAssigneeUrl, InvalidAssignessUrlCollection>
	{
		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			qc.SelectFields = " * ";
			if (string.IsNullOrEmpty(qc.OrderByClause) == true)
				qc.OrderByClause = " PROCESS_ID  ASC ";

			qc.FromClause = " WF.INVALID_ASSIGNEES_URLS ";

			/*
			if (qc.WhereClause.IsNotEmpty())
				qc.WhereClause += " AND "; */
		}

		protected override void OnAfterQuery(InvalidAssignessUrlCollection result)
		{
			InSqlClauseBuilder builder = new InSqlClauseBuilder();
			builder.DataField = "PROCESS_ID";

			foreach (InvalidAssigneeUrl item in result)
			{
				builder.AppendItem(item.ProcessID);
			}

			InvalidAssigneeCollection allInvalidAssignes = InvalidAssigneeAdapter.Instance.Load(builder);

			Dictionary<string, StringBuilder> dic = new Dictionary<string, StringBuilder>(StringComparer.OrdinalIgnoreCase);

			foreach (InvalidAssignee itemInavlidAssigne in allInvalidAssignes)
			{
				string key = string.Format("{0}@{1}", itemInavlidAssigne.ProcessID, itemInavlidAssigne.ActivityID);
				
				if (dic.ContainsKey(key) == false)
				{
					dic[key] = new StringBuilder();
					dic[key].Append(itemInavlidAssigne.UserName);
				}
				else
					dic[key].AppendFormat(",{0}", itemInavlidAssigne.UserName);
			}

			foreach (InvalidAssigneeUrl item in result)
			{
				string key = string.Format("{0}@{1}", item.ProcessID, item.ActivityID);

				if (dic.ContainsKey(key) == true)
					item.AllUsers = dic[key].ToString();
			}
		}

		protected override string GetConnectionName()
		{
			return WorkflowSettings.GetConfig().ConnectionName;
		}
	}
}