using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WcfExtensions;
using MCS.Library.WF.Contracts.Converters.DataObjects;
using MCS.Library.WF.Contracts.DataObjects;
using MCS.Library.WF.Contracts.Operations;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WfOperationServices.Services
{
    /// <summary>
    /// 数据源查询（分页查询）相关服务
    /// </summary>
    public class DataSourceService : IWfClientDataSourceService
    {
        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public WfClientUserOperationLog GetUserOperationLogByID(long logID)
        {
            UserOperationLog server = UserOperationLogAdapter.Instance.Load(logID);

            WfClientUserOperationLog client = null;

            WfClientUserOperationLogConverter.Instance.ServerToClient(server, ref client);

            return client;
        }

        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public WfClientUserOperationLogPageQueryResult QueryUserOperationLogByResourceID(string resourceID, int startRowIndex, int maximumRows, string orderBy, int totalCount)
        {
            resourceID.CheckStringIsNullOrEmpty("resourceID");

            OperationContext.Current.FillContextToOguServiceContext();

            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

            builder.AppendItem("RESOURCE_ID", resourceID);

            builder.AppendTenantCode();

            if (orderBy.IsNullOrEmpty())
                orderBy = "ID DESC";

            QueryCondition qc = new QueryCondition(startRowIndex, maximumRows,
                "*",
                ORMapping.GetMappingInfo(typeof(UserOperationLog)).TableName,
                orderBy);

            qc.WhereClause = builder.ToSqlString(TSqlBuilder.Instance);

            CommonAdapter adapter = new CommonAdapter(UserOperationLogAdapter.Instance.ConnectionName);

            UserOperationLogCollection serverLogs = adapter.SplitPageQuery<UserOperationLog, UserOperationLogCollection>(qc, ref totalCount);

            WfClientUserOperationLogCollection clientLogs = WfClientUserOperationLogConverter.Instance.ServerToClient(serverLogs);

            WfClientUserOperationLogPageQueryResult result = new WfClientUserOperationLogPageQueryResult();

            result.TotalCount = totalCount;
            result.QueryResult.CopyFrom(clientLogs);

            return result;
        }

        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public WfClientApplicationCollection GetAllApplications()
        {
            return WfClientApplicationConverter.Instance.ServerToClient(WfApplicationAdapter.Instance.LoadAll());
        }

        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public WfClientProgramInApplicationCollection GetProgramsByApplication(string appCodeName)
        {
            return WfClientProgramConverter.Instance.ServerToClient(WfApplicationAdapter.Instance.LoadProgramsByApplication(appCodeName));
        }
    }
}
