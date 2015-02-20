using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Builder;
using MCS.Library.Core;
using System.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using System.Reflection;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 流程当前信息的实现
	/// </summary>
	public class WfProcessCurrentInfoAdapter
	{
		public static readonly WfProcessCurrentInfoAdapter Instance = new WfProcessCurrentInfoAdapter();

		private WfProcessCurrentInfoAdapter()
		{
		}

		/// <summary>
		/// 加载流程运行时的信息
		/// </summary>
		/// <param name="processIDs"></param>
		/// <returns></returns>
		public WfProcessCurrentInfoCollection Load(bool fillAssignees, params string[] processIDs)
		{
			processIDs.NullCheck("processIDs");

			WfProcessCurrentInfoCollection result = new WfProcessCurrentInfoCollection();

			InSqlClauseBuilder builder = new InSqlClauseBuilder();
			builder.AppendItem(processIDs);

			if (builder.Count > 0)
			{
				string fieldNames = ORMapping.GetSelectFieldsNameSql<WfProcessInstanceData>("Data");

				string sql = string.Format("SELECT {0} FROM WF.PROCESS_INSTANCES WHERE INSTANCE_ID {1}",
					fieldNames,
					builder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

				DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

				result.LoadFromDataView(table.DefaultView);

				if (fillAssignees)
					FillAssignees(result);
			}

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fillAssignees"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public WfProcessCurrentInfoCollection Load(bool fillAssignees, Action<WhereSqlClauseBuilder> action)
		{
			action.NullCheck("action");

			WfProcessCurrentInfoCollection result = new WfProcessCurrentInfoCollection();

			WhereSqlClauseBuilder whereBuilder = new WhereSqlClauseBuilder();

			action(whereBuilder);

			string fieldNames = ORMapping.GetSelectFieldsNameSql<WfProcessInstanceData>("Data");

			string sql = string.Format("SELECT {0} FROM WF.PROCESS_INSTANCES WHERE {1}",
					fieldNames,
					whereBuilder.ToSqlString(TSqlBuilder.Instance));

			DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

			result.LoadFromDataView(table.DefaultView);

			if (fillAssignees)
				FillAssignees(result);

			return result;
		}

		/// <summary>
		/// 加载根流程的流程实例
		/// </summary>
		/// <param name="resourceIDs">资源ID</param>
		/// <returns></returns>
		public WfProcessCurrentInfoCollection Load(params string[] resourceIDs)
		{
			resourceIDs.NullCheck("resourceIDs");

			WfProcessCurrentInfoCollection result = new WfProcessCurrentInfoCollection();

			InSqlClauseBuilder builder = new InSqlClauseBuilder();
			builder.AppendItem(resourceIDs);

			if (builder.Count > 0)
			{
				string fieldNames = ORMapping.GetSelectFieldsNameSql<WfProcessInstanceData>("Data");

				string sql = string.Format("SELECT {0} FROM WF.PROCESS_INSTANCES WHERE RESOURCE_ID {1} AND OWNER_ACTIVITY_ID IS NULL",
					fieldNames,
					builder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

				DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

				result.LoadFromDataView(table.DefaultView);

				FillAssignees(result);
			}

			return result;
		}

		public WfProcessCurrentInfoCollection LoadByProcessID(params string[] processIDs)
		{
			processIDs.NullCheck("processIDs");

			WfProcessCurrentInfoCollection result = new WfProcessCurrentInfoCollection();

			InSqlClauseBuilder builder = new InSqlClauseBuilder();
			builder.AppendItem(processIDs);

			if (builder.Count > 0)
			{
				string fieldNames = ORMapping.GetSelectFieldsNameSql<WfProcessInstanceData>("Data");

				string sql = string.Format("SELECT {0} FROM WF.PROCESS_INSTANCES WHERE INSTANCE_ID {1}",
					fieldNames,
					builder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

				DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

				result.LoadFromDataView(table.DefaultView);

				FillAssignees(result);
			}

			return result;
		}

		/// <summary>
		/// 加载分支流程信息
		/// </summary>
		/// <param name="fillAssignees"></param>
		/// <param name="activityID"></param>
		/// <param name="templateKey"></param>
		/// <param name="includeAborted">是否包含已经作废的流程</param>
		/// <returns></returns>
		public WfProcessCurrentInfoCollection LoadByOwnerActivityID(bool fillAssignees, string activityID, string templateKey, bool includeAborted)
		{
			activityID.NullCheck("activityID");

			WfProcessCurrentInfoCollection result = new WfProcessCurrentInfoCollection();

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
			builder.AppendItem("OWNER_ACTIVITY_ID", activityID);

			if (templateKey.IsNotEmpty())
				builder.AppendItem("OWNER_TEMPLATE_KEY", templateKey);

			if (includeAborted == false)
				builder.AppendItem("Status", WfProcessStatus.Aborted.ToString(), "<>");

			string fieldNames = ORMapping.GetSelectFieldsNameSql<WfProcessCurrentInfo>();

			string sql = string.Format("SELECT {0} FROM WF.PROCESS_INSTANCES WHERE {1}",
				fieldNames,
				builder.ToSqlString(TSqlBuilder.Instance));

			DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

			result.LoadFromDataView(table.DefaultView);

			if (fillAssignees)
				FillAssignees(result);

			return result;
		}

		/// <summary>
		/// 得到某个活动下的分支流程统计信息
		/// </summary>
		/// <param name="activityID">不可以为空</param>
		/// <param name="templateKey">可以为空，如果有值，则只统计某个模板的流程</param>
		/// <returns></returns>
		public Dictionary<WfProcessStatus, int> LoadStatisticsDataByOwnerActivityID(string activityID, string templateKey)
		{
			activityID.NullCheck("activityID");

			Dictionary<WfProcessStatus, int> result = new Dictionary<WfProcessStatus, int>();

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
			builder.AppendItem("OWNER_ACTIVITY_ID", activityID);

			if (templateKey.IsNotEmpty())
				builder.AppendItem("OWNER_TEMPLATE_KEY", templateKey);

			string fieldNames = ORMapping.GetSelectFieldsNameSql<WfProcessCurrentInfo>();

			string sql = string.Format("SELECT STATUS, COUNT(*) AS COUNT FROM WF.PROCESS_INSTANCES WHERE {0} GROUP BY STATUS",
				builder.ToSqlString(TSqlBuilder.Instance));

			DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

			foreach (DataRow row in table.Rows)
			{
				string statusText = row["STATUS"].ToString();

				WfProcessStatus status;

				if (Enum.TryParse(statusText, true, out status))
					result[status] = (int)row["COUNT"];
			}

			return result;
		}

		/// <summary>
		/// 更新流程运行时活动节点的Assignees
		/// </summary>
		/// <param name="processCurrentInfo"></param>
		public void UpdateProcessRelatedUsers(IWfProcess process)
		{
			WfProcessCurrentAssigneeCollection pcas = new WfProcessCurrentAssigneeCollection();

			foreach (var activity in process.Activities)
			{
				foreach (var assignee in activity.Assignees)
				{
					if (assignee.User != null)
					{
						WfProcessCurrentAssignee pca = new WfProcessCurrentAssignee(assignee);

						pca.ProcessID = process.ID;
						pca.ActivityID = activity.ID;

						pcas.Add(pca);
					}
				}

				foreach (var candidate in activity.Candidates)
				{
					if (candidate.User != null && activity.Assignees.Contains(candidate.User.ID) == false)
					{
						WfProcessCurrentAssignee pca = new WfProcessCurrentAssignee(candidate);

						pca.ProcessID = process.ID;
						pca.ActivityID = activity.ID;

						pcas.Add(pca);
					}
				}
			}

			WfProcessCurrentAssigneeAdapter.Instance.Update(process.ID, pcas);
		}

		/// <summary>
		/// 删除流程运行时活动节点的Assignees
		/// </summary>
		/// <param name="activityID"></param>
		public void DeleteProcessCurrentAssignees(WfProcessCurrentInfoCollection processesInfo)
		{
			InSqlClauseBuilder builder = new InSqlClauseBuilder("ACTIVITY_ID");

			processesInfo.ForEach(p => builder.AppendItem(p.CurrentActivityID));

			if (builder.Count > 0)
			{
				string sql = string.Format("DELETE WF.PROCESS_CURRENT_ASSIGNEES WHERE {0}",
					builder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

				DbHelper.RunSql(sql, GetConnectionName());
			}
		}

		/// <summary>
		/// 查询用户相关的运行中（包含维护中和未运行的）的流程信息，包括该流程最新的待办信息
		/// </summary>
		/// <param name="userID"></param>
		/// <returns>返回是一个DataTable</returns>
		public DataTable QueryUserRelativeRunningProcesses(string userID)
		{
			userID.CheckStringIsNullOrEmpty("userID");

			return DbHelper.RunSqlReturnDS(string.Format("WF.QueryUserRelativeRunningProcesses {0}",
				TSqlBuilder.Instance.CheckUnicodeQuotationMark(userID)), GetConnectionName()).Tables[0];

			//string sql = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(),
			//    "MCS.Library.SOA.DataObjects.Workflow.DataObjects.UserRelativeRunningProcesses.sql");

			//sql = string.Format(sql, TSqlBuilder.Instance.CheckUnicodeQuotationMark(userID));

			//DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

			//List<string> processIDs = GetNeedToQueryTaskProcessIDs(table);

			////填充待办信息
			//List<string> notMatchProcessIDs = FillLastestTaskInfoToDataTable(table, "WF.USER_TASK", processIDs, true);

			////填充已办信息
			//FillLastestTaskInfoToDataTable(table, "WF.USER_ACCOMPLISHED_TASK", notMatchProcessIDs, false);

			//return table;
		}

		//private static List<string> FillLastestTaskInfoToDataTable(DataTable table, string tableName, List<string> processIDs, bool isTask)
		//{
		//    Dictionary<string, DateTime> taskKeys = QueryLastestUserTaskKeys(tableName, processIDs);
		//    Dictionary<string, UserTask> tasks = QueryUserTasksByTaskKeys(tableName, taskKeys);

		//    return FillTaskInfoToDataTable(table, tasks, isTask);
		//}

		//private static Dictionary<string, UserTask> QueryUserTasksByTaskKeys(string tableName, Dictionary<string, DateTime> taskKeys)
		//{
		//    ConnectiveSqlClauseCollection connective = new ConnectiveSqlClauseCollection(LogicOperatorDefine.Or);

		//    foreach (KeyValuePair<string, DateTime> kp in taskKeys)
		//    {
		//        WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

		//        builder.AppendItem("PROCESS_ID", kp.Key);
		//        builder.AppendItem("DELIVER_TIME", kp.Value);

		//        connective.Add(builder);
		//    }

		//    UserTaskCollection tasks = UserTaskAdapter.Instance.LoadUserTasks(tableName, connective);

		//    Dictionary<string, UserTask> result = new Dictionary<string, UserTask>();

		//    foreach (UserTask task in tasks)
		//        result[task.ProcessID] = task;

		//    return result;
		//}

		//private static List<string> FillTaskInfoToDataTable(DataTable table, Dictionary<string, UserTask> tasks, bool isTask)
		//{
		//    List<string> notMatchProcessIDs = new List<string>();

		//    foreach (DataRow row in table.Rows)
		//    {
		//        UserTask task = null;

		//        string processID = row["INSTANCE_ID"].ToString();

		//        //未填充过待办信息
		//        if (row["LASTEST_TASK_ID"].ToString().IsNullOrEmpty())
		//        {
		//            if (tasks.TryGetValue(processID, out task))
		//            {
		//                row["TASK_TITLE"] = task.TaskTitle;
		//                row["LASTEST_TASK_ID"] = task.TaskID;
		//                row["IS_TASK"] = isTask ? 1 : 0;
		//            }
		//            else
		//            {
		//                notMatchProcessIDs.Add(processID);
		//            }
		//        }
		//    }

		//    return notMatchProcessIDs;
		//}

		//private static Dictionary<string, DateTime> QueryLastestUserTaskKeys(string tableName, List<string> processIDs)
		//{
		//    InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("PROCESS_ID");

		//    inBuilder.AppendItem(processIDs.ToArray());

		//    Dictionary<string, DateTime> result = new Dictionary<string, DateTime>();

		//    if (inBuilder.IsEmpty == false)
		//    {
		//        string sql = "SELECT PROCESS_ID, MAX(DELIVER_TIME) AS MAX_DELIVER_TIME FROM {0} WHERE {1} GROUP BY PROCESS_ID";
		//        sql = string.Format(sql, tableName, inBuilder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

		//        DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

		//        foreach (DataRow row in table.Rows)
		//            result[row["PROCESS_ID"].ToString()] = (DateTime)row["MAX_DELIVER_TIME"];
		//    }

		//    return result;
		//}

		///// <summary>
		///// 得到需要被查询待办的流程ID集合
		///// </summary>
		///// <param name="table"></param>
		///// <returns></returns>
		//private List<string> GetNeedToQueryTaskProcessIDs(DataTable table)
		//{
		//    List<string> result = new List<string>();

		//    foreach (DataRow row in table.Rows)
		//    {
		//        if (row["LASTEST_TASK_ID"].ToString().IsNullOrEmpty())
		//            result.Add(row["INSTANCE_ID"].ToString());
		//    }

		//    return result;
		//}

		private static void FillAssignees(WfProcessCurrentInfoCollection result)
		{
			InSqlClauseBuilder inBuilder = new InSqlClauseBuilder();

			result.ForEach(currentInfo => inBuilder.AppendItem(currentInfo.CurrentActivityID));

			if (inBuilder.Count > 0)
			{
				string sql = string.Format("SELECT * FROM WF.PROCESS_CURRENT_ASSIGNEES WHERE ACTIVITY_ID {0} ORDER BY ID",
				inBuilder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

				DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

				DataView assigneeView = new DataView(table);

				assigneeView.Sort = "ACTIVITY_ID";

				foreach (var currentInfo in result)
				{
					DataRowView[] rows = assigneeView.FindRows(currentInfo.CurrentActivityID);

					Array.ForEach(rows, drv =>
					{
						currentInfo.Assignees.Add(DataRowToAssignee(drv.Row));
					});
				}
			}
		}

		private static WfAssignee DataRowToAssignee(DataRow row)
		{
			WfAssignee result = new WfAssignee();

			result.AssigneeType = (WfAssigneeType)Enum.Parse(typeof(WfAssigneeType), row["ASSIGNEE_TYPE"].ToString(), true);

			OguUser user = new OguUser();
			user.ID = row["USER_ID"].ToString();
			user.Name = row["USER_NAME"].ToString();
			user.FullPath = row["USER_PATH"].ToString();

			result.User = user;

			return result;
		}

		private static string GetConnectionName()
		{
			return WfRuntime.ProcessContext.SimulationContext.GetConnectionName(WorkflowSettings.GetConfig().ConnectionName);
		}
	}
}
