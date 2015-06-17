using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;
using MCS.Library.Core;
using MCS.Library.Data;

namespace MCS.Library.SOA.DataObjects
{
    internal class DefaultCommonInfoMappingUpdater : UpdatableAdapterBase<CommonInfoMapping>, ICommonInfoMappingUpdater
    {
        public static readonly DefaultCommonInfoMappingUpdater Instance = new DefaultCommonInfoMappingUpdater();

        private DefaultCommonInfoMappingUpdater()
        {
        }

        public void Update(CommonInfoMappingCollection cimItems)
        {
            cimItems.NullCheck("cimItems");

            string sqlString = string.Empty;
            StringBuilder strB = new StringBuilder();

            ConnectiveSqlClauseCollection connective = new ConnectiveSqlClauseCollection(LogicOperatorDefine.Or);

            foreach (CommonInfoMapping cim in cimItems)
            {
                if (cim.ResourceID.IsNotEmpty() && cim.ProcessID.IsNotEmpty())
                {
                    if (strB.Length > 0)
                        strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

                    WhereSqlClauseBuilder wBuilder = new WhereSqlClauseBuilder();

                    wBuilder.AppendItem("RESOURCE_ID", cim.ResourceID);
                    wBuilder.AppendItem("PROCESS_ID", cim.ProcessID);
                    wBuilder.AppendTenantCode(typeof(CommonInfoMapping));

                    connective.Add(wBuilder);

                    strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

                    strB.Append(ORMapping.GetInsertSql(cim, TSqlBuilder.Instance));
                }
            }

            if (connective.Count > 0)
            {
                sqlString = string.Format("DELETE WF.COMMON_INFO_MAPPING WHERE {0}", connective.ToSqlString(TSqlBuilder.Instance));

                sqlString += strB.ToString();
            }

            if (sqlString.Length > 0)
                DbHelper.RunSqlWithTransaction(sqlString, WorkflowSettings.GetConfig().ConnectionName);
        }
    }
}
