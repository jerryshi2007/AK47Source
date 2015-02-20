using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;
using System.Data;
using MCS.Library.Caching;
using MCS.Library.Data.DataObjects;

namespace MCS.OA.CommonPages.AppTrace
{
	public class CagetoryDataSource : DataViewDataSourceQueryAdapterBase
	{
		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			qc.FromClause = "WF.APP_PROGRAM_AUTH A (NOLOCK) LEFT JOIN WF.APPLICATIONS B ON A.APPLICATION_NAME=B.CODE_NAME LEFT JOIN WF.PROGRAMS C ON C.CODE_NAME = A.PROGRAM_NAME AND C.APPLICATION_CODE_NAME = B.CODE_NAME";

			if (qc.OrderByClause.IsNullOrEmpty())
				qc.OrderByClause = "CREATE_TIME DESC";

			//qc.SelectFields = ORMapping.GetSelectFieldsNameSql<WfApplicationAuth>();
			qc.SelectFields = "A.APPLICATION_NAME,A.PROGRAM_NAME,AUTH_TYPE,ROLE_ID,ROLE_DESCRIPTION,CREATE_TIME,B.NAME AS APP_DISP,C.NAME AS PROGRAM_DISP";

		}

		protected override string GetConnectionName()
		{
			return WorkflowSettings.GetConfig().ConnectionName;
		}

		public DataView Query(int startRowIndex, int maximumRows, string appName, string programName, WfApplicationAuthType authType, string where, string orderBy, ref int totalCount)
		{
			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			if (string.IsNullOrEmpty(appName) == false)
			{
				builder.AppendItem("APPLICATION_NAME", appName);

				if (string.IsNullOrEmpty(programName) == false)
				{
					builder.AppendItem("PROGRAM_NAME", programName);
				}
			}

			if (authType != WfApplicationAuthType.None)
			{
				builder.AppendItem("AUTH_TYPE", authType.ToString());
			}

			string sql = builder.ToSqlString(TSqlBuilder.Instance);

			if (string.IsNullOrEmpty(where) == false)
			{
				sql = (string.IsNullOrEmpty(sql) ? where : sql + " AND (" + where + ")");
			}

			return base.Query(startRowIndex, maximumRows, sql, orderBy, ref totalCount);
		}

		public int GetQueryCount(string appName, string programName, WfApplicationAuthType authType, string where, ref int totalCount)
		{
			return (int)ObjectContextCache.Instance[ContextCacheKey];
		}
	}
}