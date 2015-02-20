using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;
using MCS.Library.Core;
using System.Transactions;
using MCS.Library.Data;
using System.Data.Common;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects
{
    internal class DefaultUserTaskOperationImpl : IUserTaskOperation
    {
        #region IUserTaskOperation Members

        public void Init(UserTaskOpEventContainer eventContainer)
        {
            eventContainer.SendUserTasks += new SendUserTasksDelegete(eventContainer_SendUserTasks);
            eventContainer.UpdateUserTask += new UpdateUserTaskDelegete(eventContainer_UpdateUserTask);
            eventContainer.DeleteUserAccomplishedTasks += new DeleteUserAccomplishedTasksDelegete(eventContainer_DeleteUserAccomplishedTasks);
            eventContainer.DeleteUserTasks += new DeleteUserTasksDelegete(eventContainer_DeleteUserTasks);
            eventContainer.SetUserTasksAccomplished += new SetUserTasksAccomplishedDelegete(eventContainer_SetUserTasksAccomplished);
        }

        #endregion

        #region Event Handler
        private static void eventContainer_SetUserTasksAccomplished(UserTaskCollection tasks, Dictionary<object, object> context)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(tasks != null, "tasks");

            //需要删除的待办的ID
            InSqlClauseBuilder deleteTaskIDs = new InSqlClauseBuilder();

            StringBuilder strB = new StringBuilder();

            ORMappingItemCollection mapping = ORMapping.GetMappingInfo<UserTask>().Clone();

            string userTaskTableName = mapping.TableName;

            mapping.TableName = "WF.USER_ACCOMPLISHED_TASK";

            foreach (UserTask task in tasks)
            {
                UserTask clonedUserTask = GetClonedAccomplishedUserTask(task);

                if (strB.Length > 0)
                    strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

                WhereSqlClauseBuilder builder = GetDeleteAccomplishedUserTaskWhereBuilder(task);

                //删除已办
                strB.AppendFormat("DELETE {0} WHERE {1}",
                    mapping.TableName,
                    builder.ToSqlString(TSqlBuilder.Instance));

                strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

                strB.Append(GetUserTaskInsertSql(clonedUserTask, mapping, "Category", "TopFlag"));

                strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

                deleteTaskIDs.AppendItem(task.TaskID);
            }

            if (strB.Length > 0 && deleteTaskIDs.IsEmpty == false)
            {
                //删除待办
                strB.AppendFormat("DELETE {0} WHERE TASK_GUID {1}",
                    userTaskTableName,
                    deleteTaskIDs.ToSqlStringWithInOperator(TSqlBuilder.Instance));
            }

            if (strB.Length > 0)
                DbHelper.RunSqlWithTransaction(strB.ToString(), GetConnectionName());
        }

        private static void eventContainer_DeleteUserTasks(UserTaskCollection tasks, Dictionary<object, object> context)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(tasks != null, "tasks");

            InSqlClauseBuilder builder = new InSqlClauseBuilder();

            tasks.ForEach(u => builder.AppendItem(u.TaskID));

            if (builder.Count > 0)
            {
                string sql = string.Format("DELETE WF.USER_TASK WHERE TASK_GUID {0}", builder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

                DbHelper.RunSql(sql, GetConnectionName());
            }
        }

        private static void eventContainer_DeleteUserAccomplishedTasks(UserTaskCollection tasks, Dictionary<object, object> context)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(tasks != null, "tasks");

            InSqlClauseBuilder builder = new InSqlClauseBuilder();

            tasks.ForEach(u => builder.AppendItem(u.TaskID));

            if (builder.Count > 0)
            {
                string sql = string.Format("DELETE WF.USER_ACCOMPLISHED_TASK WHERE TASK_GUID {0}", builder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

                DbHelper.RunSql(sql, GetConnectionName());
            }
        }

        private static int eventContainer_UpdateUserTask(UserTask task, UserTaskIDType idType, UserTaskFieldDefine fields, Dictionary<object, object> context)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(task != null, "task");

            WhereSqlClauseBuilder wBuilder = GetWhereSqlClauseBuilderByUserTask(task, idType);
            UpdateSqlClauseBuilder uBuilder = GetUpdateSqlClauseBuilderByUserTask(task, fields);

            string sql = string.Format("UPDATE WF.USER_TASK SET {0} WHERE {1}",
                uBuilder.ToSqlString(TSqlBuilder.Instance),
                wBuilder.ToSqlString(TSqlBuilder.Instance));

            return DbHelper.RunSql(sql, GetConnectionName());
        }

        private static void eventContainer_SendUserTasks(UserTaskCollection tasks, Dictionary<object, object> context)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(tasks != null, "tasks");

            if (tasks.Count > 0)
            {
                int blocks = (tasks.Count - 1) / 10 + 1;

                ProcessProgress.Current.MaxStep += blocks;
                ProcessProgress.Current.Response();

                int i = 0;
                while (i < tasks.Count)
                {
                    i = SendBlockUserTasks(tasks, i, 10);

                    ProcessProgress.Current.Increment();
                    ProcessProgress.Current.StatusText = string.Format("保存了{0:#,##0}条待办或通知", i);
                    ProcessProgress.Current.Response();
                }

                ProcessProgress.Current.StatusText = string.Empty;
                ProcessProgress.Current.Response();
            }
        }
        #endregion

        private static int SendBlockUserTasks(UserTaskCollection tasks, int startIndex, int count)
        {
            StringBuilder strB = new StringBuilder(2408);

            int i = startIndex;

            while (i < tasks.Count && i - startIndex < count)
            {
                UserTask userTask = tasks[i];

                if (strB.Length > 0)
                    strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

                strB.Append(GetUserTaskInsertSql(userTask));

                i++;
            }

            if (strB.Length > 0)
                DbHelper.RunSqlWithTransaction(strB.ToString(), GetConnectionName());

            return i;
        }

        internal static WhereSqlClauseBuilder GetWhereSqlClauseBuilderByUserTask(UserTask task, UserTaskIDType idType)
        {
            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
            EnumItemDescriptionList despList = EnumItemDescriptionAttribute.GetDescriptionList(typeof(UserTaskIDType));

            foreach (EnumItemDescription desp in despList)
                if (((UserTaskIDType)desp.EnumValue & idType) != UserTaskIDType.None)
                    builder.AppendItem(desp.Description, GetUserTaskIDValue(task, (UserTaskIDType)desp.EnumValue));

            return builder;
        }

        private static UpdateSqlClauseBuilder GetUpdateSqlClauseBuilderByUserTask(UserTask task, UserTaskFieldDefine fields)
        {
            string[] ignoredProperties =
                            GetIgnorPropertiesByEnum((int)fields, EnumItemDescriptionAttribute.GetDescriptionList(typeof(UserTaskFieldDefine)));

            UpdateSqlClauseBuilder uBuilder = ORMapping.GetUpdateSqlClauseBuilder<UserTask>(task, ignoredProperties);

            return uBuilder.AppendTenantCode();
        }

        internal static string GetUserTaskIDValue(UserTask task, UserTaskIDType idType)
        {
            return (string)task.GetType().GetProperty(idType.ToString()).GetValue(task, null);
        }

        private static string[] GetIgnorPropertiesByEnum(int field, EnumItemDescriptionList despList)
        {
            List<string> result = new List<string>();

            foreach (EnumItemDescription desp in despList)
            {
                if ((field & desp.EnumValue) == 0)
                    result.Add(desp.Name);
            }

            return result.ToArray();
        }

        private static string GetConnectionName()
        {
            return WfRuntime.ProcessContext.SimulationContext.GetConnectionName(WorkflowSettings.GetConfig().ConnectionName);
        }

        private static string GetUserTaskInsertSql(UserTask task, params string[] ignoreProperties)
        {
            return GetUserTaskInsertSql(task, ORMapping.GetMappingInfo<UserTask>(), ignoreProperties);
        }

        private static string GetUserTaskInsertSql(UserTask task, ORMappingItemCollection mapping, params string[] ignoreProperties)
        {
            InsertSqlClauseBuilder builder = ORMapping.GetInsertSqlClauseBuilder(task, mapping, ignoreProperties);

            builder.AppendTenantCode();

            return string.Format("INSERT INTO {0} {1}", mapping.TableName, builder.ToSqlString(TSqlBuilder.Instance));
        }

        /// <summary>
        /// 得到从待办复制的已办的UserTask
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private static UserTask GetClonedAccomplishedUserTask(UserTask task)
        {
            UserTask clonedUserTask = task.Clone();

            string rootProcessID = DictionaryHelper.GetValue(task.Context, "ApprovalRootProcessID", task.ProcessID);

            if (rootProcessID != task.ProcessID)
            {
                clonedUserTask.ActivityID = DictionaryHelper.GetValue(task.Context, "ApprovalRootActivityID", task.ActivityID);
                clonedUserTask.ProcessID = rootProcessID;
            }

            return clonedUserTask;
        }

        private static WhereSqlClauseBuilder GetDeleteAccomplishedUserTaskWhereBuilder(UserTask task)
        {
            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

            builder.AppendItem("SEND_TO_USER", task.SendToUserID);
            builder.AppendItem("STATUS", (int)task.Status);

            string rootProcessID = DictionaryHelper.GetValue(task.Context, "ApprovalRootProcessID", task.ProcessID);

            if (rootProcessID == task.ProcessID)
                builder.AppendItem("PROCESS_ID", task.ProcessID);
            else
                builder.AppendItem("RESOURCE_ID", task.ResourceID);

            return builder;
        }
    }
}
