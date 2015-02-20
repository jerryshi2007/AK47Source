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
	public class SchemaGroupDataSource : DataViewDataSourceQueryAdapterBase
	{
		private bool fileterRole = false;
		private string[] parentPermissions = null;
		private string userID;

		public SchemaGroupDataSource()
			: base()
		{
		}

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			qc.FromClause = (TimePointContext.Current.UseCurrentTime ?
				"SC.SchemaGroupSnapshot_Current G INNER JOIN SC.SchemaRelationObjectsSnapshot_Current R ON G.ID = R.ObjectID "
				: "SC.SchemaGroupSnapshot G INNER JOIN SC.SchemaRelationObjectsSnapshot R ON G.ID = R.ObjectID ");

			if (this.fileterRole)
			{
				qc.FromClause += TimePointContext.Current.UseCurrentTime ?
					" LEFT JOIN SC.Acl_Current AC ON AC.ContainerID = R.ParentID INNER JOIN SC.SchemaRoleSnapshot_Current RS ON RS.ID = AC.MemberID INNER JOIN SC.UserAndContainerSnapshot_Current UC ON RS.ID = UC.ContainerID " :
					" LEFT JOIN SC.Acl AC ON AC.ContainerID = R.ParentID INNER JOIN SC.SchemaRoleSnapshot RS ON RS.ID = AC.MemberID INNER JOIN SC.UserAndContainerSnapshot UC ON RS.ID = UC.ContainerID ";
			}

			qc.SelectFields = "G.*,R.ParentID";
			if (qc.OrderByClause.IsNullOrEmpty())
				qc.OrderByClause = "R.InnerSort DESC";

			qc.WhereClause.IsNotEmpty((s) => qc.WhereClause += " AND ");

			var timeConditionS = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("G.");

			var timeConditionR = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("R.");

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			builder.AppendItem("R.Status", (int)SchemaObjectStatus.Normal);
			builder.AppendItem("G.Status", (int)SchemaObjectStatus.Normal);

			var allConditions = new ConnectiveSqlClauseCollection(timeConditionS, timeConditionR, builder);

			if (this.fileterRole)
			{
				allConditions.Add(VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("AC."));
				allConditions.Add(VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("RS."));
				allConditions.Add(VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("UC."));
				builder.AppendItem("AC.Status", (int)SchemaObjectStatus.Normal);
				builder.AppendItem("RS.Status", (int)SchemaObjectStatus.Normal);
				builder.AppendItem("UC.Status", (int)SchemaObjectStatus.Normal);
				builder.AppendItem("UC.UserID", this.userID);

				InSqlClauseBuilder inSql1 = new InSqlClauseBuilder("AC.ContainerPermission");
				inSql1.AppendItem(this.parentPermissions);
				allConditions.Add(inSql1);
			}

			qc.WhereClause += allConditions.ToSqlString(TSqlBuilder.Instance);
		}

		protected override string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}

		public DataView Query(int startRowIndex, int maximumRows, string where, string orderBy, string userID, string[] parentPermissions, ref int totalCount)
		{
			this.fileterRole = true;
			this.parentPermissions = parentPermissions;
			this.userID = userID;
			return this.Query(startRowIndex, maximumRows, where, orderBy, ref totalCount);
		}

		public int GetQueryCount(string where, string userID, string[] parentPermissions, ref int totalCount)
		{
			this.fileterRole = true;
			this.parentPermissions = parentPermissions;
			this.userID = userID;
			return this.GetQueryCount(where, ref totalCount);
		}
	}
}
