using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;
using MCS.Library.Data;
using System.Data.Common;
using System.Transactions;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class InvalidAssigneesUrlAdapter
	{
		public static readonly InvalidAssigneesUrlAdapter Instance = new InvalidAssigneesUrlAdapter();

		private InvalidAssigneesUrlAdapter()
		{
		}

		public void BulkAdd(InvalidAssignessUrlCollection data)
		{
			ORMappingItemCollection mappings = ORMapping.GetMappingInfo<InvalidAssigneeUrl>();

			StringBuilder sqlStrB = new StringBuilder();

			foreach (InvalidAssigneeUrl item in data)
			{
				if (sqlStrB.Length > 0)
					sqlStrB.Append(TSqlBuilder.Instance.DBStatementSeperator);

				sqlStrB.Append(ORMapping.GetInsertSql(item, mappings, TSqlBuilder.Instance));
			}

			DbHelper.RunSqlWithTransaction(sqlStrB.ToString(), this.GetConnectionName());
		}

		private string GetConnectionName()
		{
			return WorkflowSettings.GetConfig().ConnectionName;
		}
	}
}
