using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using MCS.Library.Core;
using AU = MCS.Library.SOA.DataObjects.Security.AUObjects;
using MCS.Library.Data.Builder;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.DataSources
{
	[DataObject]
	public class AUAdminScopeFullMemberDataSource : AUDataViewDataSource
	{
		private string scopeType;
		private string unitID;
		private SchemaObjectBase scope;
		private int deepOption;

		public string DefaultFromClause
		{
			get
			{
				if (TimePointContext.Current.UseCurrentTime)
					return "SC.ItemAndContainerSnapshot_Current C INNER JOIN SC.AUAdminScopeItemSnapshot_Current S ON S.ID = C.ItemID INNER JOIN SC.SchemaMembersSnapshot_Current SM ON SM.MemberID = C.ContainerID INNER JOIN SC.AdminUnitSnapshot_Current AUS ON SM.ContainerID = AUS.ID";
				else
					return "SC.ItemAndContainerSnapshot C INNER JOIN SC.AUAdminScopeItemSnapshot S ON C.ItemID = S.ID INNER JOIN SC.SchemaMembersSnapshot SM ON SM.MemberID = C.ContainerID INNER JOIN SC.AdminUnitSnapshot AUS ON SM.ContainerID = AUS.ID";
			}
		}

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			base.OnBuildQueryCondition(qc);
			switch (deepOption)
			{
				case 1:
					qc.FromClause = this.DefaultFromClause;
					break;
				case 2:
				case 3:
					qc.FromClause = this.MakeFromClause(this.scopeType, GetUnitFullPath(this.unitID));
					break;
				default:
					throw new ArgumentException("传入的参数有误");
			}

			qc.SelectFields = "S.*,AUS.Name AS AU_Name,AUS.ID AS AU_ID";
			BuildCondition(qc);
			if (string.IsNullOrEmpty(qc.OrderByClause))
				qc.OrderByClause = " AUS.Name ASC ";
		}

		private string GetUnitFullPath(string unitID)
		{
			return AUCommon.DoDbProcess(() =>
			{
				var relation = PC.Adapters.SchemaRelationObjectAdapter.Instance.LoadByObjectID(new string[] { unitID }).Where(m => m.ChildSchemaType == AUCommon.SchemaAdminUnit && m.Status == Schemas.SchemaProperties.SchemaObjectStatus.Normal && (m.ParentSchemaType == AUCommon.SchemaAdminUnit || m.ParentSchemaType == AUCommon.SchemaAUSchema)).FirstOrDefault();
				return relation != null ? relation.FullPath : string.Empty;
			});
		}

		private string MakeFromClause(string scopeType, string unitFullPath)
		{
			return this.DefaultFromClause + @" INNER JOIN (" + BuildFrom(unitFullPath, scopeType) + " ) Q ON C.ContainerID = Q.MemberID";
		}

		private string BuildFrom(string fullPath, string scopeType)
		{
			string sql = @"SELECT QM.MemberID FROM SC.SchemaRelationObjectsSnapshot QR INNER JOIN SC.SchemaMembersSnapshot QM ON QR.ObjectID= QM.ContainerID WHERE ";
			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder().NormalFor("QR.Status").NormalFor("QM.Status");
			where.AppendItem("QM.ContainerSchemaType", AUCommon.SchemaAdminUnit);
			where.AppendItem("QM.MemberSchemaType", AUCommon.SchemaAUAdminScope);
			where.AppendItem("QR.FullPath", TSqlBuilder.Instance.EscapeLikeString(fullPath) + "%", "LIKE");
			var time1 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("QR.");
			var time2 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("QM.");

			return sql + new ConnectiveSqlClauseCollection(time1, time2, where).ToSqlString(TSqlBuilder.Instance);
		}

		private void BuildCondition(QueryCondition qc)
		{
			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
			if (this.deepOption == 1)
			{
				builder.AppendCondition("C.ContainerID", this.scope.ID);
			}
			else
			{
				if (deepOption == 2)
				{
					builder.AppendItem("C.ContainerID", this.scope.ID, "<>");
				}
			}
			builder.NormalFor("C.Status").NormalFor("S.Status").NormalFor("SM.Status").NormalFor("AUS.Status");
			var timeBulder1 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("C.");
			var timeBulder2 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("S.");
			var time3 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("AUS.");
			var time4 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("SM.");

			var conditionClause = new ConnectiveSqlClauseCollection(builder, timeBulder1, timeBulder2, time3, time4).ToSqlString(TSqlBuilder.Instance);

			if (string.IsNullOrEmpty(qc.WhereClause) == false)
				qc.WhereClause += " AND (" + conditionClause + ")";
			else
				qc.WhereClause = conditionClause;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="startRowIndex"></param>
		/// <param name="maximumRows"></param>
		/// <param name="scopeType"></param>
		/// <param name="unitID"></param>
		/// <param name="deepOption">1表示当级，2表示包含下级，3表示当级和下级</param>
		/// <param name="where"></param>
		/// <param name="orderBy"></param>
		/// <param name="totalCount"></param>
		/// <returns></returns>
		public DataView Query(int startRowIndex, int maximumRows, string scopeType, string unitID, int deepOption, string where, string orderBy, ref int totalCount)
		{
			(this.scopeType = scopeType).CheckStringIsNullOrEmpty("scopeType");
			(this.unitID = unitID).CheckStringIsNullOrEmpty("unitID");
			this.scope = AU.Adapters.AUSnapshotAdapter.Instance.LoadAUScope(unitID, scopeType, true, DateTime.MinValue).FirstOrDefault();

			if (this.scope == null)
				throw new AUObjectException("未能根据指定的条件找到此管理单元的管理范围");
			this.deepOption = deepOption;
			return base.Query(startRowIndex, maximumRows, where, orderBy, ref totalCount);
		}

		public int GetQueryCount(string scopeType, string unitID, int deepOption, string where, ref int totalCount)
		{
			return base.GetQueryCount(where, ref totalCount);
		}
	}
}
