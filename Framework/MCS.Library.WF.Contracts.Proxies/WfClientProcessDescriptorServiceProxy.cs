using MCS.Library.Core;
using MCS.Library.WF.Contracts.Operations;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Proxies
{
    public class WfClientProcessDescriptorServiceProxy
    {
        public static readonly WfClientProcessDescriptorServiceProxy Instance = new WfClientProcessDescriptorServiceProxy();

        private WfClientProcessDescriptorServiceProxy()
        {
        }

        #region 流程
        public void DeleteDescriptor(string processDespKey)
        {
            ServiceProxy.SingleCall<IWfClientProcessDescriptorService>(WfClientFactory.GetProcessDescriptorService(),
                action => action.DeleteDescriptor(processDespKey));
        }

        public WfClientProcessDescriptor GetDescriptor(string processDespKey)
        {
            WfClientProcessDescriptor result = null;

            ServiceProxy.SingleCall<IWfClientProcessDescriptorService>(WfClientFactory.GetProcessDescriptorService(),
                action => result = action.GetDescriptor(processDespKey));

            return result;
        }

        public WfClientProcessDescriptor LoadDescriptor(string processDespKey)
        {
            WfClientProcessDescriptor result = null;

            ServiceProxy.SingleCall<IWfClientProcessDescriptorService>(WfClientFactory.GetProcessDescriptorService(),
                action => result = action.LoadDescriptor(processDespKey));

            return result;
        }

        public void SaveDescriptor(WfClientProcessDescriptor processDesp)
        {
            ServiceProxy.SingleCall<IWfClientProcessDescriptorService>(WfClientFactory.GetProcessDescriptorService(),
                action => action.SaveDescriptor(processDesp));
        }

        public WfClientProcessDescriptorInfoPageQueryResult QueryProcessDescriptorInfo(int startRowIndex, int maximumRows, string where, string orderBy, int totalCount)
        {
            WfClientProcessDescriptorInfoPageQueryResult result = null;

            ServiceProxy.SingleCall<IWfClientProcessDescriptorService>(WfClientFactory.GetProcessDescriptorService(),
              action => result = action.QueryProcessDescriptorInfo(startRowIndex, maximumRows, where, orderBy, totalCount));

            return result;
        }

        /// <summary>
        /// 判定流程KEY是否存在
        /// </summary>
        /// <param name="processKey">流程KEY</param>
        /// <returns>是否存在</returns>
        public bool ExsitsProcessKey(string processKey)
        {
            bool result = false;
            ServiceProxy.SingleCall<IWfClientProcessDescriptorService>(WfClientFactory.GetProcessDescriptorService(),
              action =>
              {
                  result = action.ExsitsProcessKey(processKey);
              });
            return result;
        }
        #endregion

        #region 委托信息
        public WfClientDelegationCollection LoadUserDelegations(string userID)
        {
            WfClientDelegationCollection result = null;

            ServiceProxy.SingleCall<IWfClientProcessDescriptorService>(WfClientFactory.GetProcessDescriptorService(), action =>
            {
                result = new WfClientDelegationCollection(action.LoadUserDelegations(userID));
            });

            return result;
        }

        public void UpdateUserDelegation(WfClientDelegation delegation)
        {
            ServiceProxy.SingleCall<IWfClientProcessDescriptorService>(WfClientFactory.GetProcessDescriptorService(),
                action => action.UpdateUserDelegation(delegation));
        }

        /// <summary>
        /// 按委托人删除其所有的委托信息
        /// </summary>
        /// <param name="userID"></param>
        public void DeleteUserDelegation(string userID)
        {
            ServiceProxy.SingleCall<IWfClientProcessDescriptorService>(WfClientFactory.GetProcessDescriptorService(),
                action => action.DeleteUserDelegationByUserID(userID));
        }

        /// <summary>
        /// 删除委托信息
        /// </summary>
        /// <param name="delegation"></param>      
        public void DeleteUserDelegation(WfClientDelegation delegation)
        {
            ServiceProxy.SingleCall<IWfClientProcessDescriptorService>(WfClientFactory.GetProcessDescriptorService(),
                action => action.DeleteUserDelegation(delegation));
        }
        #endregion

        #region Excel与Matrix
        public WfCreateClientDynamicProcessParams ExcelToWfCreateClientDynamicProcessParams(string processKey, Stream stream)
        {
            WfCreateClientDynamicProcessParams result = null;
            ServiceProxy.SingleCall<IWfClientProcessDescriptorService>(WfClientFactory.GetProcessDescriptorService(), action =>
            {
                result = action.ExcelToWfCreateClientDynamicProcessParams(processKey, stream);
            });
            return result;
        }


        public void ExcelToSaveDescriptor(string processKey, Stream stream)
        {
            ServiceProxy.SingleCall<IWfClientProcessDescriptorService>(WfClientFactory.GetProcessDescriptorService(),
                action => action.ExcelToSaveDescriptor(processKey, stream));
        }

        public Stream WfDynamicProcessToExcel(string processKey)
        {
            Stream result = null;

            ServiceProxy.SingleCall<IWfClientProcessDescriptorService>(WfClientFactory.GetProcessDescriptorService(),
                action => result = action.WfDynamicProcessToExcel(processKey));

            return result;
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

            Stream result = null;

            ServiceProxy.SingleCall<IWfClientProcessDescriptorService>(WfClientFactory.GetProcessDescriptorService(),
                action => result = action.ExportProcessDescriptors(exportParams, processKeys));

            return result;
        }

        /// <summary>
        /// 将流中的内容作为流程定义导入
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        public string ImportProcessDescriptors(Stream inputStream)
        {
            inputStream.NullCheck("inputStream");

            string result = null;

            ServiceProxy.SingleCall<IWfClientProcessDescriptorService>(WfClientFactory.GetProcessDescriptorService(),
                action => result = action.ImportProcessDescriptors(inputStream));

            return result;
        }
        #endregion
    }
}
