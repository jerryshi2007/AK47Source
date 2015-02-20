using CIIC.HSR.TSP.DataAccess;
using CIIC.HSR.TSP.WF.Bizlet.Common;
using CIIC.HSR.TSP.WF.Persistence.Contract;
using CIIC.HSR.TSP.WF.Bizlet.Contract;
using CIIC.HSR.TSP.WF.BizObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    /// <summary>
    /// 任务查询
    /// </summary>
    public class TaskQuery : ITaskQuery
    {
        /// <summary>
        /// 查询代办
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="status">任务状态条件</param>
        /// <returns>代办</returns>
        public PagedCollection<BizObject.USER_TASKBO_PROCESS> QueryTask(string sendToUserId, UserTaskQueryCondition condition, int pageIndex, int pageSize, int? totalCount = default(int?))
        {
            //事例代码
            using (var uow = TenantHelper.GetUnitWOrk(this.Context.TenantCode))
            {
                var userTaskRepository = uow.GetRepository<USER_TASKBO, IUSER_TASKRepository>();
                return userTaskRepository.QueryTask(this.Context.TenantCode,sendToUserId, condition, pageIndex, pageSize, totalCount);
            }
        }

        /// <summary>
        ///  查询代办
        /// </summary>
        /// <param name="sendToUserId">Id</param>
        /// <param name="condition">查询条件</param>
        /// <param name="topIndex">件数</param>
        /// <returns>待办信息</returns>
        public List<USER_TASKBO_TOPUNPROCESS> QueryTaskUnProcessedTop(string sendToUserId, UserTaskQueryCondition condition, int topIndex)
        {
            //事例代码
            using (var uow = TenantHelper.GetUnitWOrk(this.Context.TenantCode))
            {
                var userTaskRepository = uow.GetRepository<USER_TASKBO, IUSER_TASKRepository>();
                return userTaskRepository.QueryTaskUnProcessedTop(this.Context.TenantCode, sendToUserId, condition, topIndex);
            }
        }

        /// <summary>
        /// 上下文数据
        /// </summary>
        public ServiceContext Context
        {
            get;
            set;
        }
    }
}
