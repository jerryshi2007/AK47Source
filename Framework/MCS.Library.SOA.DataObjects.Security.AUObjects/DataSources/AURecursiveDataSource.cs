using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MCS.Library.Data.Builder;
using MCS.Library.Core;
using System.Data;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.DataSources
{
	[DataObject]
	public static class AURecursiveDataSource
	{
		private static readonly IEnumerable<SCSimpleObject> emptyResult = new SCSimpleObject[0];

		private static readonly string sqlTemplate = @"
DECLARE @time DATETIME

SET @time = {0};

WITH PARENT_OBJS(ID, ParentID, Name, DisplayName, SchemaType, CreateDate, CreatorID, CreatorName, VersionStartTime, VersionEndTime, [Status]) AS
(
	SELECT ID, ParentID, Name, DisplayName, SchemaType, CreateDate, CreatorID, CreatorName, VersionStartTime, VersionEndTime, [Status] FROM SC.[SchemaObjectAndParentView]
	WHERE [Status] = 1 AND [R_Status] = 1 AND VersionStartTime <= @time AND VersionEndTime > @time
		AND R_VersionStartTime <= @time AND R_VersionEndTime > @time AND {1}
	UNION ALL
	SELECT O.ID, O.ParentID, O.Name, O.DisplayName, O.SchemaType, O.CreateDate, O.CreatorID, O.CreatorName, O.VersionStartTime, O.VersionEndTime, O.[Status]
	FROM SC.[SchemaObjectAndParentView] O INNER JOIN PARENT_OBJS P ON O.ID = P.ParentID
	WHERE O.[Status] = 1 AND P.[Status] = 1 AND R_Status = 1 AND O.VersionStartTime <= @time AND O.VersionEndTime > @time
		AND R_VersionStartTime <= @time AND R_VersionEndTime > @time
)
SELECT * FROM PARENT_OBJS
";

		private static string ConnectionName
		{
			get
			{
				return AUCommon.DBConnectionName;
			}
		}

		public static IEnumerable<SCSimpleObject> QueryParents(string unitID, bool includingSelf, DateTime timePoint)
		{
			if (string.IsNullOrEmpty(unitID) == false)
			{
				string timeString = timePoint == DateTime.MinValue ? TSqlBuilder.Instance.DBCurrentTimeFunction : TSqlBuilder.Instance.FormatDateTime(timePoint);

				WhereSqlClauseBuilder where = new WhereSqlClauseBuilder().AppendCondition("ID", unitID);

				string sql = string.Format(sqlTemplate, timeString, where.ToSqlString(TSqlBuilder.Instance));

				DataTable table = DbHelper.RunSqlReturnDS(sql, ConnectionName).Tables[0];
				DataView view = new DataView(table);
				view.Sort = "ID";

				SCSimpleObjectCollection tempParents = new SCSimpleObjectCollection();

				FillAllParents(unitID, view, includingSelf, tempParents);

				tempParents.Reverse();

				return tempParents;
			}
			else
			{
				return emptyResult;
			}
		}

		private static void FillAllParents(string id, DataView view, bool includingSelf, SCSimpleObjectCollection parents)
		{
			int index = view.Find(id);

			if (index >= 0)
			{
				DataRow row = view[index].Row;

				if (includingSelf)
					parents.Add(MapDataRowToSimpleObject(row));

				FillAllParentsRecursively(row["ParentID"].ToString(), view, parents);
			}
		}

		private static void FillAllParentsRecursively(string id, DataView view, SCSimpleObjectCollection parents)
		{
			int index = view.Find(id);

			if (index >= 0)
			{
				DataRow row = view[index].Row;

				parents.Add(MapDataRowToSimpleObject(row));

				FillAllParentsRecursively(row["ParentID"].ToString(), view, parents);
			}
		}

		private static SCSimpleObject MapDataRowToSimpleObject(DataRow row)
		{
			SCSimpleObject data = new SCSimpleObject();

			ORMapping.DataRowToObject(row, data);

			if (data.DisplayName.IsNullOrEmpty())
				data.DisplayName = data.Name;

			return data;
		}
	}
}
