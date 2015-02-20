using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Builder;
using MCS.Library.Data;
using MCS.Library.Data.Mapping;
using System.Transactions;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Adapters
{
	public class SchemaPropertyExtensionAdapter : UpdatableAdapterBase<SchemaPropertyExtension>
	{
		public static SchemaPropertyExtensionAdapter Instance = new SchemaPropertyExtensionAdapter();

		protected override string GetConnectionName()
		{
			return AUCommon.DBConnectionName;
		}

		public SchemaPropertyExtensionCollection LoadAll()
		{
			string sql = "SELECT * FROM SC.SchemaPropertyExtensions";
			using (DbContext context = DbContext.GetContext(GetConnectionName()))
			{
				Database db = DatabaseFactory.Create(context);

				using (var dr = db.ExecuteReader(System.Data.CommandType.Text, sql))
				{
					SchemaPropertyExtensionCollection result = new SchemaPropertyExtensionCollection();
					while (dr.Read())
					{
						SchemaPropertyExtension obj = new SchemaPropertyExtension(dr["TargetSchemaType"].ToString(), dr["SourceID"].ToString(), dr["Description"].ToString());
						obj.InternalDefinitionXml = dr["Definition"].ToString();
						result.Add(obj);
					}

					return result;
				}
			}
		}

		public SchemaPropertyExtension Load(string targetSchemaType, string sourceID)
		{
			string sql = "SELECT TOP 1 * FROM SC.SchemaPropertyExtensions WHERE ";
			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
			builder.AppendCondition("TargetSchemaType", targetSchemaType);
			builder.AppendCondition("SourceID", sourceID);
			sql += builder.ToSqlString(TSqlBuilder.Instance);

			SchemaPropertyExtension obj = null;

			using (DbContext context = DbContext.GetContext(GetConnectionName()))
			{
				Database db = DatabaseFactory.Create(context);

				using (var dr = db.ExecuteReader(System.Data.CommandType.Text, sql))
				{
					if (dr.Read())
					{
						obj = new SchemaPropertyExtension(dr["TargetSchemaType"].ToString(), dr["SourceID"].ToString(), dr["Description"].ToString());
						obj.InternalDefinitionXml = dr["Definition"].ToString();
					}
				}
			}

			return obj;
		}
	}
}

