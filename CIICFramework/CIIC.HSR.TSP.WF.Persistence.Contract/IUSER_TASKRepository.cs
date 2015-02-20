using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIIC.HSR.TSP.DataAccess;
using CIIC.HSR.TSP.WF.Bizlet.Common;
using CIIC.HSR.TSP.WF.BizObject;

namespace CIIC.HSR.TSP.WF.Persistence.Contract
{
    public partial interface IUSER_TASKRepository
    {

        /// <summary>
        /// 查询代办
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="status">任务状态条件</param>
        /// <returns>代办</returns>
        PagedCollection<BizObject.USER_TASKBO_PROCESS> QueryTask(string tenantCode, string sendToUserId, UserTaskQueryCondition condition, int pageIndex, int pageSize, int? totalCount = default(int?));

        /// <summary>
        ///  查询代办
        /// </summary>
        /// <param name="tenantCode">租户编码</param>
        /// <param name="sendToUserId">Id</param>
        /// <param name="condition">查询条件</param>
        /// <param name="topIndex">件数</param>
        /// <returns>待办信息</returns>
        List<USER_TASKBO_TOPUNPROCESS> QueryTaskUnProcessedTop(string tenantCode, string sendToUserId, UserTaskQueryCondition condition, int topIndex);

        /// <summary>
        /// 将任务由代办转为已办
        /// </summary>
        /// <param name="tasks">任务</param>
        /// <param name="data">额外数据</param>
        void SetUserTasksAccomplished(List<CIIC.HSR.TSP.WF.BizObject.USER_TASKBO> tasks, Dictionary<string, string> data, string tenantCode);

        /// <summary>
        /// 删除代办
        /// </summary>
        /// <param name="tasks">任务</param>
        /// <param name="data">流程参数</param>
         void DeleteUserAccomplishedTasks(List<CIIC.HSR.TSP.WF.BizObject.USER_TASKBO> tasks, Dictionary<string, string> data);

         /// <summary>
         /// 删除已办
         /// </summary>
         /// <param name="tasks">任务</param>
         /// <param name="data">流程参数</param>
         void DeleteUserTasks(List<CIIC.HSR.TSP.WF.BizObject.USER_TASKBO> tasks, Dictionary<string, string> context);

        /// <summary>
        /// 同步流程状态
        /// </summary>
        /// <param name="json">被序列化的流程数据</param>
        void SyncProcess(List<ProcessBO> process);
    }
}
