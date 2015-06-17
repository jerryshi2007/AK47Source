using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    public class WfApplicationAdapter : UpdatableAndLoadableAdapterBase<WfApplication, WfApplicationCollection>
    {
        public static readonly WfApplicationAdapter Instance = new WfApplicationAdapter();

        private WfApplicationAdapter()
        {
        }

        public WfApplicationCollection LoadAll()
        {
            string sql = "SELECT * FROM WF.APPLICATIONS ORDER BY SORT";

            DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

            WfApplicationCollection result = new WfApplicationCollection();

            ORMapping.DataViewToCollection(result, table.DefaultView);

            return result;
        }

        public WfProgramInApplicationCollection LoadProgramsByApplication(string appCodeName)
        {
            appCodeName.CheckStringIsNullOrEmpty("appCodeName");

            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

            builder.AppendItem("APPLICATION_CODE_NAME", appCodeName);

            string sql = string.Format("SELECT * FROM WF.PROGRAM_AND_ALIAS_VIEW WHERE {0} ORDER BY SORT",
                builder.ToSqlString(TSqlBuilder.Instance));

            DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

            WfProgramInApplicationCollection result = new WfProgramInApplicationCollection();

            ORMapping.DataViewToCollection(result, table.DefaultView);

            return result;
        }

        protected override string GetConnectionName()
        {
            return WorkflowSettings.GetConfig().ConnectionName;
        }
    }
}
