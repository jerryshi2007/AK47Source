using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using System.DirectoryServices;
using System.Transactions;
using MCS.Library.Data;

namespace MCS.Library.Accredit
{
	public static class AccreditHelper
	{
		public static DataRow FindGroup(string fullPath)
		{
			fullPath.CheckStringIsNullOrEmpty("fullPath");

			string sql = string.Format("SELECT * FROM GROUPS WHERE ALL_PATH_NAME = {0}",
				TSqlBuilder.Instance.CheckUnicodeQuotationMark(fullPath));

			DataTable table = null;

			DbHelper.ExecSql(db => table = db.ExecuteDataSet(CommandType.Text, sql).Tables[0]);

			DataRow result = null;

			if (table.Rows.Count > 0)
				result = table.Rows[0];

			return result;
		}

		public static void SyncGroupMembers(string sourceDN, string destinationPath, ServerInfo serverInfo)
		{
			serverInfo.NullCheck("serverInfo");

			ADHelper helper = ADHelper.GetInstance(serverInfo);

			helper.EntryExists(sourceDN).FalseThrow("{0}在域中不存在");
			DataRow groupRow = FindGroup(destinationPath);

			(groupRow != null).FalseThrow("{0}在机构人员管理系统中不存在", destinationPath);

			StringBuilder strB = new StringBuilder(256);

			string groupID = groupRow["GUID"].ToString();

			strB.AppendFormat("DELETE GROUP_USERS WHERE GROUP_GUID = {0}", TSqlBuilder.Instance.CheckUnicodeQuotationMark(groupID));

			int index = 0;

			using (DirectoryEntry groupEntry = helper.NewEntry(sourceDN))
			{
				IEnumerable<DataRow> rows = CollectGroupUserRows(groupEntry);

				foreach (DataRow row in rows)
				{
					strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

					InsertSqlClauseBuilder builder = new InsertSqlClauseBuilder();

					builder.AppendItem("GROUP_GUID", groupID);
					builder.AppendItem("USER_GUID", row["USER_GUID"].ToString());
					builder.AppendItem("USER_PARENT_GUID", row["PARENT_GUID"].ToString());
					builder.AppendItem("INNER_SORT", string.Format("{0:000000}", index));
					builder.AppendItem("CREATE_TIME", "GETDATE()", "=", true);
					builder.AppendItem("MODIFY_TIME", "GETDATE()", "=", true);

					strB.AppendFormat("INSERT INTO GROUP_USERS{0}", builder.ToSqlString(TSqlBuilder.Instance));

					index++;
				}
			}

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				DbHelper.ExecSql(db => db.ExecuteNonQuery(CommandType.Text, strB.ToString()));

				scope.Complete();
			}
		}

		/// <summary>
		/// 获取AD组内的人员在安全中心中的人员信息
		/// </summary>
		/// <param name="groupEntry"></param>
		/// <returns></returns>
		private static IEnumerable<DataRow> CollectGroupUserRows(DirectoryEntry groupEntry)
		{
			InSqlClauseBuilder builder = new InSqlClauseBuilder();

			ADHelper helper = ADHelper.GetInstance();

			helper.EnumGroupMembers(groupEntry, sr =>
			{
				builder.AppendItem(helper.GetSearchResultPropertyStrValue("samAccountName", sr));
			});

			List<DataRow> result = new List<DataRow>(256);

			if (builder.Count > 0)
			{
				string sql = string.Format("SELECT OU_USERS.* FROM OU_USERS INNER JOIN USERS ON USER_GUID = GUID WHERE SIDELINE = 0 AND LOGON_NAME {0} ORDER BY GLOBAL_SORT",
					builder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

				DataTable table = null;

				DbHelper.ExecSql(db => table = db.ExecuteDataSet(CommandType.Text, sql).Tables[0]);

				foreach (DataRow row in table.Rows)
					result.Add(row);
			}

			return result;
		}
	}
}
