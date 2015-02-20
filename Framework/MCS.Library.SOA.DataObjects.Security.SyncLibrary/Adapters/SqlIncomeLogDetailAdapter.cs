using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Schemas.Adapters;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary.Adapters
{
    public class SqlIncomeLogDetailAdapter : UpdatableAndLoadableAdapterBase<SqlIncomeSyncLogDetail, SqlIncomeSyncLogDetailCollection>
    {
        public static readonly SqlIncomeLogDetailAdapter Instance = new SqlIncomeLogDetailAdapter();

        private SqlIncomeLogDetailAdapter()
        {
        }

        protected override string GetConnectionName()
        {
            return ConnectionNameMappingSettings.GetConfig().GetConnectionName("PermissionsCenter", "PermissionsCenter");
        }
    }
}
