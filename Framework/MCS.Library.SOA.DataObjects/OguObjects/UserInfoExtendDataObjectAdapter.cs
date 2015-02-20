using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MCS.Library.Data.Builder;
using MCS.Library.OGUPermission;
using MCS.Library.Data.Mapping;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 用户信息扩展数据访问适配器类
	/// </summary>
	public class UserInfoExtendDataObjectAdapter : UpdatableAndLoadableAdapterBase<UserInfoExtendDataObject, UserInfoExtendCollection>
	{
		public static readonly UserInfoExtendDataObjectAdapter Instance = new UserInfoExtendDataObjectAdapter();

		/// <summary>
		/// 构造函数
		/// </summary>
		private UserInfoExtendDataObjectAdapter()
		{

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public UserInfoExtendDataObject Load(string id)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(id, "id");

			UserInfoExtendCollection info = this.Load(builder => builder.AppendItem("ID", id));

			UserInfoExtendDataObject result = null;

			if (info.Count == 0)
			{
				result = new UserInfoExtendDataObject();
				result.ID = id;
			}
			else
				result = info[0];

			return result;
		}

		public UserInfoExtendCollection GetUserInfoExtendInfoCollectionByUsers(IEnumerable<IUser> users)
		{
			ExceptionHelper.FalseThrow(users != null, "用户集合不能为null");

			if (users.Count() == 0)
			{
				return new UserInfoExtendCollection();
			}

			string[] userIDS = new string[users.Count()];
			int i = 0;

			foreach (IUser u in users)
			{
				userIDS[i] = u.ID;
				i++;
			}

			return GetUserInfoExtendInfoCollectionByUsers(userIDS);
		}

		public UserInfoExtendCollection GetUserInfoExtendInfoCollectionByUsers(params string[] userIDS)
		{
			ExceptionHelper.FalseThrow(userIDS != null, "用户列表不能为null");

			InSqlClauseBuilder builder = new InSqlClauseBuilder();

			builder.AppendItem(userIDS);

			UserInfoExtendCollection result = new UserInfoExtendCollection();

			if (builder.Count > 0)
			{
				string sql = string.Format("SELECT * FROM USERS_INFO_EXTEND WHERE ID {0}",
					builder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

				DataTable table = null;
				DbHelper.RunSql(db => table = db.ExecuteDataSet(CommandType.Text, sql).Tables[0], ConnectionDefine.UserRelativeInfoConnectionName);

				result.LoadFromDataView(table.DefaultView);
			}

			return result;
		}

		//比较诡异，屏蔽掉
		/*
		public UserInfoExtendCollection GetUserInfoExtendInfoCollectionByUsers(IEnumerable<IUser> users, string mobileTel)
		{
			if (users.Count() == 0)
			{
				return new UserInfoExtendCollection();
			}
			string[] userIDS = new string[users.Count()];
			int i = 0;
			foreach (IUser u in users)
			{
				userIDS[i] = u.ID;
				i++;
			}

			InSqlClauseBuilder builder = new InSqlClauseBuilder();

			builder.AppendItem(userIDS);

			UserInfoExtendCollection result = new UserInfoExtendCollection();

			if (builder.Count > 0)
			{
				string sql = "";
				if (string.IsNullOrEmpty(mobileTel))
				{
					sql = string.Format("SELECT * FROM USERS_INFO_EXTEND WHERE ID {0}",
						builder.ToSqlStringWithInOperator(TSqlBuilder.Instance));
				}
				else
				{
					sql = string.Format("SELECT * FROM USERS_INFO_EXTEND WHERE ID {0} AND MOBILE LIKE '%{1}%' OR MOBILE2 LIKE '%{1}%' OR OFFICE_TEL LIKE '%{1}%'",
						builder.ToSqlStringWithInOperator(TSqlBuilder.Instance), mobileTel);
				}

				DataTable table = null;
				DbHelper.RunSql(db => table = db.ExecuteDataSet(CommandType.Text, sql).Tables[0], ConnectionDefine.UserRelativeInfoConnectionName);

				result.LoadFromDataView(table.DefaultView);
			}

			return result;
		}
		*/

		protected override string GetConnectionName()
		{
			return ConnectionDefine.UserRelativeInfoConnectionName;
		}
	}
}
