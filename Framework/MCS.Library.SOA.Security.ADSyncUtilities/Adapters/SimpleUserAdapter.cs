using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Schemas.Adapters;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace MCS.Library.SOA.Security.ADSyncUtilities.Adapters
{
	internal class SimpleUserAdapter
	{
		public readonly static SimpleUserAdapter Instance = new SimpleUserAdapter();

		private SimpleUserAdapter()
		{
		}

		internal Entity.EntityMappingCollection LoadAllMappings()
		{
			using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
			{
				Entity.EntityMappingCollection result = new Entity.EntityMappingCollection();

				WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();

				string sql = @"SELECT [ID], CodeName, Mail, Sip FROM SC.SchemaUserSnapshot_Current";

				VersionedObjectAdapterHelper.Instance.FillData(sql, this.GetConnectionName(),
					reader =>
					{
						while (reader.Read())
						{
							result.AddNotExistsItem(ReadMapping(reader));
						}
					});

				return result;
			}
		}

		private static Entity.SimpleUser ReadMapping(System.Data.IDataReader reader)
		{
			return new Entity.SimpleUser(reader);
		}

		private string GetConnectionName()
		{
			return ConnectionNameMappingSettings.GetConfig().GetConnectionName("PermissionsCenter", "PermissionsCenter");
		}
	}
}
