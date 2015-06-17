using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using System.Data;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class InvalidAssigneeAdapter
    //: UpdatableAndLoadableAdapterBase<InvalidAssigne,InvalidAssigneCollection>
    {
        public static readonly InvalidAssigneeAdapter Instance = new InvalidAssigneeAdapter();

        internal const string SelectSQL = @"SELECT * FROM WF.INVALID_ASSIGNEES";

        public InvalidAssigneeAdapter()
        {
        }

        public InvalidAssigneeCollection Load(IConnectiveSqlClause builder)
        {
            InvalidAssigneeCollection result = new InvalidAssigneeCollection();

            string sql = SelectSQL;

            if (builder.IsEmpty == false)
                sql = string.Format("{0} WHERE {1}",
                    SelectSQL,
                    builder.AppendTenantCodeSqlClause(typeof(InvalidAssignee)).ToSqlString(TSqlBuilder.Instance));

            DataTable dt = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

            ORMapping.DataViewToCollection(result, dt.DefaultView);

            return result;
        }

        private string GetConnectionName()
        {
            return WorkflowSettings.GetConfig().ConnectionName;
        }
    }
}
