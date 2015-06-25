using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.Globalization;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects.Workflow.Conditions;
using MCS.Library.WcfExtensions;
using MCS.Library.WF.Contracts.Converters;
using MCS.Library.WF.Contracts.Converters.DataObjects;
using MCS.Library.WF.Contracts.Converters.Runtime;
using MCS.Library.WF.Contracts.DataObjects;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Operations;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace WfOperationServices.Services
{
    public class ProcessRuntimeService : IWfClientProcessRuntimeService
    {
        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public WfClientProcessInfo StartWorkflow(WfClientProcessStartupParams clientStartupParams)
        {
            if (clientStartupParams == null)
                throw new ApplicationException(Translator.Translate(Define.DefaultCulture, "流程的启动参数不能为空"));

            //设置标准参数,优先使用外部参数
            if (!clientStartupParams.ApplicationRuntimeParameters.ContainsKey("ProcessRequestor")) 
                clientStartupParams.ApplicationRuntimeParameters["ProcessRequestor"] = clientStartupParams.Creator; 

            OperationContext.Current.FillContextToOguServiceContext();

            WfProcessStartupParams startupParams = null;

            WfClientProcessStartupParamsConverter.Instance.ClientToServer(clientStartupParams, ref startupParams);

            IWfProcess process = null;

            DoPrincipalAction(startupParams.Creator, () =>
            {
                WfStartWorkflowExecutor executor = new WfStartWorkflowExecutor(startupParams);

                executor.AfterModifyWorkflow += (dataContext =>
                {
                    dataContext.CurrentProcess.GenerateCandidatesFromResources();
                    clientStartupParams.ProcessContext.ForEach(kp => dataContext.CurrentProcess.Context[kp.Key] = kp.Value);

                    WfClientProcessInfoBaseConverter.Instance.FillOpinionInfoByProcessByActivity(
                        clientStartupParams.Opinion,
                        dataContext.CurrentProcess.CurrentActivity,
                        clientStartupParams.Creator,
                        clientStartupParams.Creator);
                });

                executor.SaveApplicationData += (dataContext) => SaveOpinion(clientStartupParams.Opinion);

                if (clientStartupParams.AutoPersist)
                    process = executor.Execute();
                else
                    process = executor.ExecuteNotPersist();
            });

            return process.ToClientProcessInfo(clientStartupParams.Creator).FillCurrentOpinion(process.CurrentActivity, clientStartupParams.Creator);
        }

        /// <summary>
        /// 启动一个流程实例，并且流转到下一个环节。
        /// </summary>
        /// <param name="clientStartupParams">流程启动参数</param>
        /// <param name="clientTransferParams">流程下一步的流转参数，如果这个参数为空，则流转到默认环节</param>
        /// <param name="runtimeContext">流转上下文信息</param>
        /// <returns></returns>
        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public WfClientProcessInfo StartWorkflowAndMoveTo(WfClientProcessStartupParams clientStartupParams, WfClientTransferParams clientTransferParams, WfClientRuntimeContext runtimeContext)
        {
            if (clientStartupParams == null)
                throw new ApplicationException(Translator.Translate(Define.DefaultCulture, "流程的启动参数不能为空"));

            //设置标准参数,优先使用外部参数
            if (!clientStartupParams.ApplicationRuntimeParameters.ContainsKey("ProcessRequestor")) 
            clientStartupParams.ApplicationRuntimeParameters["ProcessRequestor"] = clientStartupParams.Creator;

            OperationContext.Current.FillContextToOguServiceContext();

            WfProcessStartupParams startupParams = null;

            WfClientProcessStartupParamsConverter.Instance.ClientToServer(clientStartupParams, ref startupParams);

            WfTransferParams transferParams = null;

            if (clientTransferParams != null)
            {
                WfClientTransferParamsConverter.Instance.ClientToServer(clientTransferParams, null, ref transferParams);

                MergeTransferParams(transferParams, runtimeContext);
            }

            IWfProcess process = null;

            DoPrincipalAction(startupParams.Creator, () =>
            {
                WfStartWorkflowExecutor executor = new WfStartWorkflowExecutor(null, startupParams, transferParams);

                executor.AfterModifyWorkflow += (dataContext =>
                {
                    dataContext.CurrentProcess.GenerateCandidatesFromResources();
                    clientStartupParams.ProcessContext.ForEach(kp => dataContext.CurrentProcess.Context[kp.Key] = kp.Value);

                    WfClientProcessInfoBaseConverter.Instance.FillOpinionInfoByProcessByActivity(
                        clientStartupParams.Opinion,
                        dataContext.CurrentProcess.InitialActivity,
                        clientStartupParams.Creator,
                        clientStartupParams.Creator);
                });

                WfClientOpinion opinion = clientStartupParams.Opinion;

                if (opinion == null)
                    opinion = runtimeContext.Opinion;

                executor.SaveApplicationData += (dataContext) => SaveOpinion(opinion, dataContext.CurrentProcess.InitialActivity, executor.TransferParams);

                if (clientStartupParams.AutoPersist)
                    process = executor.Execute();
                else
                    process = executor.ExecuteNotPersist();
            });

            return process.ToClientProcessInfo(clientStartupParams.Creator).FillCurrentOpinion(process.CurrentActivity, clientStartupParams.Creator);
        }

        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public WfClientProcessInfo MoveToNextDefaultActivity(string processID, WfClientRuntimeContext runtimeContext)
        {
            processID.CheckStringIsNullOrEmpty("processID");

            OperationContext.Current.FillContextToOguServiceContext();

            IWfProcess process = GetProcessByProcessID(processID, runtimeContext);

            MeregeRuntimeContext(process, runtimeContext);

            WfTransferParams transferParams = WfTransferParams.FromNextDefaultActivity(process);

            MergeTransferParams(transferParams, runtimeContext);

            IWfActivity originalActivity = process.CurrentActivity;

            ExecuteExecutor(process, runtimeContext, (p) =>
            {
                WfMoveToExecutor executor = new WfMoveToExecutor(p.CurrentActivity, p.CurrentActivity, transferParams);

                executor.SaveApplicationData += (dataContext) => SaveOpinion(runtimeContext.Opinion, originalActivity, transferParams);
                executor.PrepareMoveToTasks += (dataContext, tasks) =>
                    tasks.ForEach(task => task.Purpose = p.CurrentActivity.Descriptor.Name);

                return executor;
            });

            return process.ToClientProcessInfo(transferParams.Operator).FillCurrentOpinion(process.CurrentActivity, runtimeContext.Operator);
        }

        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public WfClientProcessInfo MoveTo(string processID, WfClientTransferParams clientTransferParams, WfClientRuntimeContext runtimeContext)
        {
            processID.CheckStringIsNullOrEmpty("processID");
            clientTransferParams.NullCheck("clientTransferParams");

            OperationContext.Current.FillContextToOguServiceContext();

            IWfProcess process = GetProcessByProcessID(processID, runtimeContext);

            MeregeRuntimeContext(process, runtimeContext);

            WfTransferParams transferParams = null;

            WfClientTransferParamsConverter.Instance.ClientToServer(clientTransferParams, process, ref transferParams);

            MergeTransferParams(transferParams, runtimeContext);

            IWfActivity originalActivity = process.CurrentActivity;

            ExecuteExecutor(process, runtimeContext, (p) =>
            {
                WfMoveToExecutor executor = new WfMoveToExecutor(p.CurrentActivity, p.CurrentActivity, transferParams);

                executor.SaveApplicationData += (dataContext) => SaveOpinion(runtimeContext.Opinion, originalActivity, transferParams);
                executor.PrepareMoveToTasks += (dataContext, tasks) =>
                    tasks.ForEach(task => task.Purpose = p.CurrentActivity.Descriptor.Name);

                return executor;
            });

            return process.ToClientProcessInfo(transferParams.Operator).FillCurrentOpinion(process.CurrentActivity, runtimeContext.Operator);
        }

        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public WfClientProcessInfo UpdateRuntimeParameters(string processID, WfClientRuntimeContext runtimeContext)
        {
            processID.CheckStringIsNullOrEmpty("processID");
            runtimeContext.NullCheck("clientTransferParams");

            OperationContext.Current.FillContextToOguServiceContext();

            IWfProcess process = GetProcessByProcessID(processID, runtimeContext);

            MeregeRuntimeContext(process, runtimeContext);

            if (runtimeContext.AutoPersist)
            {
                WfRuntime.ProcessContext.AffectedProcesses.AddOrReplace(process);

                ExecuteProcessAction(processID, runtimeContext, (p) => WfRuntime.PersistWorkflows());
            }

            return process.ToClientProcessInfo(runtimeContext.Operator).FillCurrentOpinion(process.CurrentActivity, runtimeContext.Operator);
        }

        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public WfClientProcessInfo SaveProcess(string processID, WfClientRuntimeContext runtimeContext)
        {
            OperationContext.Current.FillContextToOguServiceContext();

            return ExecuteProcessActionByProcessID(processID, runtimeContext,
                    (process) =>
                    {
                        WfSaveDataExecutor executor = new WfSaveDataExecutor(process.CurrentActivity, process.CurrentActivity);

                        executor.SaveApplicationData += (dataContext) => SaveOpinion(runtimeContext.Opinion);
                        return executor;
                    }
                );
        }

        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public WfClientProcessInfo Withdraw(string processID, WfClientRuntimeContext runtimeContext)
        {
            OperationContext.Current.FillContextToOguServiceContext();

            return ExecuteProcessActionByProcessID(processID, runtimeContext,
                (process) => new WfWithdrawExecutor(process.CurrentActivity, process.CurrentActivity, true));
        }

        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public WfClientProcessInfo WithdrawAndCancel(string processID, WfClientRuntimeContext runtimeContext)
        {
            OperationContext.Current.FillContextToOguServiceContext();

            return ExecuteProcessActionByProcessID(processID, runtimeContext,
                (process) =>
                {
                    WfWithdrawExecutor executor = new WfWithdrawExecutor(process.CurrentActivity, process.CurrentActivity, true, true);

                    executor.SaveApplicationData += (dataContext) => SaveAbortOpinion(runtimeContext.Opinion);
                    return executor;
                });
        }

        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public WfClientProcessInfo Cancel(string processID, WfClientRuntimeContext runtimeContext)
        {
            OperationContext.Current.FillContextToOguServiceContext();

            return ExecuteProcessActionByProcessID(processID, runtimeContext,
                (process) =>
                {
                    WfCancelProcessExecutor executor = new WfCancelProcessExecutor(process.CurrentActivity, process);

                    executor.SaveApplicationData += (dataContext) => SaveAbortOpinion(runtimeContext.Opinion);
                    return executor;
                });
        }

        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public WfClientProcessInfo Pause(string processID, WfClientRuntimeContext runtimeContext)
        {
            OperationContext.Current.FillContextToOguServiceContext();

            return ExecuteProcessActionByProcessID(processID, runtimeContext,
                (process) => new WfPauseProcessExecutor(process.CurrentActivity, process));
        }

        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public WfClientProcessInfo Resume(string processID, WfClientRuntimeContext runtimeContext)
        {
            OperationContext.Current.FillContextToOguServiceContext();

            return ExecuteProcessActionByProcessID(processID, runtimeContext,
                (process) => new WfResumeProcessExecutor(process.CurrentActivity, process));
        }

        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public WfClientProcessInfo Restore(string processID, WfClientRuntimeContext runtimeContext)
        {
            OperationContext.Current.FillContextToOguServiceContext();

            return ExecuteProcessActionByProcessID(processID, runtimeContext,
                (process) => new WfRestoreProcessExecutor(process.CurrentActivity, process));
        }

        /// <summary>
        /// 替换某个活动中的办理人，无论该活动的状态。如果这个人有待办，待办也会被替换。
        /// </summary>
        /// <param name="activityID">需要替换的活动的ID</param>
        /// <param name="originalUser">被替换的人。如果这个属性为null，则替换掉这个活动中所有的指派人和候选人</param>
        /// <param name="targetUsers">替换成的人，如果为null，则不完成替换</param>
        /// <param name="runtimeContext">流转上下文信息</param>
        /// <returns></returns>
        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public WfClientProcessInfo ReplaceAssignees(string activityID, WfClientUser originalUser, List<WfClientUser> targetUsers, WfClientRuntimeContext runtimeContext)
        {
            OperationContext.Current.FillContextToOguServiceContext();

            IWfProcess process = WfRuntime.GetProcessByActivityID(activityID);
            IWfActivity targetActivity = process.Activities[activityID];

            ExecuteExecutor(process, runtimeContext,
                (p) => new WfReplaceAssigneesExecutor(targetActivity, targetActivity, (IUser)originalUser.ToOguObject(), targetUsers.ToOguObjects<WfClientUser, IUser>()));

            return process.ToClientProcessInfo(runtimeContext.Operator).FillCurrentOpinion(process.CurrentActivity, runtimeContext.Operator);
        }

        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public WfClientProcess GetProcessByID(string processID, WfClientUser user, WfClientProcessInfoFilter filter)
        {
            processID.CheckStringIsNullOrEmpty("processID");

            OperationContext.Current.FillContextToOguServiceContext();

            IWfProcess process = WfRuntime.GetProcessByProcessID(processID);

            WfClientProcess client = null;

            new WfClientProcessConverter(filter).ServerToClient(process, ref client);

            client.AuthorizationInfo = WfClientProcessInfoBaseConverter.Instance.GetAuthorizationInfo(process, process.CurrentActivity, user);

            if ((filter & WfClientProcessInfoFilter.CurrentOpinion) == WfClientProcessInfoFilter.CurrentOpinion)
                client.FillCurrentOpinion(process.CurrentActivity, user);

            return client;
        }

        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public WfClientProcess GetProcessByActivityID(string activityID, WfClientUser user, WfClientProcessInfoFilter filter)
        {
            activityID.CheckStringIsNullOrEmpty("activityID");

            OperationContext.Current.FillContextToOguServiceContext();

            IWfProcess process = WfRuntime.GetProcessByActivityID(activityID);

            WfClientProcess client = null;

            new WfClientProcessConverter(filter).ServerToClient(process, ref client);

            IWfActivity originalActivity = process.Activities[activityID];

            client.AuthorizationInfo = WfClientProcessInfoBaseConverter.Instance.GetAuthorizationInfo(process, originalActivity, user);

            if ((filter & WfClientProcessInfoFilter.CurrentOpinion) == WfClientProcessInfoFilter.CurrentOpinion)
                client.FillCurrentOpinion(originalActivity, user);

            return client;
        }

        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public List<WfClientProcess> GetProcessesByResourceID(string resourceID, WfClientUser user, WfClientProcessInfoFilter filter)
        {
            resourceID.CheckStringIsNullOrEmpty("resourceID");

            OperationContext.Current.FillContextToOguServiceContext();

            WfProcessCollection processes = WfRuntime.GetProcessByResourceID(resourceID).SortByUserRelativity((IUser)user.ToOguObject());

            WfClientProcessConverter converter = new WfClientProcessConverter(filter);

            List<WfClientProcess> result = new List<WfClientProcess>();

            foreach (IWfProcess process in processes)
            {
                WfClientProcess client = null;

                converter.ServerToClient(process, ref client);

                client.AuthorizationInfo = WfClientProcessInfoBaseConverter.Instance.GetAuthorizationInfo(process, process.CurrentActivity, user);

                if ((filter & WfClientProcessInfoFilter.CurrentOpinion) == WfClientProcessInfoFilter.CurrentOpinion)
                    client.FillCurrentOpinion(process.CurrentActivity, user);

                result.Add(client);
            }

            return result;
        }

        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public WfClientProcessInfo GetProcessInfoByActivityID(string activityID, WfClientUser user)
        {
            activityID.CheckStringIsNullOrEmpty("activityID");

            OperationContext.Current.FillContextToOguServiceContext();

            IWfProcess process = WfRuntime.GetProcessByActivityID(activityID);
            IWfActivity activity = process.Activities[activityID];

            return process.ToClientProcessInfo(activity, user).FillCurrentOpinion(activity, user);
        }

        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public WfClientProcessInfo GetProcessInfoByID(string processID, WfClientUser user)
        {
            processID.CheckStringIsNullOrEmpty("processID");

            OperationContext.Current.FillContextToOguServiceContext();

            IWfProcess process = WfRuntime.GetProcessByProcessID(processID);

            return process.ToClientProcessInfo(user).FillCurrentOpinion(process.CurrentActivity, user);
        }

        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public List<WfClientProcessInfo> GetProcessesInfoByResourceID(string resourceID, WfClientUser user)
        {
            resourceID.CheckStringIsNullOrEmpty("resourceID");

            OperationContext.Current.FillContextToOguServiceContext();

            WfProcessCollection processes = WfRuntime.GetProcessByResourceID(resourceID).SortByUserRelativity((IUser)user.ToOguObject());

            List<WfClientProcessInfo> result = new List<WfClientProcessInfo>();

            processes.ForEach(process => result.Add(process.ToClientProcessInfo(user).FillCurrentOpinion(process.CurrentActivity, user)));

            return result;
        }

        /// <summary>
        /// 根据ResourceID读取意见
        /// </summary>
        /// <param name="resourceID"></param>
        /// <returns></returns>
        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public List<WfClientOpinion> GetOpinionsByResourceID(string resourceID)
        {
            OperationContext.Current.FillContextToOguServiceContext();

            return WfClientOpinionConverter.Instance.ServerToClient(GenericOpinionAdapter.Instance.LoadFromResourceID(resourceID));
        }

        /// <summary>
        /// 根据ProcessID读取意见
        /// </summary>
        /// <param name="processID"></param>
        /// <returns></returns>
        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public List<WfClientOpinion> GetOpinionsByProcessID(string processID)
        {
            OperationContext.Current.FillContextToOguServiceContext();

            GenericOpinionCollection serverOpinions = GenericOpinionAdapter.Instance.LoadByInBuilder(builder =>
            {
                builder.DataField = "PROCESS_ID";
                builder.AppendItem(processID);
            });

            return WfClientOpinionConverter.Instance.ServerToClient(serverOpinions);
        }

        /// <summary>
        /// 获取流程中的应用上下文参数。返回的结果一定是字符串型，其它复合类型不支持
        /// </summary>
        /// <param name="processID"></param>
        /// <param name="key"></param>
        /// <param name="probeMode"></param>
        /// <returns></returns>
        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public string GetApplicationRuntimeParameters(string processID, string key, WfClientProbeApplicationRuntimeParameterMode probeMode)
        {
            OperationContext.Current.FillContextToOguServiceContext();

            IWfProcess process = WfRuntime.GetProcessByProcessID(processID);

            return process.ApplicationRuntimeParameters.GetValueRecursively(key, probeMode.ToProbeApplicationRuntimeParameterMode(), string.Empty);
        }

        /// <summary>
        /// 分页查询某个活动下子流程的信息
        /// </summary>
        /// <param name="ownerActivityID">子流程的容器活动</param>
        /// <param name="ownerTemplateKey">容器活动中的分支流程模板，可以为空。为空表示所有子流程，否则是某个模板的子流程</param>
        /// <param name="startRowIndex">从0开始的起始行，相当于分页查询的每一页的起始行</param>
        /// <param name="maximumRows">返回的最大行，相当于分页查询每页的大小</param>
        /// <param name="orderBy">排序字段，允许为空，如果为空，则使用StartTime排序</param>
        /// <param name="totalCount">以前查询的总记录数，如果是第一次，则传入-1</param>
        /// <returns>分页查询结果，里面包含总行数和每一行的结果。其总行数在翻页时需要传入到totalCount参数中</returns>
        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public WfClientProcessCurrentInfoPageQueryResult QueryBranchProcesses(string ownerActivityID, string ownerTemplateKey, int startRowIndex, int maximumRows, string orderBy, int totalCount)
        {
            ownerActivityID.NullCheck("ownerActivityID");

            OperationContext.Current.FillContextToOguServiceContext();

            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

            builder.AppendItem("OWNER_ACTIVITY_ID", ownerActivityID);

            if (ownerTemplateKey.IsNotEmpty())
                builder.AppendItem("OWNER_TEMPLATE_KEY", ownerTemplateKey);

            if (orderBy.IsNullOrEmpty())
                orderBy = "START_TIME";

            QueryCondition qc = new QueryCondition(startRowIndex, maximumRows,
                "INSTANCE_ID, OWNER_ACTIVITY_ID, STATUS, PROCESS_NAME, DESCRIPTOR_KEY, START_TIME, END_TIME",
                ORMapping.GetMappingInfo(typeof(WfProcessCurrentInfo)).TableName,
                orderBy);

            qc.WhereClause = builder.ToSqlString(TSqlBuilder.Instance);

            return QueryProcessInfo(qc, totalCount);
        }

        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public WfClientProcessCurrentInfoPageQueryResult QueryProcesses(WfClientProcessQueryCondition condition, int startRowIndex, int maximumRows, string orderBy, int totalCount)
        {
            condition.NullCheck("condition");

            OperationContext.Current.FillContextToOguServiceContext();

            WfProcessQueryCondition serverCondition = null;

            WfClientProcessQueryConditionConverter.Instance.ClientToServer(condition, ref serverCondition);

            if (orderBy.IsNullOrEmpty())
                orderBy = "START_TIME DESC";

            ConnectiveSqlClauseCollection connective = serverCondition.ToSqlBuilder();

            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

            builder.AppendItem("COMMITTED", "1");
            builder.AppendTenantCode();

            connective.Add(builder);

            QueryCondition qc = new QueryCondition(startRowIndex, maximumRows,
                ORMapping.GetSelectFieldsNameSql<WfProcessCurrentInfo>(),
                ORMapping.GetMappingInfo(typeof(WfProcessCurrentInfo)).TableName,
                orderBy);

            qc.WhereClause += connective.ToSqlString(TSqlBuilder.Instance);

            return QueryProcessInfo(qc, totalCount);
        }

        /// <summary>
        /// 清除租户的业务流程相关的数据
        /// </summary>
        /// <param name="tenantCode">租户编码</param>
        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void ClearTenantProcessInstanceData(string tenantCode)
        {
            string connectionName = WfRuntime.ProcessContext.SimulationContext.GetConnectionName(WorkflowSettings.GetConfig().ConnectionName);

            string sql = string.Format("EXECUTE WF.ClearTenantProcessInstanceData {0}", TSqlBuilder.Instance.CheckUnicodeQuotationMark(tenantCode));

            DbHelper.RunSql(sql, connectionName);
        }

        private static WfClientProcessCurrentInfoPageQueryResult QueryProcessInfo(QueryCondition qc, int totalCount)
        {
            CommonAdapter adapter = new CommonAdapter(WfProcessCurrentInfoAdapter.Instance.ConnectionName);

            WfProcessCurrentInfoCollection processInfo = adapter.SplitPageQuery<WfProcessCurrentInfo, WfProcessCurrentInfoCollection>(qc, ref totalCount);

            WfClientProcessCurrentInfoCollection clientInfo = new WfClientProcessCurrentInfoCollection();

            WfClientProcessCurrentInfoConverter.Instance.ServerToClient(processInfo, clientInfo);

            WfClientProcessCurrentInfoPageQueryResult result = new WfClientProcessCurrentInfoPageQueryResult();

            result.TotalCount = totalCount;
            result.QueryResult.CopyFrom(clientInfo);

            return result;
        }

        /// <summary>
        /// 将runtimeContext的数据合并到流程上下文中
        /// </summary>
        /// <param name="process"></param>
        /// <param name="runtimeContext"></param>
        private static void MeregeRuntimeContext(IWfProcess process, WfClientRuntimeContext runtimeContext)
        {
            if (runtimeContext != null)
            {
                WfClientDictionaryConverter.Instance.ClientToServer(runtimeContext.ApplicationRuntimeParameters, process.ApplicationRuntimeParameters);
                WfClientDictionaryConverter.Instance.ClientToServer(runtimeContext.ProcessContext, process.Context);
                WfClientProcessInfoBaseConverter.Instance.FillOpinionInfoByProcessByActivity(runtimeContext.Opinion, process.CurrentActivity, runtimeContext.Operator, runtimeContext.Operator);

                if (runtimeContext.AutoCalculate)
                    process.GenerateCandidatesFromResources();
            }
        }

        private static void MergeTransferParams(WfTransferParamsBase transferParams, WfClientRuntimeContext runtimeContext)
        {
            if (runtimeContext != null)
            {
                if (transferParams.Operator == null)
                    transferParams.Operator = (IUser)runtimeContext.Operator.ToOguObject();
            }
        }

        private static WfClientProcessInfo ExecuteProcessActionByProcessID(string processID, WfClientRuntimeContext runtimeContext, Func<IWfProcess, WfExecutorBase> getExecutor)
        {
            processID.CheckStringIsNullOrEmpty("processID");
            getExecutor.NullCheck("getExecutor");

            IWfProcess process = GetProcessByProcessID(processID, runtimeContext);

            MeregeRuntimeContext(process, runtimeContext);

            ExecuteExecutor(process, runtimeContext, getExecutor);

            return process.ToClientProcessInfo(runtimeContext.Operator).FillCurrentOpinion(process.CurrentActivity, runtimeContext.Operator);
        }

        private static void ExecuteExecutor(IWfProcess process, WfClientRuntimeContext runtimeContext, Func<IWfProcess, WfExecutorBase> getExecutor)
        {
            DoPrincipalAction(runtimeContext, () =>
            {
                WfExecutorBase executor = getExecutor(process);

                executor.PrepareMoveToTasks += (context, tasks) =>
                {
                    if (runtimeContext.TaskTitle.IsNotEmpty())
                        tasks.ForEach(task => task.TaskTitle = runtimeContext.TaskTitle);
                };

                executor.PrepareNotifyTasks += (context, tasks) =>
                {
                    if (runtimeContext.NotifyTitle.IsNotEmpty())
                        tasks.ForEach(task => task.TaskTitle = runtimeContext.TaskTitle);
                };

                if (runtimeContext.AutoPersist)
                    executor.Execute();
                else
                    executor.ExecuteNotPersist();
            });
        }

        private static WfClientProcessInfo ExecuteProcessAction(string processID, WfClientRuntimeContext runtimeContext, Action<IWfProcess> action)
        {
            processID.CheckStringIsNullOrEmpty("processID");
            action.NullCheck("action");

            IWfProcess process = GetProcessByProcessID(processID, runtimeContext);

            MeregeRuntimeContext(process, runtimeContext);

            DoPrincipalAction(runtimeContext, () => action(process));

            return process.ToClientProcessInfo(runtimeContext.Operator);
        }

        private static void DoPrincipalAction(IUser user, Action action)
        {
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;

            try
            {
                if (user != null)
                {
                    DeluxeIdentity identity = new DeluxeIdentity(user);
                    Thread.CurrentPrincipal = new DeluxePrincipal(identity);
                }

                if (action != null)
                    action();
            }
            finally
            {
                Thread.CurrentPrincipal = originalPrincipal;
            }
        }

        private static void DoPrincipalAction(WfClientRuntimeContext runtimeContext, Action action)
        {
            DoPrincipalAction((IUser)runtimeContext.Operator.ToOguObject(), action);
        }

        private static void SaveOpinion(WfClientOpinion client)
        {
            if (client != null)
            {
                GenericOpinion server = null;

                WfClientOpinionConverter.Instance.ClientToServer(client, ref server);

                GenericOpinionAdapter.Instance.Update(server);
            }
        }

        private static void SaveAbortOpinion(WfClientOpinion client)
        {
            if (client != null)
            {
                Dictionary<string, object> extraDataDict = new Dictionary<string, object>();

                extraDataDict["NextSteps"] = PrepareAbortProcessNextStepsString();

                client.FillExtraDataFromDictionary(extraDataDict);

                SaveOpinion(client);
            }
        }

        private static void SaveOpinion(WfClientOpinion client, IWfActivity currentActivity, WfTransferParams transferParams)
        {
            if (client != null)
            {
                WfTransitionDescriptorCollection nextTransitions = currentActivity.Descriptor.ToTransitions.GetAllCanTransitTransitions(true);

                Dictionary<string, object> extraDataDict = new Dictionary<string, object>();

                extraDataDict["NextSteps"] = PrepareNextStepsSelectorsString(nextTransitions, transferParams);

                client.FillExtraDataFromDictionary(extraDataDict);

                SaveOpinion(client);
            }
        }

        private static string PrepareNextStepsSelectorsString(IEnumerable<IWfTransitionDescriptor> transitions, WfTransferParamsBase transferParams)
        {
            WfClientNextStepCollection nextSteps = new WfClientNextStepCollection();

            foreach (IWfTransitionDescriptor transition in transitions)
            {
                WfClientNextStep nextStep = new WfClientNextStep();

                nextStep.TransitionKey = transition.Key;
                nextStep.TransitionName = transition.Name;
                nextStep.TransitionDescription = transition.Description;

                nextStep.ActivityKey = transition.ToActivity.Key;
                nextStep.ActivityName = transition.ToActivity.Name;
                nextStep.ActivityDescription = transition.ToActivity.Description;

                nextSteps.Add(nextStep);
            }

            if (transferParams != null && transferParams.FromTransitionDescriptor != null)
                nextSteps.SelectedKey = transferParams.FromTransitionDescriptor.Key;

            XElement nextStepsRoot = new XElement("NextSteps");

            nextSteps.ToXElement(nextStepsRoot);

            return nextStepsRoot.ToString();
        }

        private static string PrepareAbortProcessNextStepsString()
        {
            WfClientNextStepCollection nextSteps = new WfClientNextStepCollection();

            WfClientNextStep nextStep = new WfClientNextStep();

            nextStep.TransitionKey = "AbortProcess";
            nextStep.TransitionName = "作废";
            nextStep.TransitionDescription = "作废";

            nextStep.ActivityKey = "AbortProcess";
            nextStep.ActivityName = "作废";
            nextStep.ActivityDescription = "作废";

            nextSteps.Add(nextStep);

            nextSteps.SelectedKey = "AbortProcess";

            XElement nextStepsRoot = new XElement("NextSteps");

            nextSteps.ToXElement(nextStepsRoot);

            return nextStepsRoot.ToString();
        }

        private static IWfProcess GetProcessByProcessID(string processID, WfClientRuntimeContext runtimeContext)
        {
            IWfProcess process = WfRuntime.GetProcessByProcessID(processID);

            if (runtimeContext.UpdateTag >= 0)
                ((WfProcess)process).UpdateTag = runtimeContext.UpdateTag;

            return process;
        }
    }
}
