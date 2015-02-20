using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Builder;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.DataSources
{
	/// <summary>
	/// 提供导航控件的数据源
	/// </summary>
	public class SchemaNavPathDataSource
	{
		public SchemaNavPathDataSource()
		{
		}

		public System.Collections.Generic.Stack<SchemaNavPathNode> Query(Guid id)
		{
			var timePoint = DateTime.MinValue;
			if (TimePointContext.Current.UseCurrentTime == false)
				timePoint = TimePointContext.Current.SimulatedTime;
			return this.Query(id, DateTime.MinValue);
		}

		public System.Collections.Generic.Stack<SchemaNavPathNode> Query(Guid id, DateTime timePoint)
		{
			System.Collections.Generic.Stack<SchemaNavPathNode> stack = new Stack<SchemaNavPathNode>();

			StringBuilder sb = new StringBuilder();
			var db = DbHelper.GetDBDatabase();
			SchemaNavPathNode node;
			do
			{
				node = null;
				sb.Clear();
				sb.Append(TimePointContext.Current.UseCurrentTime ? @"
SELECT R.ObjectID, R.ParentID, O.ID, O.Name, O.DisplayName,O.SchemaType
FROM SC.SchemaOrganizationSnapshot_Current AS O INNER JOIN SC.SchemaRelationObjects_Current AS R ON O.ID = R.ObjectID
WHERE " : @"
SELECT R.ObjectID, R.ParentID, O.ID, O.Name, O.DisplayName,O.SchemaType
FROM SC.SchemaOrganizationSnapshot AS O INNER JOIN SC.SchemaRelationObjects AS R ON O.ID = R.ObjectID
WHERE ");
				WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

				builder.AppendItem<int>("R.Status", (int)SchemaObjectStatus.Normal);
				builder.AppendItem<string>("R.ParentSchemaType", "Organizations");

				var timeCondition = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "O.");

				builder.AppendItem<int>("O.Status", (int)SchemaObjectStatus.Normal);

				var timeCondition2 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "R.");

				builder.AppendItem<int>("R.Status", (int)SchemaObjectStatus.Normal);
				builder.AppendItem<string>("R.ObjectID", id.ToString());

				var schemaConditon1 = DataSourceUtil.SchemaTypeCondition("R.ParentSchemaType", SchemaInfo.FilterByCategory("Organizations").ToSchemaNames());
				var schemaConditon2 = DataSourceUtil.SchemaTypeCondition("O.SchemaType", SchemaInfo.FilterByCategory("Organizations").ToSchemaNames());
				var schemaConditon3 = DataSourceUtil.SchemaTypeCondition("O.ChildSchemaType", SchemaInfo.FilterByCategory("Organizations").ToSchemaNames());
				sb.Append(new ConnectiveSqlClauseCollection(builder, timeCondition, timeCondition2, schemaConditon1, schemaConditon2, schemaConditon3).ToSqlString(TSqlBuilder.Instance));

				using (var dr = db.ExecuteReader(System.Data.CommandType.Text, sb.ToString()))
				{
					if (dr.Read())
					{
						node = new SchemaNavPathNode()
						{
							ID = dr.GetString(0),
							ParentId = dr.GetString(1),
							Name = dr.GetString(2),
							Description = dr.GetString(3),
							NodeType = dr.GetString(4)
						};

						stack.Push(node);
						id = Guid.Parse(node.ParentId);
					}
				}
			} while (node != null && node.ID != SCOrganization.RootOrganizationID);

			return stack;
		}
	}

	public class SchemaNavPathNode
	{
		public string ID { get; set; }

		public string ParentId { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public string NodeType { get; set; }
	}
}
