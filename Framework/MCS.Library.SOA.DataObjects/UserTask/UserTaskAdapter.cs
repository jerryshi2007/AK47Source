using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects
{
	public class UserTaskAdapter
	{
		public static readonly UserTaskAdapter Instance = new UserTaskAdapter();

		private UserTaskAdapter()
		{
		}

		public void SendUserTasks(UserTaskCollection tasks)
		{
			UserOpContext contexts = InitEventContexts();

			contexts.OnBeforeSendUserTasks(tasks);

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				contexts.OnSendUserTasks(tasks);

				scope.Complete();
			}
		}

		public void SetUserTasksAccomplished(UserTaskCollection tasks)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(tasks != null, "tasks");

			UserOpContext contexts = InitEventContexts();

			contexts.OnBeforeSetUserTasksAccomplished(tasks);

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				contexts.OnSetUserTasksAccomplished(tasks);

				scope.Complete();
			}
		}

		public UserTaskCollection LoadUserTasksByActivity(string activityID, Action<InSqlClauseBuilder> action)
		{
			action.NullCheck("action");

			InSqlClauseBuilder builder = new InSqlClauseBuilder();

			action(builder);

			string sql = string.Format("SELECT * FROM WF.USER_TASK WHERE ACTIVITY_ID = {0}",
				TSqlBuilder.Instance.CheckUnicodeQuotationMark(activityID));

			if (builder.Count > 0)
				sql += " AND " + builder.ToSqlStringWithInOperator(TSqlBuilder.Instance);

			UserTaskCollection result = new UserTaskCollection();

			DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

			foreach (DataRow row in table.Rows)
			{
				UserTask task = new UserTask();

				ORMapping.DataRowToObject(row, task);

				result.Add(task);
			}

			return result;
		}

		public UserTaskCollection LoadUserTasks(string tableName, IConnectiveSqlClause builder)
		{
			UserTaskCollection result = new UserTaskCollection();

			if (builder.IsEmpty == false)
			{
				string sql = string.Format("SELECT * FROM {0} WHERE {1}",
					tableName, builder.ToSqlString(TSqlBuilder.Instance));

				DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

				foreach (DataRow row in table.Rows)
				{
					UserTask task = new UserTask();

					ORMapping.DataRowToObject(row, task);

					result.Add(task);
				}
			}

			return result;
		}

		public UserTaskCollection LoadUserTasks(Action<WhereSqlClauseBuilder> action)
		{
			action.NullCheck("action");

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			action(builder);

			string sql = string.Format("SELECT * FROM WF.USER_TASK WHERE {0}", builder.ToSqlString(TSqlBuilder.Instance));

			UserTaskCollection result = new UserTaskCollection();

			DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

			foreach (DataRow row in table.Rows)
			{
				UserTask task = new UserTask();

				ORMapping.DataRowToObject(row, task);

				result.Add(task);
			}

			return result;
		}

		public UserTask LoadSingleUserTaskByID(string taskID)
		{
			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
			builder.AppendItem("TASK_GUID", taskID);

			string sql = string.Format("SELECT TOP (1) * FROM WF.USER_TASK WHERE {0}", builder.ToSqlString(TSqlBuilder.Instance));
			DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

			UserTask userTask = new UserTask();
			if (table.Rows.Count > 0)
				ORMapping.DataRowToObject(table.Rows[0], userTask);

			return userTask;
		}

		/// <summary>
		/// 删除待办事项
		/// </summary>
		/// <param name="action"></param>
		public void DeleteUserTasks(UserTaskCollection tasks)
		{
			tasks.NullCheck("tasks");

			UserOpContext contexts = InitEventContexts();

			contexts.OnBeforeDeleteUserTasks(tasks);

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				contexts.OnDeleteUserTasks(tasks);

				scope.Complete();
			}
		}

		/// <summary>
		/// 删除已办事项
		/// </summary>
		/// <param name="task"></param>
		/// <param name="idType"></param>
		public void DeleteUserAccomplishedTasks(UserTaskCollection tasks)
		{
			tasks.NullCheck("tasks");

			UserOpContext contexts = InitEventContexts();

			contexts.OnBeforeDeleteUserAccomplishedTasks(tasks);

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				contexts.OnDeleteUserAccomplishedTasks(tasks);

				scope.Complete();
			}
		}

		public int UpdateUserTask(UserTask task, UserTaskIDType idType, UserTaskFieldDefine fields)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(task != null, "task");
			int result = 0;

			UserOpContext contexts = InitEventContexts();

			contexts.OnBeforeUpdateUserTask(task, idType, fields);

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				result = contexts.OnUpdateUserTask(task, idType, fields);

				scope.Complete();
			}

			return result;
		}

		#region 其它流程操作
		/// <summary>
		/// 设置待办箱置顶
		/// </summary>
		/// <param name="taskID"></param>
		//为修改个别属性单独做方法，不方便维护和使用，今后最好使用UpdateTask方法来负责更新。不过现在列表
		//的CheckBox只提供一个Key不提供整个对象，暂时先这么凑合吧---2008.04.30 by RexCheng
		public void SetTaskAtTop(params string[] taskID)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(taskID != null, "taskID");

			if (taskID.Length > 0)
			{
				InSqlClauseBuilder inSQL = new InSqlClauseBuilder();
				inSQL.AppendItem(taskID);

				string strSQL = string.Format("UPDATE WF.USER_TASK SET TOP_FLAG = 1 WHERE TASK_GUID IN ({0})", inSQL.ToSqlString(TSqlBuilder.Instance));

				DbHelper.RunSql(strSQL, GetConnectionName());
			}
		}

		/// <summary>
		/// 取消待办箱置顶
		/// </summary>
		/// <param name="taskID"></param>
		public void CancelTaskAtTop(params string[] taskID)
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(null == taskID, "taskID");

			if (taskID.Length > 0)
			{
				InSqlClauseBuilder inSQL = new InSqlClauseBuilder();
				inSQL.AppendItem(taskID);

				string strSQL = string.Format("UPDATE WF.USER_TASK SET TOP_FLAG = 0 WHERE TASK_GUID IN ({0})", inSQL.ToSqlString(TSqlBuilder.Instance));

				DbHelper.RunSql(strSQL, GetConnectionName());
			}
		}

		/// <summary>
		/// 设置消息类别
		/// </summary>
		/// <param name="categoryID">类别ID，若为"NULL"则为无类别</param>
		/// <param name="userTaskIDs">TaskID</param>
		public void SetTaskCategory(string categoryID, params string[] userTaskIDs)
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(null == categoryID, "categoryID");
			ExceptionHelper.TrueThrow<ArgumentNullException>(null == userTaskIDs, "userTaskIDs");

			if (userTaskIDs.Length > 0)
			{
				InSqlClauseBuilder inSQL = new InSqlClauseBuilder();
				inSQL.AppendItem(userTaskIDs);

				//当指定的类别存在或指定NULL时，才对消息进行设置。
				//防止设置类别时该类别已被删除，造成脏数据。
				string strSql = string.Format(
					@"IF (((SELECT COUNT(*) FROM WF.USER_TASK_CATEGORY WHERE CATEGORY_GUID = {0}) = 1) OR ({0} IS NULL))
					BEGIN
						UPDATE WF.USER_TASK SET CATEGORY_GUID = {0} WHERE TASK_GUID IN ({1})
					END",
					TSqlBuilder.Instance.CheckQuotationMark(categoryID, categoryID != "NULL"),
					inSQL.ToSqlString(TSqlBuilder.Instance));

				DbHelper.RunSql(strSql, GetConnectionName());
			}
		}

		/// <summary>
		/// 设置已办箱阅读时间
		/// </summary>
		/// <param name="taskID">已办箱ID</param>
		public void SetAccomplishedTaskReadFlag(string taskID)
		{
			taskID.NullCheck("taskID");

			string sql = "UPDATE WF.USER_ACCOMPLISHED_TASK SET READ_TIME = GETDATE() WHERE TASK_GUID = " + TSqlBuilder.Instance.CheckQuotationMark(taskID, true);

			DbHelper.RunSql(sql, GetConnectionName());
		}

		/// <summary>
		/// 设置待办箱阅读时间
		/// </summary>
		/// <param name="taskID">待办箱ID</param>
		public void SetTaskReadFlag(string taskID)
		{
			taskID.NullCheck("taskID");

			string sql = "UPDATE WF.USER_TASK SET READ_TIME = GETDATE() WHERE TASK_GUID = " + TSqlBuilder.Instance.CheckQuotationMark(taskID, true);

			DbHelper.RunSql(sql, GetConnectionName());
		}
		#endregion

		#region 待办事项和已办事项的查询
		/// <summary>
		/// 获取当前用户待办和待阅数量
		/// </summary>
		/// <param name="userID">用户的ID</param>
		/// <returns>查询结果</returns>
		public UserTaskCount GetUserTaskCount(string userID)
		{
			using (DbContext context = DbHelper.GetDBContext())
			{
				Database db = DatabaseFactory.Create(context);

				DataTable table = db.ExecuteDataSet("WF.QueryUserTaskCount", userID).Tables[0];

				return new UserTaskCount(table);
			}
		}

		/// <summary>
		/// 根据Task的ID获得UserTask对象集合
		/// </summary>
		/// <param name="idType"></param>
		/// <param name="returnFields"></param>
		/// <param name="ids"></param>
		/// <returns></returns>
		public UserTaskCollection GetUserTasks(UserTaskIDType idType, UserTaskFieldDefine returnFields, params string[] ids)
		{
			return GetUserTasks(idType, returnFields, false, ids);
		}

		/// <summary>
		/// 根据Task的ID获得UserTask对象集合
		/// </summary>
		/// <param name="idType"></param>
		/// <param name="returnFields"></param>
		/// <param name="ids"></param>
		/// <returns></returns>
		public UserTaskCollection GetUserTasks(UserTaskIDType idType, UserTaskFieldDefine returnFields, bool nolock, params string[] ids)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(ids != null, "ids");

			UserTaskCollection utc = new UserTaskCollection();

			if (ids.Length > 0)
			{
				ORMappingItem keyItem = GetMappingItemFromIDType(idType);

				InSqlClauseBuilder iBuilder = new InSqlClauseBuilder();

				iBuilder.AppendItem(ids);

				string sql = string.Format("SELECT {0} FROM WF.USER_TASK{1} WHERE {2} {3}",
					GetUserTaskSelectFields(returnFields),
					nolock ? "(NOLOCK)" : string.Empty,
					GetMappingItemFromIDType(idType).DataFieldName,
					iBuilder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

				using (DbContext dbi = DbHelper.GetDBContext(GetConnectionName()))
				{
					Database db = DatabaseFactory.Create(dbi);

					using (IDataReader dr = db.ExecuteReader(CommandType.Text, sql))
					{
						while (dr.Read())
						{
							UserTask ut = new UserTask();

							ORMapping.DataReaderToObject(dr, ut);

							utc.Add(ut);
						}
					}
				}
			}

			return utc;
		}

		public UserTaskCollection GetUserAccomplishedTasks(UserTaskIDType idType, UserTaskFieldDefine returnFields, bool nolock, params string[] ids)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(ids != null, "ids");

			UserTaskCollection utc = new UserTaskCollection();

			if (ids.Length > 0)
			{
				ORMappingItem keyItem = GetMappingItemFromIDType(idType);

				InSqlClauseBuilder iBuilder = new InSqlClauseBuilder();

				iBuilder.AppendItem(ids);

				string sql = string.Format("SELECT {0} FROM WF.USER_ACCOMPLISHED_TASK{1} WHERE {2} {3}",
					GetUserTaskSelectFields(returnFields),
					nolock ? "(NOLOCK)" : string.Empty,
					GetMappingItemFromIDType(idType).DataFieldName,
					iBuilder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

				using (DbContext dbi = DbHelper.GetDBContext(GetConnectionName()))
				{
					Database db = DatabaseFactory.Create(dbi);

					using (IDataReader dr = db.ExecuteReader(CommandType.Text, sql))
					{
						while (dr.Read())
						{
							UserTask ut = new UserTask();

							ORMapping.DataReaderToObject(dr, ut);

							utc.Add(ut);
						}
					}
				}
			}

			return utc;
		}

		private static string GetUserTaskSelectFields(UserTaskFieldDefine returnFields)
		{
			ORMappingItemCollection mappings = ORMapping.GetMappingInfo<UserTask>();

			StringBuilder strB = new StringBuilder();

			if (returnFields != UserTaskFieldDefine.All)
			{
				foreach (EnumItemDescription desp in EnumItemDescriptionAttribute.GetDescriptionList(typeof(UserTaskFieldDefine)))
				{
					UserTaskFieldDefine enumItem = (UserTaskFieldDefine)desp.EnumValue;

					if ((enumItem & returnFields) != UserTaskFieldDefine.None)
					{
						ORMappingItem mappingItem = GetMappingFromPropertyName(enumItem.ToString(), mappings);

						if (mappingItem != null)
						{
							if (strB.Length > 0)
								strB.Append(", ");

							strB.Append(mappingItem.DataFieldName);
						}
					}
				}
			}
			else
				strB.Append("*");

			return strB.ToString();
		}

		private static ORMappingItem GetMappingItemFromIDType(UserTaskIDType idType)
		{
			ORMappingItemCollection mappings = ORMapping.GetMappingInfo<UserTask>();

			ORMappingItem keyItem = GetMappingFromPropertyName(idType.ToString(), mappings);

			ExceptionHelper.FalseThrow(keyItem != null, "不能找到idType为{0}对应的字段", idType.ToString());

			return keyItem;
		}

		private static ORMappingItem GetMappingFromPropertyName(string propertyName, ORMappingItemCollection mappings)
		{
			ORMappingItem result = null;

			foreach (ORMappingItem item in mappings)
			{
				if (item.PropertyName == propertyName)
				{
					result = item;
					break;
				}
			}

			return result;
		}

		/// <summary>
		/// 根据TaskGuid得到UserAccomplishedTask
		/// </summary>
		/// <param name="strTaskGuid"></param>
		/// <returns></returns>
		public UserTaskCollection GetUserAccomplishedTasks(params string[] strTaskGuid)
		{
			UserTaskCollection utc = new UserTaskCollection();

			if (strTaskGuid.Length > 0)
			{

				InSqlClauseBuilder builder = new InSqlClauseBuilder();
				builder.AppendItem(strTaskGuid);

				string strSql = "SELECT * FROM WF.USER_ACCOMPLISHED_TASK WHERE (TASK_GUID " + builder.ToSqlStringWithInOperator(TSqlBuilder.Instance) + ")";
				using (DbContext dbi = DbHelper.GetDBContext(GetConnectionName()))
				{
					Database db = DatabaseFactory.Create(dbi);

					using (IDataReader dr = db.ExecuteReader(CommandType.Text, strSql))
					{
						while (dr.Read())
						{
							UserTask ut = new UserTask();

							ORMapping.DataReaderToObject(dr, ut);

							utc.Add(ut);
						}
					}
				}
			}

			return utc;
		}

		private UserOpContext InitEventContexts()
		{
			UserOpContext contexts = new UserOpContext();

			foreach (IUserTaskOperation op in UserTaskOperationSettings.GetConfig().Operations)
			{
				UserTaskOpEventContainer container = new UserTaskOpEventContainer();

				op.Init(container);
				contexts.EventContainers.Add(container);
			}

			return contexts;
		}

		private static string GetConnectionName()
		{
			return WfRuntime.ProcessContext.SimulationContext.GetConnectionName(WorkflowSettings.GetConfig().ConnectionName);
		}
		#endregion
	}
}
