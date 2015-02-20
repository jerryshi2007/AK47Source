/*
 * created 2012-07-10
 * created by v-weirf
 * 
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.DataSources
{
	public class SchemaRootOrgDataSource : DataViewDataSourceQueryAdapterBase
	{
		public SchemaRootOrgDataSource()
			: base()
		{
		}

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			qc.FromClause = TimePointContext.Current.UseCurrentTime ? "SC.SchemaOrganizationSnapshot_Current O INNER JOIN SC.SchemaRelationObjectsSnapshot_Current R ON O.ID = R.ObjectID" : "SC.SchemaOrganizationSnapshot O INNER JOIN SC.SchemaRelationObjectsSnapshot R ON O.ID = R.ObjectID";
			qc.SelectFields = "O.*";
			if (string.IsNullOrEmpty(qc.OrderByClause))
				qc.OrderByClause = "R.InnerSort DESC";

			qc.WhereClause.IsNotEmpty((s) => qc.WhereClause += " AND ");

			var timeCondition = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("R.");

			var timeCondition2 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("O.");

			var builder = new WhereSqlClauseBuilder();

			builder.AppendItem("R.Status", (int)SchemaObjectStatus.Normal);
			builder.AppendItem("R.ParentID", SCOrganization.RootOrganizationID);
			IConnectiveSqlClause clause = DataSourceUtil.SchemaTypeCondition("R.ChildSchemaType", SchemaInfo.FilterByCategory("Organizations").ToSchemaNames());
			qc.WhereClause += new ConnectiveSqlClauseCollection(builder, timeCondition, timeCondition2, clause).ToSqlString(TSqlBuilder.Instance);
		}

		protected override string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}
	}
}
