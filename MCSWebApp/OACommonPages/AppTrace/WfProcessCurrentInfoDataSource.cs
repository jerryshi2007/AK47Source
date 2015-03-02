using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;

namespace MCS.OA.CommonPages.AppTrace
{
    public class WfProcessCurrentInfoDataSource : ObjectDataSourceQueryAdapterBase<WfProcessCurrentInfo, WfProcessCurrentInfoCollection>
    {
        protected override void OnBuildQueryCondition(QueryCondition qc)
        {
            qc.FromClause = "WF.PROCESS_INSTANCES (NOLOCK)";

            if (qc.OrderByClause.IsNullOrEmpty())
                qc.OrderByClause = "START_TIME DESC";

            qc.SelectFields = ORMapping.GetSelectFieldsNameSql<WfProcessCurrentInfo>();

            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

            builder.AppendItem("COMMITTED", "1");
            builder.AppendTenantCode();

            if (qc.WhereClause.IsNotEmpty())
                qc.WhereClause += " AND ";

            qc.WhereClause += builder.ToSqlString(TSqlBuilder.Instance);
        }

        protected override string GetConnectionName()
        {
            return WorkflowSettings.GetConfig().ConnectionName;
        }
    }
}