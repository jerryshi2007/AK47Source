using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
	/// <summary>
	/// 访问表达式计算结果的Adapter。
	/// 所有表达式的计算结果都会保存到SC.ConditionCalculateResult表中，这个Adapter主要完成对这个表的读写操作
	/// </summary>
	public class ConditionCalculateResultAdapter
	{
		public static readonly ConditionCalculateResultAdapter Instance = new ConditionCalculateResultAdapter();

		private ConditionCalculateResultAdapter()
		{
		}

		public void Update(string ownerID, IEnumerable<SchemaObjectBase> users)
		{
			ownerID.CheckStringIsNullOrEmpty("ownerID");
			users.NullCheck("users");

			using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
			{
				DbHelper.RunSqlWithTransaction(GetDeleteConditionCalculateResultSql(ownerID), this.GetConnectionName());

				StringBuilder strB = new StringBuilder();

				foreach (SchemaObjectBase user in users)
					DbHelper.RunSql(GetInsertConditionCalculateResultSql(ownerID, user.ID), this.GetConnectionName());

				ProcessProgress.Current.Increment();
				ProcessProgress.Current.Response();
			}
		}

		/// <summary>
		/// 批量更新
		/// </summary>
		/// <param name="ownerID"></param>
		/// <param name="users"></param>
		public void UpdateBatch(string ownerID, IEnumerable<SchemaObjectBase> users)
		{
			ownerID.CheckStringIsNullOrEmpty("ownerID");
			users.NullCheck("users");

			StringBuilder strB = new StringBuilder();

			strB.Append(GetDeleteConditionCalculateResultSql(ownerID));

			foreach (SchemaObjectBase user in users)
			{
				strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

				strB.Append(GetInsertConditionCalculateResultSql(ownerID, user.ID));
			}

			DbHelper.RunSqlWithTransaction(strB.ToString(), this.GetConnectionName());
		}

		public SchemaObjectCollection LoadCurrentUsers(string ownerID)
		{
			ownerID.CheckStringIsNullOrEmpty("ownerID");

			var timePointBuilder = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(DateTime.MinValue);

			WhereSqlClauseBuilder whereBuilder = new WhereSqlClauseBuilder();

			whereBuilder.AppendItem("C.OwnerID", ownerID);

			timePointBuilder.Add(whereBuilder);

			string sql = string.Format(
				"SELECT SC.* FROM {0} SC INNER JOIN SC.ConditionCalculateResult C ON SC.ID = C.UserID WHERE {1}",
				ORMapping.GetMappingInfo(typeof(SchemaObjectBase)).TableName, timePointBuilder.ToSqlString(TSqlBuilder.Instance));

			SchemaObjectCollection result = new SchemaObjectCollection();

			using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
			{
				result.LoadFromDataView(DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0].DefaultView);
			}

			return result;
		}

		protected virtual string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}

		private static string GetDeleteConditionCalculateResultSql(string ownerID)
		{
			return string.Format("DELETE SC.ConditionCalculateResult WHERE OwnerID = {0}",
				TSqlBuilder.Instance.CheckUnicodeQuotationMark(ownerID));
		}

		private static string GetInsertConditionCalculateResultSql(string ownerID, string userID)
		{
			InsertSqlClauseBuilder insertBuilder = new InsertSqlClauseBuilder();

			insertBuilder.AppendItem("OwnerID", ownerID);
			insertBuilder.AppendItem("UserID", userID);

			return string.Format("INSERT INTO SC.ConditionCalculateResult{0}", insertBuilder.ToSqlString(TSqlBuilder.Instance));
		}
	}
}
