using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using System.Data;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Security.DataSources
{
	/// <summary>
	/// 用户信息查询列表的DataSource
	/// </summary>
	public class SchemaUserDataSource : DataViewDataSourceQueryAdapterBase
	{
		private bool fileterRole = false;
		private string[] parentPermissions = null;
		private string logonUserID = null;
		private string excludeID = null;
		private bool defaultOnly = false;
		private bool dissociate = false;

		public SchemaUserDataSource()
			: base()
		{
		}

		protected override void OnAfterQuery(DataView result)
		{
			base.OnAfterQuery(result);
			DataSourceUtil.FillUserDefaultParent(result, "ID", "ParentID", this.GetConnectionName());
		}

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			qc.SelectFields = "U.*,ParentID='' ";
			qc.FromClause = TimePointContext.Current.UseCurrentTime ? "SC.SchemaUserSnapshot_Current U " : "SC.SchemaUserSnapshot U ";

			if (qc.OrderByClause.IsNullOrEmpty())
				qc.OrderByClause = "U.VersionStartTime DESC";

			if (this.fileterRole && this.parentPermissions != null)
			{
				var joinClause = TimePointContext.Current.UseCurrentTime ?
					@" INNER JOIN SC.SchemaRelationObjectsSnapshot_Current R ON R.ObjectID = U.ID INNER JOIN SC.Acl_Current AC ON AC.ContainerID = R.ParentID INNER JOIN SC.SchemaRoleSnapshot_Current RS ON RS.ID = AC.MemberID INNER JOIN SC.UserAndContainerSnapshot_Current UC ON RS.ID = UC.ContainerID "
					:
					@" INNER JOIN SC.SchemaRelationObjectsSnapshot R ON R.ObjectID = U.ID INNER JOIN SC.Acl AC ON AC.ContainerID = R.ParentID INNER JOIN SC.SchemaRoleSnapshot RS ON RS.ID = AC.MemberID INNER JOIN SC.UserAndContainerSnapshot UC ON RS.ID = UC.ContainerID ";
				qc.FromClause += joinClause;
			}

			qc.WhereClause.IsNotEmpty((s) => qc.WhereClause += " AND ");

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
			builder.AppendItem("U.Status", (int)SchemaObjectStatus.Normal);

			var timeCondition = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("U.");

			var allConditions = new ConnectiveSqlClauseCollection(builder, timeCondition);

			if (this.fileterRole && this.logonUserID != null && this.parentPermissions != null)
			{
				allConditions.Add(VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("AC."));
				allConditions.Add(VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("RS."));
				allConditions.Add(VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("UC."));
				builder.AppendItem("AC.Status", (int)SchemaObjectStatus.Normal);
				builder.AppendItem("RS.Status", (int)SchemaObjectStatus.Normal);
				builder.AppendItem("UC.Status", (int)SchemaObjectStatus.Normal);
				builder.AppendItem("UC.UserID", this.logonUserID);

				allConditions.Add(DataSourceUtil.SchemaTypeCondition("R.ParentSchemaType", SchemaInfo.FilterByCategory("Organizations").ToSchemaNames()));

				allConditions.Add(VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("R."));
				if (parentPermissions.Length > 0)
				{
					InSqlClauseBuilder inSql1 = new InSqlClauseBuilder("AC.ContainerPermission");
					inSql1.AppendItem(this.parentPermissions);
					allConditions.Add(inSql1);
				}
			}

			if (this.excludeID != null)
			{
				builder.AppendItem("U.ID", this.excludeID, "<>");
			}

			if (this.defaultOnly)
			{
				builder.AppendItem("R.IsDefault", 1);
			}

			if (this.dissociate)
			{
				var timeConditionIn = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("RR.");

				WhereSqlClauseBuilder innerWhere = new WhereSqlClauseBuilder();
				innerWhere.AppendItem("RR.Status", (int)SchemaObjectStatus.Normal);

				InSqlClauseBuilder innerInParent = new InSqlClauseBuilder("RR.ParentSchemaType");
				innerInParent.AppendItem(SchemaInfo.FilterByCategory("Organizations").ToSchemaNames());

				allConditions.Add(new ExistExpressionBuilder(true, string.Format(@"SELECT TOP 1 1 FROM SC.SchemaRelationObjects RR WHERE 
RR.ObjectID = U.ID AND RR.ChildSchemaType = U.SchemaType AND {0}", new ConnectiveSqlClauseCollection(timeConditionIn, innerWhere, innerInParent).ToSqlString(TSqlBuilder.Instance))));
			}

			qc.WhereClause += allConditions.ToSqlString(TSqlBuilder.Instance);
		}

		protected override string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}

		public DataView Query(int startRowIndex, int maximumRows, string where, string orderBy, string logonUserID, string[] parentPermissions, bool defaultOnly, string excludeID, ref int totalCount)
		{
			return Query(startRowIndex, maximumRows, false, where, orderBy, logonUserID, parentPermissions, defaultOnly, excludeID, ref totalCount);
		}

		public DataView Query(int startRowIndex, int maximumRows, bool dissociatedOnly, string where, string orderBy, ref int totalCount)
		{
			this.dissociate = dissociatedOnly;
			return Query(startRowIndex, maximumRows, where, orderBy, ref totalCount);
		}

		public DataView Query(int startRowIndex, int maximumRows, bool dissociatedOnly, string where, string orderBy, string logonUserID, string[] parentPermissions, bool defaultOnly, string excludeID, ref int totalCount)
		{
			this.dissociate = dissociatedOnly;
			this.fileterRole = true;
			this.defaultOnly = defaultOnly;
			this.parentPermissions = parentPermissions;
			this.logonUserID = logonUserID;
			this.excludeID = excludeID;
			return base.Query(startRowIndex, maximumRows, where, orderBy, ref totalCount);
		}

		public int GetQueryCount(string where, bool dissociatedOnly, ref int totalCount)
		{
			this.dissociate = dissociatedOnly;
			return GetQueryCount(where, ref totalCount);
		}

		public int GetQueryCount(string where, bool dissociatedOnly, string[] parentPermissions, string logonUserID, bool defaultOnly, string excludeID, ref int totalCount)
		{
			this.dissociate = dissociatedOnly;
			this.fileterRole = true;
			this.defaultOnly = defaultOnly;
			this.parentPermissions = parentPermissions;
			this.logonUserID = logonUserID;
			this.excludeID = excludeID;
			return base.GetQueryCount(where, ref totalCount);
		}

		public int GetQueryCount(string where, string[] parentPermissions, string logonUserID, bool defaultOnly, string excludeID, ref int totalCount)
		{
			return GetQueryCount(where, false, parentPermissions, logonUserID, defaultOnly, excludeID, ref totalCount);
		}
	}

	public class ExistExpressionBuilder : IConnectiveSqlClause
	{
		private string expression;
		private bool opposite;

		public ExistExpressionBuilder(string expression)
		{
			this.expression = expression;
		}

		public ExistExpressionBuilder(bool opposite, string expression)
			: this(expression)
		{
			this.opposite = opposite;
		}

		public bool IsEmpty
		{
			get { return string.IsNullOrEmpty(this.expression); }
		}

		public string ToSqlString(ISqlBuilder sqlBuilder)
		{
			if (string.IsNullOrEmpty(this.expression))
				return "(1=1)";
			else
				return (opposite ? " NOT " : " ") + "EXISTS(" + this.expression + ")";
		}
	}
}
