using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Schemas.Adapters
{
	/// <summary>
	/// 版本相关的SchemaObject对象SQL生成器
	/// </summary>
	public class VersionSchemaObjectUpdateSqlBuilder : VersionStrategyUpdateSqlBuilder<VersionedSchemaObjectBase>
	{
		public static readonly VersionSchemaObjectUpdateSqlBuilder Instance = new VersionSchemaObjectUpdateSqlBuilder();

		private VersionSchemaObjectUpdateSqlBuilder()
		{
		}

		protected override InsertSqlClauseBuilder PrepareInsertSqlBuilder(VersionedSchemaObjectBase obj, ORMappingItemCollection mapping)
		{
			VersionedSchemaObjectBase schemaObj = (VersionedSchemaObjectBase)obj;

			if (OguBase.IsNullOrEmpty(schemaObj.Creator))
			{
				if (DeluxePrincipal.IsAuthenticated)
					schemaObj.Creator = DeluxeIdentity.CurrentUser;
			}

			InsertSqlClauseBuilder builder = base.PrepareInsertSqlBuilder(obj, mapping);

			builder.AppendItem("Data", obj.ToString());

			return builder;
		}
	}
}
