using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.Globalization;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects.Workflow.Exporters;
using MCS.Library.WcfExtensions;
using MCS.Library.WF.Contracts.Converters.DataObjects;
using MCS.Library.WF.Contracts.Converters.Descriptors;
using MCS.Library.WF.Contracts.Operations;
using MCS.Library.WF.Contracts.Workflow.Builders;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WfOperationServices.Services
{
    /// <summary>
    /// 操作流程定义的服务
    /// </summary>
    public class ProcessDescriptorService : IWfClientProcessDescriptorService
    {
        #region 流程定义
        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void DeleteDescriptor(string processKey)
        {
            if (processKey.IsNullOrEmpty())
                throw new ApplicationException(Translator.Translate(Define.DefaultCulture, "流程描述的Key不能为空"));

            OperationContext.Current.FillContextToOguServiceContext();

            WfDeleteTemplateExecutor executor = new WfDeleteTemplateExecutor(processKey);

            executor.Execute();
        }

        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void ClearAll()
        {
            OperationContext.Current.FillContextToOguServiceContext();

            WfClearAllTemplatesExecutor executor = new WfClearAllTemplatesExecutor();

            executor.Execute();
        }

        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public WfClientProcessDescriptor GetDescriptor(string processKey)
        {
            if (processKey.IsNullOrEmpty())
                throw new ApplicationException(Translator.Translate(Define.DefaultCulture, "流程描述的Key不能为空"));

            OperationContext.Current.FillContextToOguServiceContext();

            WfProcessDescriptor processDesp = (WfProcessDescriptor)WfProcessDescriptorManager.GetDescriptor(processKey);

            WfClientProcessDescriptor clientProcessDesp = null;

            WfClientProcessDescriptorConverter.Instance.ServerToClient(processDesp, ref clientProcessDesp);

            return clientProcessDesp;
        }

        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public WfClientProcessDescriptor LoadDescriptor(string processKey)
        {
            if (processKey.IsNullOrEmpty())
                throw new ApplicationException(Translator.Translate(Define.DefaultCulture, "流程描述的Key不能为空"));

            OperationContext.Current.FillContextToOguServiceContext();

            WfProcessDescriptor processDesp = (WfProcessDescriptor)WfProcessDescriptorManager.LoadDescriptor(processKey);

            WfClientProcessDescriptor clientProcessDesp = null;

            WfClientProcessDescriptorConverter.Instance.ServerToClient(processDesp, ref clientProcessDesp);

            return clientProcessDesp;
        }

        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void SaveDescriptor(WfClientProcessDescriptor clientProcessDesp)
        {
            OperationContext.Current.FillContextToOguServiceContext();

            WfProcessDescriptor processDesp = null;

            WfClientProcessDescriptorConverter.Instance.ClientToServer(clientProcessDesp, ref processDesp);

            WfSaveTemplateExecutor executor = new WfSaveTemplateExecutor(processDesp);

            executor.Execute();
        }

        /// <summary>
        /// 查询流程定义列表
        /// </summary>
        /// <param name="startRowIndex">从0开始的起始行，相当于分页查询的每一页的起始行</param>
        /// <param name="maximumRows">返回的最大行，相当于分页查询每页的大小</param>
        /// <param name="where">筛选条件</param>
        /// <param name="orderBy">排序字段，允许为空，如果为空，则使用修改时间降排序</param>
        /// <param name="totalCount">以前查询的总记录数，如果是第一次，则传入-1</param>
        /// <returns>分页查询结果，里面包含总行数和每一行的结果。其总行数在翻页时需要传入到totalCount参数中</returns>
        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public WfClientProcessDescriptorInfoPageQueryResult QueryProcessDescriptorInfo(int startRowIndex, int maximumRows, string where, string orderBy, int totalCount)
        {
            OperationContext.Current.FillContextToOguServiceContext();

            if (orderBy.IsNullOrEmpty())
                orderBy = "MODIFY_TIME DESC";

            string selectFields = "PROCESS_KEY, APPLICATION_NAME, PROGRAM_NAME, PROCESS_NAME, ENABLED, CREATE_TIME, CREATOR_ID, CREATOR_NAME, MODIFY_TIME, MODIFIER_ID, MODIFIER_NAME, IMPORT_TIME, IMPORT_USER_ID, IMPORT_USER_NAME";

            QueryCondition qc = new QueryCondition(
                startRowIndex,
                maximumRows,
                selectFields,
                ORMapping.GetMappingInfo(typeof(WfProcessDescriptorInfo)).TableName,
                orderBy);

            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

            builder.AppendTenantCode();

            if (where.IsNotEmpty())
                where += " AND ";

            where += builder.ToSqlString(TSqlBuilder.Instance);

            qc.WhereClause = where;

            CommonAdapter adapter = new CommonAdapter(WfProcessDescriptorInfoAdapter.Instance.ConnectionName);

            WfProcessDescriptorInfoCollection processInfo = adapter.SplitPageQuery<WfProcessDescriptorInfo, WfProcessDescriptorInfoCollection>(qc, ref totalCount);

            WfClientProcessDescriptorInfoCollection clientInfo = new WfClientProcessDescriptorInfoCollection();

            WfClientProcessDescriptorInfoConverter.Instance.ServerToClient(processInfo, clientInfo);

            WfClientProcessDescriptorInfoPageQueryResult result = new WfClientProcessDescriptorInfoPageQueryResult();

            result.TotalCount = totalCount;
            result.QueryResult.CopyFrom(clientInfo);

            return result;
        }

        /// <summary>
        /// 判定流程KEY是否存在
        /// </summary>
        /// <param name="processKey">流程KEY</param>
        /// <returns>是否存在</returns>
        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public bool ExsitsProcessKey(string processKey)
        {
            if (processKey.IsNullOrEmpty())
                throw new ApplicationException(Translator.Translate(Define.DefaultCulture, "流程描述的Key不能为空"));

            OperationContext.Current.FillContextToOguServiceContext();

            return WfProcessDescriptorManager.ExsitsProcessKey(processKey);
        }
        #endregion

        #region 委托代办

        /// <summary>
        /// 得到委托人的所有委托信息
        /// </summary>
        /// <param name="userID">委托人ID</param>
        /// <returns></returns>
        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public List<WfClientDelegation> LoadUserDelegations(string userID)
        {
            OperationContext.Current.FillContextToOguServiceContext();

            WfDelegationCollection delegations = WfDelegationAdapter.Instance.Load(userID);

            return WfClientDelegationConverter.Instance.ServerToClient(delegations);
        }

        /// <summary>
        /// 更新委托信息
        /// </summary>
        /// <param name="delegetion"></param>
        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void UpdateUserDelegation(WfClientDelegation delegation)
        {
            OperationContext.Current.FillContextToOguServiceContext();

            WfDelegation server = null;
            WfClientDelegationConverter.Instance.ClientToServer(delegation, ref server);

            WfDelegationAdapter.Instance.Update(server);
        }

        /// <summary>
        /// 按委托人删除其所有的委托信息
        /// </summary>
        /// <param name="userID"></param>
        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void DeleteUserDelegationByUserID(string userID)
        {
            OperationContext.Current.FillContextToOguServiceContext();

            WfDelegationAdapter.Instance.Delete(builder =>
             {
                 builder.AppendItem("SOURCE_USER_ID", userID);
             });
        }

        /// <summary>
        /// 删除委托信息
        /// </summary>
        /// <param name="delegation"></param>
        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void DeleteUserDelegation(WfClientDelegation delegation)
        {
            OperationContext.Current.FillContextToOguServiceContext();
            WfDelegation server = null;
            WfClientDelegationConverter.Instance.ClientToServer(delegation, ref server);

            WfDelegationAdapter.Instance.Delete(server);
        }

        #endregion

        #region EXCEL与Matrix
        /// <summary>
        /// 导入EXCEL返回具有一个动态活动流程的参数
        /// </summary>
        /// <param name="processKey"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public WfCreateClientDynamicProcessParams ExcelToWfCreateClientDynamicProcessParams(string processKey, Stream stream)
        {
            OperationContext.Current.FillContextToOguServiceContext();
            WfCreateClientDynamicProcessParams processParams = null;

            WfCreateClientDynamicProcessParamsConverter.Instance.ExcelStreamToClient(processKey, stream, ref processParams);

            return processParams;
        }

        /// <summary>
        /// 导入EXCEL生成具有一个动态活动流程的定义
        /// </summary>
        /// <param name="processKey"></param>
        /// <param name="stream"></param>
        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void ExcelToSaveDescriptor(string processKey, Stream stream)
        {
            OperationContext.Current.FillContextToOguServiceContext();

            WfCreateClientDynamicProcessParams processParams = ExcelToWfCreateClientDynamicProcessParams(processKey, stream);

            WfClientDynamicProcessBuilder builder = new WfClientDynamicProcessBuilder(processParams);
            WfClientProcessDescriptor client = builder.Build(processParams.Key, processParams.Name);

            WfProcessDescriptor processDesp = null;
            WfClientProcessDescriptorConverter.Instance.ClientToServer(client, ref processDesp);

            WfProcessDescriptorManager.SaveDescriptor(processDesp);
        }

        /// <summary>
        /// 导出具有一个动态活动流程的EXCEL文件
        /// </summary>
        /// <param name="processKey"></param>
        /// <returns></returns>
        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public Stream WfDynamicProcessToExcel(string processKey)
        {
            OperationContext.Current.FillContextToOguServiceContext();

            WfProcessDescriptor processDesp = (WfProcessDescriptor)WfProcessDescriptorManager.LoadDescriptor(processKey);
            WfClientProcessDescriptor client = null;
            WfClientProcessDescriptorConverter.Instance.ServerToClient(processDesp, ref client);

            return WfClientProcessDescriptorConverter.Instance.ClientDynamicProcessToExcelStream(client);
        }
        #endregion

        /// <summary>
        /// 将若干已经存在流程定义导出
        /// </summary>
        /// <param name="exportParams"></param>
        /// <param name="processKeys"></param>
        /// <returns></returns>
        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public Stream ExportProcessDescriptors(WfClientExportProcessDescriptorParams exportParams, string[] processKeys)
        {
            OperationContext.Current.FillContextToOguServiceContext();

            WfExportProcessDescriptorParams serverExportParams = null;

            WfClientExportProcessDescriptorParamsConverter.Instance.ClientToServer(exportParams, ref serverExportParams);

            MemoryStream stream = new MemoryStream();

            WfProcessExporter.ExportProcessDescriptors(serverExportParams, stream, processKeys);

            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        /// <summary>
        /// 将流中的内容作为流程定义导入
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public string ImportProcessDescriptors(Stream inputStream)
        {
            OperationContext.Current.FillContextToOguServiceContext();

            StringBuilder logger = new StringBuilder();

            WfImportTemplateExecutor executor = new WfImportTemplateExecutor(inputStream, info => logger.Append(info));

            executor.Execute();

            return logger.ToString();
        }

        /// <summary>
        /// 导出审批矩阵到Excel
        /// </summary>
        /// <param name="matrixID"></param>
        /// <returns></returns>
        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public Stream ExportApprovalMatrix(string matrixID)
        {
            WfApprovalMatrix matrix = WfApprovalMatrixAdapter.Instance.LoadByID(matrixID);

            return matrix.ToExcelStream();
        }

        /// <summary>
        /// 从Excel导入审批矩阵
        /// </summary>
        /// <param name="matrixID"></param>
        /// <param name="inputStream"></param>
        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void ImportApprovalMatrix(string matrixID, Stream inputStream)
        {
            WfImportApprovalMatrixExecutor executor = new WfImportApprovalMatrixExecutor(matrixID, inputStream);

            executor.Execute();
        }

        /// <summary>
        /// 删除审批矩阵
        /// </summary>
        /// <param name="matrixID"></param>
        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void DeleteApprovalMatrix(string matrixID)
        {
            WfDeleteApprovalMatrixExecutor executor = new WfDeleteApprovalMatrixExecutor(matrixID);

            executor.Execute();
        }

        /// <summary>
        /// 审批矩阵是否存在
        /// </summary>
        /// <param name="matrixID"></param>
        /// <returns></returns>
        [WfJsonFormatter]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public bool ApprovalMatrixExists(string matrixID)
        {
            matrixID.CheckStringIsNullOrEmpty("matrixID");

            Dictionary<string, bool> dictionary = SOARolePropertyDefinitionAdapter.Instance.AreExist(new string[] { matrixID });

            return dictionary.GetValue(matrixID, false);
        }
    }
}
