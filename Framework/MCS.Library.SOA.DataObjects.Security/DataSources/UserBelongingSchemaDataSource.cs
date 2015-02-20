using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Builder;
using System.Data;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Security.DataSources
{
	public abstract class UserBelongingSchemaDataSource : DataViewDataSourceQueryAdapterBase
	{
		string userId;
		private string orderBy;
		private string whereClause;
		public UserBelongingSchemaDataSource()
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
				return this.SnapshotTableName + (TimePointContext.Current.UseCurrentTime ? " O INNER JOIN SC.UserAndContainerSnapshot_Current R ON O.ID = R.ContainerID" : " O INNER JOIN SC.UserAndContainerSnapshot R ON O.ID = R.ContainerID");
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
		protected virtual string DefaultOrderBy { get { return "R.VersionStartTime DESC"; } }

		/// <summary>
		/// 获取需要对应到查询条件ID的字段名称
		/// </summary>
		protected virtual string UserIdField
		{
			get { return "R.UserID"; }
		}

		/// <summary>
		/// 获取父关系的SchemaType
		/// </summary>
		protected abstract string[] ContainerSchemaTypes { get; }

		/// <summary>
		/// 获取子关系的SchemaType
		/// </summary>
		protected abstract string[] UserSchemaTypes { get; }

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			qc.FromClause = FromClause;
			qc.SelectFields = SelectFields;
			if (string.IsNullOrEmpty(this.orderBy))
				qc.OrderByClause = this.DefaultOrderBy;
			else
				qc.OrderByClause = orderBy;
			base.OnBuildQueryCondition(qc);

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			var timeCondition = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("R.");
			var timeCondition2 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("O.");

			builder.AppendItem(UserIdField, userId);

			InnerBuildWhere(builder);

			var conditionUserSchemas = DataSourceUtil.SchemaTypeCondition("R.UserSchemaType", this.UserSchemaTypes);
			var conditionContainerSchemas = DataSourceUtil.SchemaTypeCondition("R.ContainerSchemaType", this.ContainerSchemaTypes);

			var allConditions = new ConnectiveSqlClauseCollection(timeCondition, timeCondition2, conditionUserSchemas, conditionContainerSchemas, builder);

			OnConnectConditions(allConditions);

			if (string.IsNullOrEmpty(qc.WhereClause))
			{
				qc.WhereClause = allConditions.ToSqlString(TSqlBuilder.Instance);
			}
			else
			{
				//将builder中的内容与WhereClause合并
				qc.WhereClause = allConditions.ToSqlString(TSqlBuilder.Instance) + " AND (" + qc.WhereClause + ")";
			}
		}

		protected virtual void OnConnectConditions(ConnectiveSqlClauseCollection allConditions)
		{
		}

		protected virtual void InnerBuildWhere(WhereSqlClauseBuilder builder)
		{
			builder.AppendItem<int>("R.Status", (int)SchemaObjectStatus.Normal);
		}

		public DataView Query(string userId, string where, ref int totalCount, string orderBy, int startRowIndex, int maximumRows)
		{
			this.userId = userId;
			this.orderBy = orderBy;
			this.whereClause = where;
			return base.Query(startRowIndex, maximumRows, where, orderBy, ref totalCount);
		}

		public int GetQueryCount(string userId, string where, ref int totalCount)
		{
			this.userId = userId;
			return base.GetQueryCount(where, ref totalCount);
		}

		protected override string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}
	}
}
