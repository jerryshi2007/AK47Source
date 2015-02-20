using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Builder;
using MCS.Library.OGUPermission;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using System.Transactions;
using MCS.Library.Data;
using System.Data;

namespace MCS.Library.SOA.DataObjects
{
	public class WfGroupAdapter : UpdatableAndLoadableAdapterBase<WfGroup, WfGroupCollection>
	{
		public static readonly WfGroupAdapter Instance = new WfGroupAdapter();

		private WfGroupAdapter()
		{
		}

		public WfGroup Load(string groupID)
		{
			groupID.CheckStringIsNullOrEmpty("groupID");

			WfGroupCollection groups = Load(build => build.AppendItem("GROUP_ID", groupID));

			(groups.Count > 0).FalseThrow("不能找到GROUP_ID为{0}的群组", groupID);

			return groups[0];
		}

		public bool CheckGroupManager(WfGroup group,IUser manger)
		{
			string sql = string.Format("SELECT * FROM WF.GROUP_USERS WHERE GROUP_ID = {0} AND USER_ID= {1}",
					TSqlBuilder.Instance.CheckQuotationMark(group.GroupID, true), TSqlBuilder.Instance.CheckQuotationMark(manger.ID, true));

			DataSet ds = DbHelper.RunSqlReturnDS(sql, GetConnectionName());
			if (ds == null)
				return false;

			if (ds.Tables.Count == 1 && ds.Tables[0].Rows.Count > 0)
				return true;
			else
				return false;
		}

		public void AddGroupUsers(WfGroup group, IEnumerable<IUser> users)
		{
			group.NullCheck("group");
			users.NullCheck("users");

			WfGroupUserCollection groupUsers = new WfGroupUserCollection();

			foreach (IUser user in users)
			{
				WfGroupUser gu = new WfGroupUser();

				gu.GroupID = group.GroupID;
				gu.User = user;

				groupUsers.Add(gu);
			}

			InsertGroupUsers(groupUsers);
		}

		/// <summary>
		/// 调用本方法，批量删除组内用户
		/// </summary>
		/// <param name="group"></param>
		/// <param name="userIDs"></param>
		public void DeleteGroupUsers(WfGroup group, params string[] userIDs)
		{
			group.NullCheck("group");
			userIDs.NullCheck("userIDs");

			InSqlClauseBuilder builder = new InSqlClauseBuilder();

			builder.AppendItem(userIDs);

			if (builder.Count > 0)
			{
				string sql = string.Format("DELETE WF.GROUP_USERS WHERE GROUP_ID = {0} AND USER_ID {1}",
					TSqlBuilder.Instance.CheckQuotationMark(group.GroupID,true), builder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

				DbHelper.RunSql(sql, GetConnectionName());
			}
		}

		/// <summary>
		/// 在删除Group后触发删除关联用户操作。
		/// </summary>
		/// <param name="data"></param>
		/// <param name="context"></param>
		protected override void AfterInnerDelete(WfGroup data, Dictionary<string, object> context)
		{
			string sql = string.Format("DELETE WF.GROUP_USERS WHERE GROUP_ID = {0}",
				TSqlBuilder.Instance.CheckQuotationMark(data.GroupID, true));

			DbHelper.RunSql(sql, GetConnectionName());
		}

		protected override string GetConnectionName()
		{
			return ConnectionDefine.UserRelativeInfoConnectionName;
		}

		private void InsertGroupUsers(WfGroupUserCollection groupUsers)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				foreach (WfGroupUser groupUser in groupUsers)
				{
					string ingoreUserId = "UserID";
					string sql = ORMapping.GetInsertSql(groupUser, TSqlBuilder.Instance, ingoreUserId);

					try
					{
						DbHelper.RunSql(sql, GetConnectionName());
					}
					catch (System.Data.SqlClient.SqlException ex)
					{
						if (ex.ErrorCode != 2627)
							throw;
					}
				}
				scope.Complete();
			}
		}
	}
}
