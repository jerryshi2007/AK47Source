using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using System.Data;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Security.DataSources
{
	public abstract class ConstBelongingSchemaDataSource : DataViewDataSourceQueryAdapterBase
	{
		private string parentId;
		private string orderBy;
		private string whereClause;

		public ConstBelongingSchemaDataSource()
			: base()
		{
		}

		/// <summary>
		/// 获取左边关系的表名
		/// </summary>
		protected abstract string SnapshotTableName { get; }

		/// <summary>
		/// 获取From字段或表达式
		/// </summary>
		protected virtual string FromClause
		{
			get
			{
				return this.SnapshotTableName + (TimePointContext.Current.UseCurrentTime ? " O INNER JOIN SC.SchemaMembersSnapshot_Current R ON O.ID = R.MemberID" : " O INNER JOIN SC.SchemaMembersSnapshot R ON O.ID = R.MemberID");
			}
		}

		/// <summary>
		/// 获取选择字段
		/// </summary>
		protected virtual string SelectFields
		{
			get { return "O.*"; }
		}

		/// <summary>
		/// 获取缺省的排序字段
		/// </summary>
		protected virtual string DefaultOrderBy
		{
			get { return "R.InnerSort DESC"; }
		}

		/// <summary>
		/// 获取需要对应到查询条件ID的字段名称
		/// </summary>
		protected virtual string ParentIdField
		{
			get { return "R.ContainerID"; }
		}

		/// <summary>
		/// 获取父关系的SchemaType
		/// </summary>
		protected abstract string[] ContainerSchemaTypes { get; }

		/// <summary>
		/// 获取子关系的SchemaType
		/// </summary>
		protected abstract string[] MemberSchemaTypes { get; }

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			qc.FromClause = FromClause;
			qc.SelectFields = SelectFields;

			if (this.orderBy.IsNullOrEmpty())
				qc.OrderByClause = this.DefaultOrderBy;
			else
				qc.OrderByClause = orderBy;

			base.OnBuildQueryCondition(qc);

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			var timeCondition = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("R.");

			var timeCondition2 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("O.");

			builder.AppendItem(ParentIdField, parentId);
			builder.AppendItem<int>("R.Status", (int)SchemaObjectStatus.Normal);
			builder.AppendItem<int>("O.Status", (int)SchemaObjectStatus.Normal);

			InnerBuildWhere(builder);
			var conditionMemberCondition = DataSourceUtil.SchemaTypeCondition("R.MemberSchemaType", this.MemberSchemaTypes);
			var conditionContainerCondition = DataSourceUtil.SchemaTypeCondition("R.ContainerSchemaType", this.ContainerSchemaTypes);

			var allCondition = new ConnectiveSqlClauseCollection(builder, conditionMemberCondition, conditionMemberCondition, timeCondition, timeCondition2);

			if (string.IsNullOrEmpty(qc.WhereClause))
			{
				qc.WhereClause = allCondition.ToSqlString(TSqlBuilder.Instance);
			}
			else
			{
				qc.WhereClause = allCondition.ToSqlString(TSqlBuilder.Instance) + " AND (" + qc.WhereClause + ")";
			}
		}

		protected virtual void InnerBuildWhere(WhereSqlClauseBuilder builder)
		{
			builder.AppendItem<int>("R.Status", (int)SchemaObjectStatus.Normal);
		}

		public DataView Query(string parentId, string where, ref int totalCount, string orderBy, int startRowIndex, int maximumRows)
		{
			this.parentId = parentId;
			this.orderBy = orderBy;
			this.whereClause = where;

			return base.Query(startRowIndex, maximumRows, where, orderBy, ref totalCount);
		}

		public int GetQueryCount(string parentId, string where, ref int totalCount)
		{
			this.parentId = parentId;

			return base.GetQueryCount(where, ref totalCount);
		}

		protected override string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}
	}
}
