using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Schemas.Adapters
{
	/// <summary>
	/// 用于生成快照信息SQL的Builder
	/// </summary>
	public class VersionSnapshotUpdateSqlBuilder : VersionStrategyUpdateSqlBuilder<VersionedSchemaObjectBase>
	{
		private SnapshotModeDefinition _ModeDefinition = SnapshotModeDefinition.IsInSnapshot;
		private string _TableName = null;

		public static readonly VersionSnapshotUpdateSqlBuilder Instance = new VersionSnapshotUpdateSqlBuilder();

		public VersionSnapshotUpdateSqlBuilder()
		{
		}

		public VersionSnapshotUpdateSqlBuilder(SnapshotModeDefinition modeDefinition, string tableName)
		{
			this._ModeDefinition = modeDefinition;
			this._TableName = tableName;
		}

		public string TableName
		{
			get
			{
				return this._TableName;
			}

			set
			{
				this._TableName = value;
			}
		}

		public SnapshotModeDefinition ModeDefinition
		{
			get
			{
				return this._ModeDefinition;
			}

			set
			{
				this._ModeDefinition = value;
			}
		}

		protected override InsertSqlClauseBuilder PrepareInsertSqlBuilder(VersionedSchemaObjectBase obj, ORMappingItemCollection mapping)
		{
			InsertSqlClauseBuilder builder = new InsertSqlClauseBuilder();

			obj.SchemaToSqlClauseBuilder(this._ModeDefinition, builder);

			builder.AppendItem(GetPropertyFieldName("VersionStartTime", mapping), "@currentTime", "=", true);
			builder.AppendItem(GetPropertyFieldName("VersionEndTime", mapping), ConnectionDefine.MaxVersionEndTime);

			if (obj.CreateDate != DateTime.MinValue)
				builder.AppendItem("CreateDate", obj.CreateDate);

			if (OguBase.IsNotNullOrEmpty(obj.Creator))
			{
				builder.AppendItem("CreatorID", obj.Creator.ID);
				builder.AppendItem("CreatorName", obj.Creator.Name);
			}

			return builder;
		}

		protected override string GetTableName(VersionedSchemaObjectBase obj, ORMappingItemCollection mapping)
		{
			string result = this._TableName;

			if (result.IsNullOrEmpty())
				result = obj.Schema.SnapshotTable;

			return result;
		}
	}
}
