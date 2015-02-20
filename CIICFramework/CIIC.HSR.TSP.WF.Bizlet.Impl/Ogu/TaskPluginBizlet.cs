using CIIC.HSR.TSP.IoC;
using CIIC.HSR.TSP.WF.Bizlet.Contract;
using CIIC.HSR.TSP.WF.BizObject;
using CIIC.HSR.TSP.WF.BizObject.Exchange;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIIC.HSR.TSP.WF.Bizlet.Common;
using CIIC.HSR.TSP.WF.Bizlet.Impl.Cache;
using CIIC.HSR.TSP.AN.Bizlet.Impl;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl.Ogu
{
    public class TaskPluginBizlet : ITaskPluginBizlet
    {
        public DataAccess.PagedCollection<BizObject.USER_TASKBO_PROCESS> QueryTask(string tenantCode, string userId, BizObject.UserTaskQueryCondition condition, int pageIndex, int pageSize, int? totalCount = default(int?))
        {
            IServiceFactory serviceFactory = Containers.Global.Singleton.Resolve<IServiceFactory>();
            ITaskQuery iTaskQuery = serviceFactory.CreateService<ITaskQuery>();
            iTaskQuery.Context.TenantCode = tenantCode;
            //调用方法
            var retResult = iTaskQuery.QueryTask(userId, condition, pageIndex, pageSize, totalCount);

            return retResult; 
        }
        public DataAccess.PagedCollection<USER_TASKBO_PROCESS> GetTaskFromCache(string cacheKey, string userId, string tenantCode)
        {
            bool isTenandMode=TenantFilterSection.Instance.IsTenantMode;
            return TaskCachePool.Instance.GetCacheData<DataAccess.PagedCollection<USER_TASKBO_PROCESS>>
                (cacheKey, userId, tenantCode, isTenandMode);
        }

        public T GetTaskFromCache<T>(string cacheKey, string userId, string tenantCode) where T:class
        {
            bool isTenandMode = TenantFilterSection.Instance.IsTenantMode;
            return TaskCachePool.Instance.GetCacheData<DataAccess.PagedCollection<USER_TASKBO_PROCESS>>
                (cacheKey, userId, tenantCode, isTenandMode) as T;
        }

        /// <summary>
        /// 查询首页待办
        /// </summary>
        /// <param name="tenantCode">租户编码</param>
        /// <param name="sendToUserId">用户</param>
        /// <param name="condition">条件</param>
        /// <param name="topIndex">件数</param>
        /// <returns>符合条件的所有任务<</returns>
        public List<USER_TASKBO_TOPUNPROCESS> QueryTaskUnProcessedTop(string tenantCode, string sendToUserId, UserTaskQueryCondition condition, int topIndex)
        {
            IServiceFactory serviceFactory = Containers.Global.Singleton.Resolve<IServiceFactory>();
            ITaskQuery iTaskQuery = serviceFactory.CreateService<ITaskQuery>();
            iTaskQuery.Context.TenantCode = tenantCode;
            //调用方法
            var retResult = iTaskQuery.QueryTaskUnProcessedTop(sendToUserId, condition, topIndex);

            return retResult;
        
        }

        public void SendUserTasks(string josn, System.Collections.DictionaryEntry[] args)
        {
            ExecuteSyncOperation(josn, args, (p, t, i) =>
            {
                SendTaskAlarm(p, t);
                i.SendUserTasks(p, t);
            });
        }

        public void SetUserTasksAccomplished(string josn, System.Collections.DictionaryEntry[] args)
        {
            ExecuteSyncOperation(josn, args, (p, t, i) =>
            {
                i.SetUserTasksAccomplished(p, t);
            });
        }

        public void DeleteUserAccomplishedTasks(string josn, System.Collections.DictionaryEntry[] args)
        {
            ExecuteSyncOperation(josn, args, (p, t, i) =>
            {
                i.DeleteUserAccomplishedTasks(p, t);
            });
        }

        public void DeleteUserTasks(string josn, System.Collections.DictionaryEntry[] args)
        {
            ExecuteSyncOperation(josn, args, (p, t, i) =>
            {
                i.DeleteUserTasks(p, t);
            });
        }

        public void SyncProcess(string json)
        {
            IServiceFactory serviceFactory = Containers.Global.Singleton.Resolve<IServiceFactory>();
            ITaskOperator iTaskOperator = serviceFactory.CreateService<ITaskOperator>();
            //取得调用参数
            List<ProcessBO> processBOListFrom = new List<ProcessBO>();
            processBOListFrom = JsonStringToList<ProcessBO>(json);

            if (null != processBOListFrom)
            {
                processBOListFrom.ForEach(p =>
                {
                    if (p.Status.Equals("Completed", StringComparison.CurrentCultureIgnoreCase))
                    {
                        SendCompletionAlarm(p);
                    }
                    iTaskOperator.Context.TenantCode = p.TenantCode;
                    iTaskOperator.SyncProcess(new List<ProcessBO>() { p });
                });
            }
        }
        /// <summary>
        /// 发送待办邮件
        /// </summary>
        /// <param name="tasks">待办列表</param>
        /// <param name="args">客户端的预警信息</param>
        private void SendTaskAlarm(List<USER_TASKBO> tasks,Dictionary<string, string> args)
        { 
            if(!args.ContainsKey(Consts.EmailCollector)||string.IsNullOrEmpty(args[Consts.EmailCollector]))
            {
                return;
            }

            MailCollector collector =JsonConvert.DeserializeObject<MailCollector>(args[Consts.EmailCollector]);
            MailTaskProcessor processor = new MailTaskProcessor(collector, tasks);
            processor.Send();
        }
        /// <summary>
        /// 发送流程完成预警
        /// </summary>
        /// <param name="process">流程对象，其中含有预警信息</param>
        private void SendCompletionAlarm(ProcessBO process)
        {
            if (string.IsNullOrEmpty(process.EmailCollector))
            {
                return;
            }

            MailCollector collector = JsonConvert.DeserializeObject<MailCollector>(process.EmailCollector);
            MailCompletionProcessor processor = new MailCompletionProcessor(collector, process);
            processor.Send();
        }
        /// <summary>
        /// 执行同步任务操作
        /// </summary>
        /// <param name="josn">被序列化的任务列表</param>
        /// <param name="args">额外参数</param>
        /// <param name="operation">执行的同步命令</param>
        private void ExecuteSyncOperation(string josn, DictionaryEntry[] args, Action<List<USER_TASKBO>, Dictionary<string, string>, ITaskOperator> operation)
        {
            IServiceFactory serviceFactory = Containers.Global.Singleton.Resolve<IServiceFactory>();
            ITaskOperator iTaskOperator = serviceFactory.CreateService<ITaskOperator>();
            iTaskOperator.Context.TenantCode = args.GetDictionaryEntryValue(Consts.TenantCode);

            //取得调用参数
            List<UserTask> userListFrom = new List<UserTask>();
            userListFrom = JsonStringToList<UserTask>(josn);

            List<BizObject.USER_TASKBO> userListTo = new List<BizObject.USER_TASKBO>();
            userListTo = ConverToUserTaskBo(userListFrom);

            Dictionary<string, string> dictionaryData = new Dictionary<string, string>();
            dictionaryData = ConverToDictionary(args);

            operation(userListTo, dictionaryData, iTaskOperator);
            ExpireTask(userListTo, iTaskOperator.Context.TenantCode);
        }

        private void ExpireTask(List<BizObject.USER_TASKBO> users,string tenandCode)
        {
            if (null != users)
            {
                users.ForEach(p =>
                {
                    if (!string.IsNullOrEmpty(p.SEND_TO_USER))
                    {
                        TaskCachePool.Instance.RefreshCacheData(p.SEND_TO_USER, tenandCode, TenantFilterSection.Instance.IsTenantMode);
                    }
                });
            }
        }

        private List<T> JsonStringToList<T>(string json)
        {
            List<T> listObjs = JsonConvert.DeserializeObject<List<T>>(json);

            return listObjs;

        }
        private Dictionary<string, string> ConverToDictionary(DictionaryEntry[] arg)
        {
            Dictionary<string, string> dictionaryData = new Dictionary<string, string>();

            if (arg != null)
            {
                foreach (var data in arg)
                {
                    string strKey = "";
                    string strValue = "";
                    //取得Key
                    if (data.Key != null)
                    {
                        strKey = data.Key.ToString();
                    }
                    //取得Value
                    if (data.Value != null)
                    {
                        strValue = data.Value.ToString();
                    }

                    dictionaryData.Add(strKey, strValue);
                }

            }
            return dictionaryData;
        }

        private List<BizObject.USER_TASKBO> ConverToUserTaskBo(List<UserTask> tasks)
        {

            List<BizObject.USER_TASKBO> retUserList = new List<USER_TASKBO>();

            if (tasks != null)
            {
                foreach (var item in tasks)
                {

                    BizObject.USER_TASKBO userBo = new USER_TASKBO();
                    userBo.TASK_GUID = item.TaskID;
                    userBo.APPLICATION_NAME = item.ApplicationName;
                    userBo.PROGRAM_NAME = item.ProgramName;
                    userBo.TASK_LEVEL = item.Level;
                    userBo.TASK_TITLE = item.TaskTitle;
                    userBo.RESOURCE_ID = item.ResourceID;
                    userBo.PROCESS_ID = item.ProcessID;
                    userBo.ACTIVITY_ID = item.ActivityID;
                    userBo.URL = ChangeUrlTemp(item.Url);
                    userBo.DATA = item.Body;
                    userBo.EMERGENCY = item.Emergency;
                    userBo.PURPOSE = item.Purpose;
                    userBo.STATUS = item.Status;
                    userBo.TASK_START_TIME = item.TaskStartTime;
                    userBo.EXPIRE_TIME = item.ExpireTime;
                    userBo.SOURCE_ID = item.SourceID;
                    userBo.SOURCE_NAME = item.SourceName;
                    userBo.SEND_TO_USER = item.SendToUserID;
                    userBo.SEND_TO_USER_NAME = item.SendToUserName;
                    userBo.READ_TIME = item.ReadTime;
                    if (item.Category != null && !string.IsNullOrEmpty(item.Category.CategoryID))
                    {
                        userBo.CATEGORY_GUID = item.Category.CategoryID;
                    }
                    userBo.TOP_FLAG = item.TopFlag;
                    userBo.DRAFT_DEPARTMENT_NAME = item.DraftDepartmentName;
                    userBo.DELIVER_TIME = item.DeliverTime;
                    userBo.DRAFT_USER_ID = item.DraftUserID;
                    userBo.DRAFT_USER_NAME = item.DraftUserName;
                    retUserList.Add(userBo);
                }

            }
            return retUserList;
        }

        private string ChangeUrlTemp(string strUrl)
        {
            int retResultIndex = strUrl.IndexOf("?");
            string retUrl = strUrl.Substring(0, retResultIndex + 1)
                            + strUrl.Substring(retResultIndex + 1).Replace("?", "&");
            return retUrl;
        }
            
        public ServiceContext Context
        {
            get;
            set;
        }


        
    }
}
