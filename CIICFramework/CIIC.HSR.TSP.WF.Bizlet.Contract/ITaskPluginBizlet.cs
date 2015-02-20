using CIIC.HSR.TSP.DataAccess;
using CIIC.HSR.TSP.WF.BizObject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Contract
{
    public interface ITaskPluginBizlet : IRuntime
    {
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
        PagedCollection<BizObject.USER_TASKBO_PROCESS> QueryTask(string tenantCode, string userId, UserTaskQueryCondition condition, int pageIndex, int pageSize, int? totalCount = default(int?));


        /// <summary>
        /// 查询首页待办
        /// </summary>
        /// <param name="tenantCode">租户编码</param>
        /// <param name="sendToUserId">用户</param>
        /// <param name="condition">条件</param>
        /// <param name="topIndex">件数</param>
        /// <returns>符合条件的所有任务<</returns>
        List<USER_TASKBO_TOPUNPROCESS> QueryTaskUnProcessedTop(string tenantCode, string sendToUserId, UserTaskQueryCondition condition, int topIndex);

        /// <summary>
        /// 缓存接口
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="userId">用户Id</param>
        /// <param name="tenantCode">租户编码</param>
        /// <returns></returns>
        DataAccess.PagedCollection<BizObject.USER_TASKBO_PROCESS> GetTaskFromCache(string cacheKey, string userId, string tenantCode);
        /// <summary>
        /// 从缓存中获取待办的通用方法
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="userId">用户Id</param>
        /// <param name="tenantCode">租户编码</param>
        /// <returns>被缓存的业务数据</returns>
        T GetTaskFromCache<T>(string cacheKey, string userId, string tenantCode) where T : class;
        /// <summary>
        /// 产生代办
        /// <param name="josn">任务数据</param>
        /// <param name="args">额外数据</param>
        void SendUserTasks(string josn, DictionaryEntry[] args);
        /// <summary>
        /// 将任务由代办转为已办
        /// </summary>
        /// <param name="josn">任务</param>
        /// <param name="args">额外数据</param>
        void SetUserTasksAccomplished(string josn, DictionaryEntry[] args);
        /// <summary>
        /// 删除代办
        /// </summary>
        /// <param name="josn">任务</param>
        /// <param name="args">流程参数</param>
        void DeleteUserAccomplishedTasks(string josn, DictionaryEntry[] args);
        /// <summary>
        /// 删除已办
        /// </summary>
        /// <param name="josn">任务</param>
        /// <param name="args">流程参数</param>
        void DeleteUserTasks(string josn, DictionaryEntry[] args);
        /// <summary>
        /// 同步流程状态
        /// </summary>
        /// <param name="json">被序列化的流程数据</param>
        void SyncProcess(string json);
    }
}
