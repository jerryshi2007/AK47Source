using MCS.Library.Core;
using MCS.Library.WcfExtensions;
using MCS.Library.WF.Contracts.DataObjects;
using MCS.Library.WF.Contracts.Operations;
using MCS.Library.WF.Contracts.Proxies.Configuration;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Proxies
{
    /// <summary>
    /// 数据源查询（分页查询）相关服务的代理类
    /// </summary>
    public class WfClientDataSourceServiceProxy : WfClientServiceProxyBase<IWfClientDataSourceService>
    {
        public static readonly WfClientDataSourceServiceProxy Instance = new WfClientDataSourceServiceProxy();

        private WfClientDataSourceServiceProxy()
        {
        }

        public WfClientUserOperationLogPageQueryResult QueryUserOperationLogByResourceID(string resourceID, int startRowIndex, int maximumRows, string orderBy, int totalCount)
        {
            return this.SingleCall(action => action.QueryUserOperationLogByResourceID(resourceID, startRowIndex, maximumRows, orderBy, totalCount));
        }

        public WfClientUserOperationLog GetUserOperationLogByID(Int64 logID)
        {
            return this.SingleCall(action => action.GetUserOperationLogByID(logID));
        }

        protected override WfClientChannelFactory<IWfClientDataSourceService> GetService()
        {
            EndpointAddress endPoint = new EndpointAddress(WfContractsProxySettings.GetConfig().DataSourceServiceUrl.ToString());

            return new WfClientChannelFactory<IWfClientDataSourceService>(endPoint);
        }
    }
}
