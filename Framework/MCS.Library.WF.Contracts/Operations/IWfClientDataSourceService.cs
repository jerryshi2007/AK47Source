using MCS.Library.WF.Contracts.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Operations
{
    /// <summary>
    /// 数据源查询（分页查询）相关服务
    /// </summary>
    [ServiceContract]
    public interface IWfClientDataSourceService
    {
        /// <summary>
        /// 根据ResourceID来查询用户操作日志。默认会传入TenantCode
        /// </summary>
        /// <param name="resourceID"></param>
        /// <param name="startRowIndex"></param>
        /// <param name="maximumRows"></param>
        /// <param name="orderBy"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        [OperationContract]
        WfClientUserOperationLogPageQueryResult QueryUserOperationLogByResourceID(string resourceID, int startRowIndex, int maximumRows, string orderBy, int totalCount);

        /// <summary>
        /// 根据日志ID来获取日志对象
        /// </summary>
        /// <param name="logID"></param>
        /// <returns></returns>
        [OperationContract]
        WfClientUserOperationLog GetUserOperationLogByID(Int64 logID);
    }
}
