using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Security.Conditions;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
	public class HistoryAdapter
	{
		protected virtual string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}

		/// <summary>
		/// 获取与指定对象关联的条件历史
		/// </summary>
		/// <param name="id">对象ID</param>
		/// <param name="type">类型，如果为<see langword="null"/>则不限。</param>
		/// <returns></returns>
		public IEnumerable<SCCondition> GetConditionHistoryEntries(string id, string type)
		{
			SCConditionCollection entries = new SCConditionCollection();
			using (var context = DbHelper.GetDBContext(this.GetConnectionName()))
			{
				Database db = DatabaseFactory.Create(context);

				WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
				builder.AppendItem("OwnerID", id);

				if (type != null)
				{
					builder.AppendItem("Type", type);
				}

				var sql = @"SELECT *  FROM [SC].[Conditions] WHERE " + builder.ToSqlString(TSqlBuilder.Instance) + " ORDER BY [VersionEndTime] DESC, SortID ASC";
				var cmd = db.GetSqlStringCommand(sql);
				using (var dr = db.ExecuteReader(cmd))
				{
					ORMapping.DataReaderToCollection(entries, dr);
				}

				return entries;
			}
		}

		public IEnumerable<HistoryEntry> GetObjectHistoryEntries(string objId)
		{
			HistoryEntryCollection list = new HistoryEntryCollection();

			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();

			where.AppendItem("ID", objId);

			var sql = @"SELECT [Name],[DisplayName],[VersionStartTime], [VersionEndTime],[Status]
FROM [SC].[SchemaObjectSnapshot] WHERE " + where.ToSqlString(TSqlBuilder.Instance) + " ORDER BY VersionStartTime ASC";

			using (var context = DbHelper.GetDBContext(this.GetConnectionName()))
			{
				Database db = DatabaseFactory.Create(context);

				var cmd = db.GetSqlStringCommand(sql);
				using (var dr = db.ExecuteReader(cmd))
				{
					ORMapping.DataReaderToCollection(list, dr);
				}
			}

			return list;
		}

		public IEnumerable<ReferenceHistoryEntry> GetReferenceHistoryEntries(string objId)
		{
			ReferenceHistoryEntryCollection list = new ReferenceHistoryEntryCollection();

			WhereSqlClauseBuilder where1 = new WhereSqlClauseBuilder();
			where1.AppendItem("R.ObjectID", objId);

			WhereSqlClauseBuilder where2 = new WhereSqlClauseBuilder();
			where2.AppendItem("R.ParentID", objId);

			var sql = string.Format(@"SELECT R.[ParentID], R.[ObjectID], R.[VersionStartTime], R.[VersionEndTime], R.[Status], R.[IsDefault], R.[InnerSort], R.[CreatorID], R.[CreatorName],O.Name, O.DisplayName
FROM [SC].[SchemaRelationObjectsSnapshot] R LEFT JOIN SC.SchemaObjectSnapshot_Current O ON R.ParentID = O.ID
WHERE O.VersionStartTime <=R.VersionStartTime AND (O.VersionEndTime IS NULL OR O.VersionEndTime>=R.VersionEndTime) AND {0}
UNION
SELECT R.[ParentID], R.[ObjectID], R.[VersionStartTime], R.[VersionEndTime], R.[Status], R.[IsDefault], R.[InnerSort], R.[CreatorID], R.[CreatorName],O.Name, O.DisplayName
FROM [SC].[SchemaRelationObjectsSnapshot] R LEFT JOIN SC.SchemaObjectSnapshot_Current O ON R.ObjectID = O.ID
WHERE O.VersionStartTime <=R.VersionStartTime AND (O.VersionEndTime IS NULL OR O.VersionEndTime>=R.VersionEndTime) AND {1} ORDER BY VersionStartTime ASC
", where1.ToSqlString(TSqlBuilder.Instance), where2.ToSqlString(TSqlBuilder.Instance));

			using (var context = DbHelper.GetDBContext(this.GetConnectionName()))
			{
				Database db = DatabaseFactory.Create(context);

				var cmd = db.GetSqlStringCommand(sql);
				using (var dr = db.ExecuteReader(cmd))
				{
					ORMapping.DataReaderToCollection(list, dr);
				}
			}

			return list;
		}

		public IEnumerable<MembershipHistoryEntry> GetMembershipHistoryEntries(string objId)
		{
			MembershipHistoryEntryCollection list = new MembershipHistoryEntryCollection();

			WhereSqlClauseBuilder where1 = new WhereSqlClauseBuilder();
			where1.AppendItem("MemberID", objId);
			WhereSqlClauseBuilder where2 = new WhereSqlClauseBuilder();
			where2.AppendItem("ContainerID", objId);

			var sql = string.Format(@"SELECT R.[ContainerID], R.[MemberID], R.[VersionStartTime], R.[VersionEndTime], R.[Status], R.[InnerSort], R.[CreatorID], R.[CreatorName],O.Name,O.DisplayName
FROM [SC].[SchemaMembersSnapshot] R LEFT JOIN SC.SchemaObjectSnapshot_Current O ON R.ContainerID = O.ID
WHERE O.VersionStartTime <=R.VersionStartTime AND (O.VersionEndTime IS NULL OR O.VersionEndTime>=R.VersionEndTime) AND {0}
UNION
SELECT R.[ContainerID], R.[MemberID], R.[VersionStartTime], R.[VersionEndTime], R.[Status], R.[InnerSort], R.[CreatorID], R.[CreatorName],O.Name,O.DisplayName
FROM [SC].[SchemaMembersSnapshot] R LEFT JOIN SC.SchemaObjectSnapshot_Current O ON R.MemberID = O.ID
WHERE O.VersionStartTime <=R.VersionStartTime AND (O.VersionEndTime IS NULL OR O.VersionEndTime>=R.VersionEndTime) AND {1}
ORDER BY VersionStartTime ASC
", where1.ToSqlString(TSqlBuilder.Instance), where2.ToSqlString(TSqlBuilder.Instance));

			using (var context = DbHelper.GetDBContext(this.GetConnectionName()))
			{
				Database db = DatabaseFactory.Create(context);

				var cmd = db.GetSqlStringCommand(sql);
				using (var dr = db.ExecuteReader(cmd))
				{
					ORMapping.DataReaderToCollection(list, dr);
				}
			}

			return list;
		}
	}
}
