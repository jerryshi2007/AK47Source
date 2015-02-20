using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.Security.ADSyncUtilities.Entity;

namespace MCS.Library.SOA.Security.ADSyncUtilities.Adapters
{
	public class IDMappingAdapter : UpdatableAndLoadableAdapterBase<IDMapping, IDMappingCollection>
	{
		public static readonly IDMappingAdapter Instance = new IDMappingAdapter();

		private IDMappingAdapter()
		{
		}

		public int BatchInsert(DataTable dataTable)
		{
			Database db = DatabaseFactory.Create(GetConnectionName());
			SqlCommand cmd = (SqlCommand)db.GetSqlStringCommand(@"INSERT INTO [SC].[PermissionCenter_AD_IDMapping] (SCObjectID,ADObjectGuid,LastSynchronizedVersionTime)
	  VALUES (@SCObjectID,@ADObjectGuid,@LastSynchronizedVersionTime)");

			cmd.Parameters.Add("@SCObjectID", SqlDbType.NVarChar, 36, "SCObjectID");
			cmd.Parameters.Add("@ADObjectGuid", SqlDbType.NVarChar, 36, "ADObjectGuid");
			cmd.Parameters.Add("@LastSynchronizedVersionTime", SqlDbType.DateTime, 8, "LastSynchronizedVersionTime");

			return db.BatchInsert(UpdateBehavior.Standard, dataTable, cmd);
		}

		public int BatchDelete(DataTable dataTable)
		{
			Database db = DatabaseFactory.Create(GetConnectionName());
			InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("SCObjectID");

			int result = 0;
			if (dataTable.Rows.Count > 0)
			{
				foreach (DataRow row in dataTable.Rows)
				{
					inBuilder.AppendItem(row["SCObjectID"]);
				}

				string sqlDelete = string.Format("DELETE FROM [SC].[PermissionCenter_AD_IDMapping] WHERE {0}", inBuilder.ToSqlString(TSqlBuilder.Instance));
				result = db.ExecuteNonQuery(CommandType.Text, sqlDelete);
			}

			return result;
		}

		protected override string GetConnectionName()
		{
			return ConnectionNameMappingSettings.GetConfig().GetConnectionName("PermissionsCenter", "PermissionsCenter");
		}
	}
}