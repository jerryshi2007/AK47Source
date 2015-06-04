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
using MCS.Web.Library.Script;

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
            this.ExecuteSyncOperation("DeleteUserAccomplishedTasksService", tasks, context, (p, f) =>
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
            this.ExecuteSyncOperation("SetUserTasksAccomplishedService", tasks, context, (p, f) =>
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
            this.ExecuteSyncOperation("DeleteUserTasksService", tasks, context, (p, f) =>
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
            this.ExecuteSyncOperation("SendUserTasksService", tasks, context, (p, f) =>
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
        private void ExecuteSyncOperation(string opName, UserTaskCollection tasks, Dictionary<object, object> context, Action<string, DictionaryEntry[]> operation)
        {
            if (tasks.Count > 0)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                //获取流程ID以及流程参数
                List<string> listProcessId = new List<string>();

                foreach (var item in tasks)
                {
                    if (!listProcessId.Contains(item.ProcessID))
                    {
                        listProcessId.Add(item.ProcessID);
                    }
                }

                foreach (var item in listProcessId)
                {
                    Dictionary<string, object> resultTemp = new Dictionary<string, object>();

                    context.ToList().ForEach(p => result.Add(p.Key.ToString(), p.Value.ToString()));
                    resultTemp.Add(_TenantCode, GetTenantCode(item));
                    resultTemp.Add(_Department, GetDepartment(item));
                    resultTemp.Add(_MailCollector, GetMailCollector(item));

                    result.Add(item, ConvertDictToEntry(resultTemp));
                }

                //序列化待办信息
                string seriTasks = serializer.Serialize(tasks);

                PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration(
                    opName, () => operation(seriTasks, ConvertDictToEntry(result)));
            }
        }

        /// <summary>
        /// 将字典转换为可传输的Entry
        /// </summary>
        /// <param name="dicts">字典</param>
        /// <returns></returns>
        private static DictionaryEntry[] ConvertDictToEntry(Dictionary<string, object> dicts)
        {
            List<DictionaryEntry> entries = new List<DictionaryEntry>();

            if (null != dicts)
                dicts.ForEach(kp => entries.Add(new DictionaryEntry() { Key = kp.Key, Value = kp.Value }));

            return entries.ToArray();
        }

        /// <summary>
        /// 获取从流程中获取租户编码
        /// </summary>
        /// <param name="tasks">待办列表</param>
        /// <returns></returns>
        private static string GetTenantCode(string processID)
        {
            string tenantCode = TenantContext.Current.TenantCode;

            if (tenantCode.IsNullOrEmpty())
                tenantCode = GetParameter(_TenantCode, processID);

            return tenantCode;
        }

        /// <summary>
        /// 从流程中获取部门
        /// </summary>
        /// <param name="tasks">任务列表</param>
        /// <returns>部门</returns>
        private static string GetDepartment(string processID)
        {
            return GetParameter(_Department, processID);
        }

        /// <summary>
        /// 获取邮件相关参数
        /// </summary>
        /// <param name="tasks">任务列表</param>
        /// <returns>客户端传递的邮件参数</returns>
        private static string GetMailCollector(string processID)
        {
            return GetParameter(_MailCollector, processID);
        }

        /// <summary>
        /// 从流程中获取指定的参数信息
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="tasks">任务列表</param>
        /// <returns>值</returns>
        private static string GetParameter(string key, string processID)
        {
            string para = string.Empty;

            if (!string.IsNullOrEmpty(processID))
            {
                IWfProcess process = WfRuntime.GetProcessByProcessID(processID);

                para = process.ApplicationRuntimeParameters.GetValue(key, string.Empty);
            }

            return para;
        }
    }
}
