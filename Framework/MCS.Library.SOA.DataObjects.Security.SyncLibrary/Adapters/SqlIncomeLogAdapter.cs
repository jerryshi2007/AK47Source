using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Schemas.Adapters;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary.Adapters
{
    public class SqlIncomeLogAdapter : UpdatableAndLoadableAdapterBase<SqlIncomeSyncLog, SqlIncomeSyncLogCollection>
    {
        public static readonly SqlIncomeLogAdapter Instance = new SqlIncomeLogAdapter();

        private SqlIncomeLogAdapter()
        {
        }

        protected override string GetConnectionName()
        {
            return ConnectionNameMappingSettings.GetConfig().GetConnectionName("PermissionsCenter", "PermissionsCenter");
        }
    }
}
