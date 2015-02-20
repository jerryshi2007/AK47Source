using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public sealed class WfActivityTemplateAdpter :
		UpdatableAndLoadableAdapterBase<WfActivityTemplate, WfActivityTemplateCollection>
	{
		public static readonly WfActivityTemplateAdpter Instance = new WfActivityTemplateAdpter();
		private static readonly string SQLCOMMAND_DELETE = @"DELETE FROM WF.ACTIVITY_TEMPLATE WHERE ID {0}";

		private WfActivityTemplateAdpter() { }

		public WfActivityTemplateCollection LoadAvailableTemplates()
		{
			return Load(p => p.AppendItem("AVAILABLE", 1));
		}

		public int DeleteTemplates(string[] templateIDs)
		{
			InSqlClauseBuilder builder = new InSqlClauseBuilder();

			templateIDs.ForEach(p => builder.AppendItem(p));

			int result = 0;

			if (builder.Count > 0)
			{
				string sql = string.Format(SQLCOMMAND_DELETE, builder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

				result = DbHelper.RunSql(sql, GetConnectionName());
			}

			return result;
		}

		protected override string GetConnectionName()
		{
			return WorkflowSettings.GetConfig().ConnectionName;
		}
	}
}
