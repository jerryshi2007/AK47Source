using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Security;
using System.Transactions;
using MCS.Library.Data;
using System.Data;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
	/// <summary>
	/// 快照表帮助器
	/// </summary>
	public class SCSnapshotHelper
	{
		/// <summary>
		/// <see cref="SCSnapshotHelper"/>的实例，此字段为只读
		/// </summary>
		public static readonly SCSnapshotHelper Instance = new SCSnapshotHelper();

		public void WaitForFullTextUpdate()
		{
			string sql = @"
WHILE fulltextcatalogproperty('SCFullTextIndex','populateStatus')<>0
WAITFOR DELAY '0:0:1'
";
			DbHelper.RunSql(sql, this.GetConnectionName());
		}

		public bool ValidateGroups(DateTime timePoint)
		{
			InSqlClauseBuilder inSql1 = new InSqlClauseBuilder("ChildSchemaType");
			inSql1.AppendItem(SchemaInfo.FilterByCategory("Groups").ToSchemaNames());

			InSqlClauseBuilder inSql2 = new InSqlClauseBuilder("ParentSchemaType");
			inSql2.AppendItem(SchemaInfo.FilterByCategory("Organizations").ToSchemaNames());


			var all = new ConnectiveSqlClauseCollection(inSql1, inSql2, VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint));

			string sql = string.Format(@"SELECT TOP 1 1 from SC.SchemaRelationObjects WHERE ObjectID IN(
SELECT ObjectID FROM SC.SchemaRelationObjects WHERE {0} AND STATUS =1
GROUP by ObjectID
HAVING COUNT(ParentID) <>1 
)", all.ToSqlString(TSqlBuilder.Instance));

			using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
			{
				Database db = DatabaseFactory.Create(context);
				using (var dr = db.ExecuteReader(CommandType.Text, sql))
				{
					if (dr.Read())
					{
						return false;
					}
					else
					{
						return true;
					}
				}
			}

		}

		public DataSet DetectConflict()
		{
			string sql = string.Format(@"
DECLARE @now DATETIME

SET @now = GETDATE();

SELECT * FROM
(SELECT A.*,O.ID AS OID,O.Status AS OStatus FROM SC.SchemaRelationObjectsSnapshot A LEFT JOIN SC.SchemaObjectSnapshot O
ON A.ObjectID = O.ID
WHERE A.STATUS = 1
) Q WHERE Q.OID IS NULL AND Q.ParentID<>'{0}'

SELECT * FROM
(SELECT A.*,O.ID AS OID,O.Status AS OStatus FROM SC.SchemaRelationObjectsSnapshot A LEFT JOIN SC.SchemaObjectSnapshot O
ON A.ParentID = O.ID
WHERE A.STATUS = 1
) Q WHERE Q.OID IS NULL AND Q.ParentID<>'{0}'

SELECT * FROM
(SELECT A.*,O.ID AS OID,O.Status AS OStatus FROM SC.SchemaMembersSnapshot A LEFT JOIN SC.SchemaObjectSnapshot O
ON A.ContainerID = O.ID
WHERE A.STATUS = 1
) Q WHERE Q.OID IS NULL AND Q.ContainerID<>'{0}'

SELECT * FROM
(SELECT A.*,O.ID AS OID,O.Status AS OStatus FROM SC.SchemaMembersSnapshot A LEFT JOIN SC.SchemaObjectSnapshot O
ON A.MemberID = O.ID
WHERE A.STATUS = 1
) Q WHERE Q.OID IS NULL AND Q.ContainerID<>'{0}'

SELECT 
	O.Data.value('Object[1]/@CodeName','nvarchar(255)') as CodeName,
	O.Data.value('Object[1]/@Name','nvarchar(255)') as Name,
	A.Data.value('Object[1]/@ID','nvarchar(255)') as [Object_ID],
	A.Data.value('Object[1]/@ParentID','nvarchar(255)') as Parent_ID,
	A.Data.value('Object[1]/@Default','nvarchar(255)') as ISDefault,
	A.Data.value('Object[1]/@FullPath','nvarchar(255)') as FullPath
FROM SC.SchemaRelationObjects A INNER JOIN (
	SELECT ObjectID FROM SC.SchemaRelationObjects
	WHERE ParentSchemaType='Organizations' AND ChildSchemaType='Users' AND Status = 1 AND VersionStartTime<GETDATE() AND VersionEndTime>GETDATE()
	GROUP BY ObjectID
	HAVING COUNT(ParentID)>0 AND SUM(CASE Data.value('Object[1]/@Default','nvarchar(10)') WHEN 'True' THEN 1 WHEN 'true' THEN 1 ELSE 0 END) >1 
	)B ON A.ObjectID = B.ObjectID INNER JOIN
	SC.SchemaObject O ON A.ObjectID = O.ID

WHERE A.VersionStartTime<GETDATE() AND A.Status = 1 AND A.VersionEndTime>GETDATE() AND O.VersionStartTime<GETDATE() AND O.VersionEndTime>GETDATE() AND ParentSchemaType='Organizations' AND ChildSchemaType='Users'
ORDER BY A.ObjectID
", SCOrganization.RootOrganizationID);
			return DbHelper.RunSqlReturnDS(sql, this.GetConnectionName());
		}

		public void ClearDbErrors()
		{
			var schemaUsers = SchemaInfo.FilterByCategory("Users").ToSchemaNames();

			InSqlClauseBuilder containerSchemaInSql = new InSqlClauseBuilder("ContainerSchemaType");
			containerSchemaInSql.AppendItem(schemaUsers);

			InSqlClauseBuilder memberSchemaInSql = new InSqlClauseBuilder("MemberSchemaType");
			containerSchemaInSql.AppendItem(schemaUsers);

			string sql = string.Format(@"
DELETE FROM SC.SchemaMembers
WHERE ContainerID = MemberID
AND {0}

DELETE FROM SC.SchemaMembersSnapshot
WHERE ContainerID = MemberID
AND {0}

DELETE FROM SC.SchemaMembersSnapshot_Current
WHERE ContainerID = MemberID
AND {0}

", new ConnectiveSqlClauseCollection(containerSchemaInSql, memberSchemaInSql).ToSqlString(TSqlBuilder.Instance));
			DbHelper.RunSql(sql, this.GetConnectionName());
		}

		protected string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}
	}
}
