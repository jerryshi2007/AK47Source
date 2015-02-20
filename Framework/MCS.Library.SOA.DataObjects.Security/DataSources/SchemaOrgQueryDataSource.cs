using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Builder;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using System.Data;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Security.DataSources
{
	public class SchemaOrgQueryDataSource : DataViewDataSourceQueryAdapterBase
	{
		bool fileterRole = false;
		string userID = null;
		string[] permissions = null;

		public DataView Query(int startRowIndex, int maximumRows, string where, string orderBy, ref int totalCount, string userID, string[] orgPermissions)
		{
			fileterRole = true;
			this.userID = userID;
			this.permissions = orgPermissions;

			return base.Query(startRowIndex, maximumRows, where, orderBy, ref totalCount);
		}

		public int GetQueryCount(string where, string userID, string[] orgPermissions, ref int totalCount)
		{
			fileterRole = true;
			this.userID = userID;
			this.permissions = orgPermissions;

			return base.GetQueryCount(where, ref totalCount);
		}

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			qc.FromClause = TimePointContext.Current.UseCurrentTime ? "SC.SchemaOrganizationSnapshot_Current O INNER JOIN SC.SchemaRelationObjectsSnapshot_Current R ON O.ID = R.ObjectID" : "SC.SchemaOrganizationSnapshot O INNER JOIN SC.SchemaRelationObjectsSnapshot R ON O.ID = R.ObjectID";

			if (this.fileterRole)
			{
				qc.FromClause += TimePointContext.Current.UseCurrentTime ?
					" INNER JOIN SC.Acl_Current AC ON AC.ContainerID = O.ID INNER JOIN SC.SchemaRoleSnapshot_Current RS ON RS.ID = AC.MemberID INNER JOIN SC.UserAndContainerSnapshot_Current UC ON RS.ID = UC.ContainerID "
					: " INNER JOIN SC.Acl AC ON AC.ContainerID = O.ID INNER JOIN SC.SchemaRoleSnapshot RS ON RS.ID = AC.MemberID INNER JOIN SC.UserAndContainerSnapshot UC ON RS.ID = UC.ContainerID ";
			}

			qc.SelectFields = "DISTINCT O.*,R.FullPath";

			if (string.IsNullOrEmpty(qc.OrderByClause))
				qc.OrderByClause = "R.InnerSort ASC";

			qc.WhereClause.IsNotEmpty((s) => qc.WhereClause += " AND ");

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
			builder.AppendItem("R.Status", (int)SchemaObjectStatus.Normal);
			builder.AppendItem("O.Status", (int)SchemaObjectStatus.Normal);
			var timeCondition = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("R.");
			var timeCondition2 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("O.");
			var schemaCondition = DataSourceUtil.SchemaTypeCondition("R.ChildSchemaType", "Organizations");
			var allConditions = new ConnectiveSqlClauseCollection(builder, timeCondition, timeCondition2, schemaCondition);

			if (this.fileterRole && this.userID != null && this.permissions != null)
			{
				allConditions.Add(VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("AC."));
				allConditions.Add(VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("RS."));
				allConditions.Add(VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("UC."));
				builder.AppendItem("AC.Status", (int)SchemaObjectStatus.Normal);
				builder.AppendItem("RS.Status", (int)SchemaObjectStatus.Normal);
				builder.AppendItem("UC.Status", (int)SchemaObjectStatus.Normal);
				builder.AppendItem("UC.UserID", this.userID);

				InSqlClauseBuilder inSql1 = new InSqlClauseBuilder("AC.ContainerPermission");
				inSql1.AppendItem(this.permissions);
				allConditions.Add(inSql1);
			}

			qc.WhereClause += allConditions.ToSqlString(TSqlBuilder.Instance);
		}

		protected override string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}
	}
}
