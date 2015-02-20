using CIIC.HSR.TSP.DataAccess;
using CIIC.HSR.TSP.IoC;
using CIIC.HSR.TSP.WF.Bizlet.Contract;
using CIIC.HSR.TSP.WF.BizObject;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using CIIC.HSR.TSP.WF.Bizlet.Common;
using CIIC.HSR.TSP.WF.Bizlet.Impl;

namespace CIIC.HSR.TSP.WF.PermissionManagerService
{
    /// <summary>
    /// Summary description for TaskPlugin
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class TaskPlugin : System.Web.Services.WebService
    {
        private static readonly string TenantKey = "TenantCode";
        [WebMethod]
        /// <summary>
        /// 查询待办
        /// </summary>
        /// <param name="tenantCode">租户编码</param>
        /// <param name="userId">用户Id</param>
        /// <param name="status">状态</param>
        /// <param name="pageIndex">页序号</param>
        /// <param name="pageSize">也大小</param>
        /// <param name="totalCount">总数量</param>
        /// <returns>符合条件的所有任务</returns>
        public PagedCollection<BizObject.USER_TASKBO_PROCESS> QueryTask(string tenantCode, string userId, UserTaskQueryCondition condition, int pageIndex, int pageSize, int? totalCount = default(int?))
        {
            ServiceFactory sf = new ServiceFactory();
            var readerService = sf.CreateService<ITaskPluginBizlet>();
            return readerService.QueryTask(tenantCode,userId,condition,pageIndex,pageSize,totalCount);
        }

        [WebMethod]
        /// <summary>
        /// 产生代办
        /// <param name="josn">任务数据</param>
        /// <param name="args">额外数据</param>
        public void SendUserTasks(string josn, DictionaryEntry[] args)
        {
            ServiceFactory sf = new ServiceFactory();
            var readerService = sf.CreateService<ITaskPluginBizlet>();
            readerService.SendUserTasks(josn,args);
        }
        [WebMethod]
        /// <summary>
        /// 将任务由代办转为已办
        /// </summary>
        /// <param name="josn">任务</param>
        /// <param name="args">额外数据</param>
        public void SetUserTasksAccomplished(string josn, DictionaryEntry[] args)
        {
            ServiceFactory sf = new ServiceFactory();
            var readerService = sf.CreateService<ITaskPluginBizlet>();
            readerService.SetUserTasksAccomplished(josn, args);
        }
        [WebMethod]
        /// <summary>
        /// 删除代办
        /// </summary>
        /// <param name="josn">任务</param>
        /// <param name="args">流程参数</param>
        public void DeleteUserAccomplishedTasks(string josn, DictionaryEntry[] args)
        {
            ServiceFactory sf = new ServiceFactory();
            var readerService = sf.CreateService<ITaskPluginBizlet>();
            readerService.DeleteUserAccomplishedTasks(josn, args);
        }
        [WebMethod]
        /// <summary>
        /// 删除已办
        /// </summary>
        /// <param name="josn">任务</param>
        /// <param name="args">流程参数</param>
        public void DeleteUserTasks(string josn, DictionaryEntry[] args)
        {
            ServiceFactory sf = new ServiceFactory();
            var readerService = sf.CreateService<ITaskPluginBizlet>();
            readerService.DeleteUserTasks(josn, args);
        }
        [WebMethod]
        /// <summary>
        /// 同步流程状态
        /// </summary>
        /// <param name="json">被序列化的流程数据</param>
        public void SyncProcess(string json)
        {
            ServiceFactory sf = new ServiceFactory();
            var readerService = sf.CreateService<ITaskPluginBizlet>();
            readerService.SyncProcess(json);
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
            iTaskOperator.Context.TenantCode = args.GetDictionaryEntryValue(TenantKey);

            //取得调用参数
            List<UserTask> userListFrom = new List<UserTask>();
            userListFrom = JsonStringToList<UserTask>(josn);

            List<BizObject.USER_TASKBO> userListTo = new List<BizObject.USER_TASKBO>();
            userListTo = ConverToUserTaskBo(userListFrom);

            Dictionary<string, string> dictionaryData = new Dictionary<string, string>();
            dictionaryData = ConverToDictionary(args);

            operation(userListTo, dictionaryData, iTaskOperator);
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
                    userBo.URL = item.Url;
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
    }
}
