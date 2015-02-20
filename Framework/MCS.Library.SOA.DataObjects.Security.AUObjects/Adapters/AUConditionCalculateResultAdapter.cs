using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using System.Data;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Adapters
{
	/// <summary>
	/// 访问表达式计算结果的Adapter。
	/// 所有表达式的计算结果都会保存到SC.ConditionCalculateResults表中，这个Adapter主要完成对这个表的读写操作
	/// </summary>
	public class AUConditionCalculateResultAdapter
	{
		public static readonly AUConditionCalculateResultAdapter Instance = new AUConditionCalculateResultAdapter();

		private AUConditionCalculateResultAdapter()
		{
		}

		public void Update(string ownerID, IEnumerable<SchemaObjectBase> objects)
		{
			ownerID.CheckStringIsNullOrEmpty("ownerID");
			objects.NullCheck("objects");

			using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
			{
				DbHelper.RunSqlWithTransaction(GetDeleteConditionCalculateResultSql(ownerID), this.GetConnectionName());

				StringBuilder strB = new StringBuilder();

				foreach (SchemaObjectBase user in objects)
					DbHelper.RunSql(GetInsertConditionCalculateResultSql(ownerID, user.ID), this.GetConnectionName());

				ProcessProgress.Current.Increment();
				ProcessProgress.Current.Response();
			}
		}

		/// <summary>
		/// 批量更新
		/// </summary>
		/// <param name="ownerID"></param>
		/// <param name="objects"></param>
		public void UpdateBatch(string ownerID, IEnumerable<SchemaObjectBase> objects)
		{
			ownerID.CheckStringIsNullOrEmpty("ownerID");
			objects.NullCheck("users");

			StringBuilder strB = new StringBuilder();

			strB.Append(GetDeleteConditionCalculateResultSql(ownerID));

			foreach (SchemaObjectBase user in objects)
			{
				strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

				strB.Append(GetInsertConditionCalculateResultSql(ownerID, user.ID));
			}

			DbHelper.RunSqlWithTransaction(strB.ToString(), this.GetConnectionName());
		}

		public SchemaObjectCollection LoadCurrentObjects<T>(string ownerID) where T : SchemaObjectBase
		{
			ownerID.CheckStringIsNullOrEmpty("ownerID");

			var timePointBuilder = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(DateTime.MinValue);

			WhereSqlClauseBuilder whereBuilder = new WhereSqlClauseBuilder();

			whereBuilder.AppendItem("C.OwnerID", ownerID);

			timePointBuilder.Add(whereBuilder);

			string sql = string.Format(
				"SELECT SC.* FROM {0} SC INNER JOIN SC.ConditionCalculateResults C ON SC.ID = C.ObjectID WHERE {1}",
				ORMapping.GetMappingInfo(typeof(T)).TableName, timePointBuilder.ToSqlString(TSqlBuilder.Instance));

			SchemaObjectCollection result = new SchemaObjectCollection();

			using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
			{
				result.LoadFromDataView(DbHelper.RunSPReturnDS(sql, this.GetConnectionName()).Tables[0].DefaultView);
			}

			return result;
		}

		protected virtual string GetConnectionName()
		{
			return AUCommon.DBConnectionName;
		}

		private static string GetDeleteConditionCalculateResultSql(string ownerID)
		{
			return string.Format("DELETE SC.ConditionCalculateResults WHERE OwnerID = {0}",
				TSqlBuilder.Instance.CheckUnicodeQuotationMark(ownerID));
		}

		private static string GetInsertConditionCalculateResultSql(string ownerID, string objectID)
		{
			InsertSqlClauseBuilder insertBuilder = new InsertSqlClauseBuilder();

			insertBuilder.AppendItem("OwnerID", ownerID);
			insertBuilder.AppendItem("ObjectID", objectID);

			return string.Format("INSERT INTO SC.ConditionCalculateResults{0}", insertBuilder.ToSqlString(TSqlBuilder.Instance));
		}
	}
}
