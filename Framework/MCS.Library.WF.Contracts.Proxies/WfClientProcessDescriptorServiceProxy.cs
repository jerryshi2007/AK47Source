using MCS.Library.Core;
using MCS.Library.WcfExtensions;
using MCS.Library.WF.Contracts.Operations;
using MCS.Library.WF.Contracts.Proxies.Configuration;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Proxies
{
    public class WfClientProcessDescriptorServiceProxy : WfClientServiceProxyBase<IWfClientProcessDescriptorService>
    {
        public static readonly WfClientProcessDescriptorServiceProxy Instance = new WfClientProcessDescriptorServiceProxy();

        private WfClientProcessDescriptorServiceProxy()
        {
        }

        #region 流程
        public void DeleteDescriptor(string processDespKey)
        {
            this.SingleCall(action => action.DeleteDescriptor(processDespKey));
        }

        public WfClientProcessDescriptor GetDescriptor(string processDespKey)
        {
            return this.SingleCall(action => action.GetDescriptor(processDespKey));
        }

        public WfClientProcessDescriptor LoadDescriptor(string processDespKey)
        {
            return this.SingleCall(action => action.LoadDescriptor(processDespKey));
        }

        public void SaveDescriptor(WfClientProcessDescriptor processDesp)
        {
            this.SingleCall(action => action.SaveDescriptor(processDesp));
        }

        public WfClientProcessDescriptorInfoPageQueryResult QueryProcessDescriptorInfo(int startRowIndex, int maximumRows, string where, string orderBy, int totalCount)
        {
            return this.SingleCall(action => action.QueryProcessDescriptorInfo(startRowIndex, maximumRows, where, orderBy, totalCount));
        }

        /// <summary>
        /// 判定流程KEY是否存在
        /// </summary>
        /// <param name="processKey">流程KEY</param>
        /// <returns>是否存在</returns>
        public bool ExsitsProcessKey(string processKey)
        {
            return this.SingleCall(action => action.ExsitsProcessKey(processKey));
        }
        #endregion

        #region 委托待办信息
        public WfClientDelegationCollection LoadUserDelegations(string userID)
        {
            return this.SingleCall(action => new WfClientDelegationCollection(action.LoadUserDelegations(userID)));
        }

        public void UpdateUserDelegation(WfClientDelegation delegation)
        {
            this.SingleCall(action => action.UpdateUserDelegation(delegation));
        }

        /// <summary>
        /// 按委托人删除其所有的委托信息
        /// </summary>
        /// <param name="userID"></param>
        public void DeleteUserDelegation(string userID)
        {
            this.SingleCall(action => action.DeleteUserDelegationByUserID(userID));
        }

        /// <summary>
        /// 删除委托信息
        /// </summary>
        /// <param name="delegation"></param>      
        public void DeleteUserDelegation(WfClientDelegation delegation)
        {
            this.SingleCall(action => action.DeleteUserDelegation(delegation));
        }
        #endregion

        #region Excel与Matrix
        public WfCreateClientDynamicProcessParams ExcelToWfCreateClientDynamicProcessParams(string processKey, Stream stream)
        {
            return this.SingleCall(action => action.ExcelToWfCreateClientDynamicProcessParams(processKey, stream));
        }

        public void ExcelToSaveDescriptor(string processKey, Stream stream)
        {
            this.SingleCall(action => action.ExcelToSaveDescriptor(processKey, stream));
        }

        public Stream WfDynamicProcessToExcel(string processKey)
        {
            return this.SingleCall(action => action.WfDynamicProcessToExcel(processKey));
        }
        #endregion

        #region 导入导出流程
        /// <summary>
        /// 将若干已经存在流程定义导出
        /// </summary>
        /// <param name="processKeys"></param>
        /// <returns></returns>
        public Stream ExportProcessDescriptors(params string[] processKeys)
        {
            return this.ExportProcessDescriptors(new WfClientExportProcessDescriptorParams(), processKeys);
        }

        /// <summary>
        /// 将若干已经存在流程定义导出
        /// </summary>
        /// <param name="exportParams"></param>
        /// <param name="processKeys"></param>
        /// <returns></returns>
        public Stream ExportProcessDescriptors(WfClientExportProcessDescriptorParams exportParams, params string[] processKeys)
        {
            exportParams.NullCheck("exportParams");
            processKeys.NullCheck("processKeys");

            return this.SingleCall(action => action.ExportProcessDescriptors(exportParams, processKeys));
        }

        /// <summary>
        /// 将流中的内容作为流程定义导入
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        public string ImportProcessDescriptors(Stream inputStream)
        {
            inputStream.NullCheck("inputStream");

            return this.SingleCall(action => action.ImportProcessDescriptors(inputStream));
        }

        /// <summary>
        /// 导出审批矩阵到Excel
        /// </summary>
        /// <param name="matrixID"></param>
        /// <returns></returns>
        public Stream ExportApprovalMatrix(string matrixID)
        {
            matrixID.CheckStringIsNullOrEmpty("matrixID");

            return this.SingleCall(action => action.ExportApprovalMatrix(matrixID));
        }

        /// <summary>
        /// 从Excel导入审批矩阵
        /// </summary>
        /// <param name="matrixID"></param>
        /// <param name="inputStream"></param>
        public void ImportApprovalMatrix(string matrixID, Stream inputStream)
        {
            inputStream.NullCheck("inputStream");

            this.SingleCall(action => action.ImportApprovalMatrix(matrixID, inputStream));

        }

        /// <summary>
        /// 删除审批矩阵
        /// </summary>
        /// <param name="matrixID"></param>
        public void DeleteApprovalMatrix(string matrixID)
        {
            matrixID.CheckStringIsNullOrEmpty("matrixID");

            this.SingleCall(action => action.DeleteApprovalMatrix(matrixID));
        }

        /// <summary>
        /// 审批矩阵是否存在
        /// </summary>
        /// <param name="matrixID"></param>
        /// <returns></returns>
        public bool ApprovalMatrixExists(string matrixID)
        {
            matrixID.CheckStringIsNullOrEmpty("matrixID");

            return this.SingleCall(action => action.ApprovalMatrixExists(matrixID));
        }
        #endregion

        protected override WfClientChannelFactory<IWfClientProcessDescriptorService> GetService()
        {
            EndpointAddress endPoint = new EndpointAddress(WfContractsProxySettings.GetConfig().ProcessDescriptorServiceUrl.ToString());

            return new WfClientChannelFactory<IWfClientProcessDescriptorService>(endPoint);
        }
    }
}
