using CIIC.HSR.TSP.DataAccess;
using CIIC.HSR.TSP.WF.Bizlet.Common;
using CIIC.HSR.TSP.WF.BizObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIIC.HSR.TSP.WF.Bizlet.Contract
{
    /// <summary>
    /// 代办查询
    /// </summary>
    public interface ITaskQuery : IRuntime
    {
        /// <summary>
        /// 查询代办
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="status">任务状态条件</param>
        /// <returns>代办</returns>
        PagedCollection<USER_TASKBO_PROCESS> QueryTask(string sendToUserId, UserTaskQueryCondition condition, int pageIndex, int pageSize, int? totalCount = default(int?));

        /// <summary>
        ///  查询代办
        /// </summary>
        /// <param name="sendToUserId">Id</param>
        /// <param name="condition">查询条件</param>
        /// <param name="topIndex">件数</param>
        /// <returns>待办信息</returns>
        List<USER_TASKBO_TOPUNPROCESS> QueryTaskUnProcessedTop(string sendToUserId, UserTaskQueryCondition condition, int topIndex);
    
    
    }
}
