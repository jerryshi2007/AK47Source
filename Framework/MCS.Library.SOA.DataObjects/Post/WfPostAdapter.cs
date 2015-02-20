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

namespace MCS.Library.SOA.DataObjects
{
	public class WfPostAdapter : UpdatableAndLoadableAdapterBase<WfPost, WfPostCollection>
	{
		public static readonly WfPostAdapter Instance = new WfPostAdapter();

		private WfPostAdapter()
		{
		}

		public WfPost Load(string postID)
		{
			postID.CheckStringIsNullOrEmpty("postID");

			WfPostCollection posts = base.Load(builder => builder.AppendItem("POST_ID", postID));

			(posts.Count > 0).FalseThrow("不能找到POST_ID为{0}的记录", postID);

			return posts[0];
		}

		public void AddPostUsers(WfPost post, IEnumerable<IUser> users)
		{
			post.NullCheck("post");
			users.NullCheck("users");

			WfPostUserCollection postUsers = new WfPostUserCollection();

			foreach (IUser user in users)
			{
				WfPostUser gu = new WfPostUser();

				gu.PostID = post.PostID;
				gu.User = user;
				postUsers.Add(gu);
			}

			InsertPostUsers(postUsers);
		}

		public void DeletePostUsers(WfPost post, params string[] userIDs)
		{
			post.NullCheck("post");
			userIDs.NullCheck("userIDs");

			InSqlClauseBuilder builder = new InSqlClauseBuilder();

			builder.AppendItem(userIDs);

			if (builder.Count > 0)
			{
				string sql = string.Format("DELETE WF.POST_USERS WHERE POST_ID = {0} AND USER_ID {1}",
				   TSqlBuilder.Instance.CheckQuotationMark(post.PostID,true), builder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

				DbHelper.RunSql(sql, GetConnectionName());
			}
		}

		protected override void AfterInnerDelete(WfPost data, Dictionary<string, object> context)
		{
			string sql = string.Format("DELETE WF.POST_USERS WHERE POST_ID = {0}",
				TSqlBuilder.Instance.CheckQuotationMark(data.PostID, true));

			DbHelper.RunSql(sql, GetConnectionName());
		}

		protected override string GetConnectionName()
		{
			return ConnectionDefine.UserRelativeInfoConnectionName;
		}

		private void InsertPostUsers(WfPostUserCollection postUsers)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				foreach (WfPostUser postUser in postUsers)
				{
					string ignoreFields = "UserID";

					string sql = ORMapping.GetInsertSql(postUser, TSqlBuilder.Instance,ignoreFields);

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
