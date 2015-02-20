using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIIC.HSR.TSP.DataAccess;
namespace CIIC.HSR.TSP.WF.Persistence.Contract
{
    interface TaskQuery
    {
                /// <summary>
        /// 查询代办
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="status">任务状态条件</param>
        /// <returns>代办</returns>
        PagedCollection<BizObject.USER_TASKBO> QueryTask(string userId, TaskStatus status, string TenantCode);
    }
}
