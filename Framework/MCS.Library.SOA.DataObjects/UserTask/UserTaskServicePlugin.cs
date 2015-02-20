using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.UserTaskPlugin;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects.UserTaskSync
{
    public class UserTaskServicePlugin : IUserTaskOperation
    {
        public static readonly string _TenantCode = "TenantCode";
        public static readonly string _Department = "Department";
        public static readonly string _MailCollector = "EmailCollector";

        /// <summary>
        /// 监听流程事件
        /// </summary>
        /// <param name="eventContainer"></param>
        public void Init(UserTaskOpEventContainer eventContainer)
        {
            eventContainer.SendUserTasks += eventContainer_SendUserTasks;
            eventContainer.DeleteUserTasks += eventContainer_DeleteUserTasks;
            eventContainer.SetUserTasksAccomplished += eventContainer_SetUserTasksAccomplished;
            eventContainer.DeleteUserAccomplishedTasks += eventContainer_DeleteUserAccomplishedTasks;
        }

        /// <summary>
        /// 删除办结任务
        /// </summary>
        /// <param name="tasks">任务列表</param>
        /// <param name="context">上下文数据</param>
        public void eventContainer_DeleteUserAccomplishedTasks(UserTaskCollection tasks, Dictionary<object, object> context)
        {
            ExecuteSyncOperation(tasks, context, (p, f) =>
            {
                UserTaskServicePluginBroker.Instance.DeleteUserAccomplishedTasks(p, f);
            });
        }
        /// <summary>
        /// 设置任务已完成
        /// </summary>
        /// <param name="tasks">任务列表</param>
        /// <param name="context">上下文数据</param>
        public void eventContainer_SetUserTasksAccomplished(UserTaskCollection tasks, Dictionary<object, object> context)
        {
            ExecuteSyncOperation(tasks, context, (p, f) =>
            {
                UserTaskServicePluginBroker.Instance.SetUserTasksAccomplished(p, f);
            });
        }
        /// <summary>
        /// 删除待办
        /// </summary>
        /// <param name="tasks">任务列表</param>
        /// <param name="context">上下文数据</param>
        public void eventContainer_DeleteUserTasks(UserTaskCollection tasks, Dictionary<object, object> context)
        {
            ExecuteSyncOperation(tasks, context, (p, f) =>
            {
                UserTaskServicePluginBroker.Instance.DeleteUserTasks(p, f);
            });
        }

        /// <summary>
        /// 发送待办
        /// </summary>
        /// <param name="tasks">任务列表</param>
        /// <param name="context">上下文数据</param>
        public void eventContainer_SendUserTasks(UserTaskCollection tasks, Dictionary<object, object> context)
        {
            ExecuteSyncOperation(tasks, context, (p, f) =>
            {
                UserTaskServicePluginBroker.Instance.SendUserTasks(p, f);
            });
        }
        /// <summary>
        /// 执行同步操作
        /// </summary>
        /// <param name="tasks"></param>
        /// <param name="context"></param>
        /// <param name="operation"></param>
        public void ExecuteSyncOperation(UserTaskCollection tasks, Dictionary<object, object> context, Action<string, DictionaryEntry[]> operation)
        {
            if (null == tasks || tasks.Count == 0)
            {
                return;
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            Dictionary<string, string> result = new Dictionary<string, string>();

            context.ToList().ForEach(p => result.Add(p.Key.ToString(), p.Value.ToString()));
            result.Add(_TenantCode, GetTenantCode(tasks));
            result.Add(_Department, GetDepartment(tasks));
            result.Add(_MailCollector, GetMailCollector(tasks));

            string seriTasks = serializer.Serialize(tasks);

            operation(seriTasks, ConvertDicToEntry(result));
        }

        /// <summary>
        /// 将字典转换为可传输的Entry
        /// </summary>
        /// <param name="dics">字典</param>
        /// <returns></returns>
        private static DictionaryEntry[] ConvertDicToEntry(Dictionary<string, string> dics)
        {
            List<DictionaryEntry> entries = new List<DictionaryEntry>();

            if (null != dics)
                dics.ForEach(kp => entries.Add(new DictionaryEntry() { Key = kp.Key, Value = kp.Value }));

            return entries.ToArray();
        }

        /// <summary>
        /// 获取从流程中获取租户编码
        /// </summary>
        /// <param name="tasks">待办列表</param>
        /// <returns></returns>
        private static string GetTenantCode(UserTaskCollection tasks)
        {
            string tenantCode = TenantContext.Current.TenantCode;

            if (tenantCode.IsNullOrEmpty())
            {
                tenantCode = GetParameter(_TenantCode, tasks);
            }

            return tenantCode;
        }

        /// <summary>
        /// 从流程中获取部门
        /// </summary>
        /// <param name="tasks">任务列表</param>
        /// <returns>部门</returns>
        private static string GetDepartment(UserTaskCollection tasks)
        {
            return GetParameter(_Department, tasks);
        }

        /// <summary>
        /// 获取邮件相关参数
        /// </summary>
        /// <param name="tasks">任务列表</param>
        /// <returns>客户端传递的邮件参数</returns>
        private static string GetMailCollector(UserTaskCollection tasks)
        {
            return GetParameter(_MailCollector, tasks);
        }

        /// <summary>
        /// 从流程中获取指定的参数信息
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="tasks">任务列表</param>
        /// <returns>值</returns>
        private static string GetParameter(string key, UserTaskCollection tasks)
        {
            string para = string.Empty;
            UserTask task = tasks.FirstOrDefault();

            if (task != null && task.ProcessID.IsNotEmpty())
            {
                IWfProcess process = WfRuntime.GetProcessByProcessID(task.ProcessID);

                para = process.ApplicationRuntimeParameters.GetValue(key, string.Empty);
            }

            return para;
        }
    }
}
