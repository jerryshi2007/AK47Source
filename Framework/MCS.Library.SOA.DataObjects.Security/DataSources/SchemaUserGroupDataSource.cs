using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Builder;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.DataSources
{
	/// <summary>
	/// 提供用户所属群组查询列表数据源
	/// </summary>
	public class SchemaUserGroupDataSource : UserBelongingSchemaDataSource
	{
		protected override string SnapshotTableName
		{
			get { return TimePointContext.Current.UseCurrentTime ? "SC.SchemaGroupSnapshot_Current" : "SC.SchemaGroupSnapshot"; }
		}

		protected override string[] ContainerSchemaTypes
		{
			get { return SchemaInfo.FilterByCategory("Groups").ToSchemaNames(); }
		}

		protected override string[] UserSchemaTypes
		{
			get { return SchemaInfo.FilterByCategory("Users").ToSchemaNames(); }
		}

		protected override string FromClause
		{
			get
			{
				return base.FromClause + (TimePointContext.Current.UseCurrentTime ? " INNER JOIN SC.SchemaRelationObjectsSnapshot_Current OU ON OU.ObjectID = O.ID" : " INNER JOIN SC.SchemaRelationObjectsSnapshot OU ON OU.ObjectID = O.ID");
			}
		}

		protected override string SelectFields
		{
			get
			{
				return "O.*, OU.ParentID AS OUID";
			}
		}

		protected override void OnConnectConditions(ConnectiveSqlClauseCollection allConditions)
		{
			base.OnConnectConditions(allConditions);
			allConditions.Add(VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("OU."));
			allConditions.Add(VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("O."));
			WhereSqlClauseBuilder where1 = new WhereSqlClauseBuilder();
			where1.AppendItem("OU.Status", (int)SchemaObjectStatus.Normal);
			where1.AppendItem("O.Status", (int)SchemaObjectStatus.Normal);
			allConditions.Add(where1);
		}
	}
}
