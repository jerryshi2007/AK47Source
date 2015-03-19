using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Transactions;
using MCS.Library.Data;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 基于Sql Server的流程定义管理器
    /// </summary>
    public class WfSqlProcessDescriptorManager : WfProcessDescriptorManagerBase
    {
        public override bool ExsitsProcessKey(string processKey)
        {
            return WfProcessDescriptorInfoAdapter.Instance.ExistsProcessKey(processKey);
        }

        /// <summary>
        /// 从数据库中加载Xml
        /// </summary>
        /// <param name="processKey"></param>
        /// <returns></returns>
        protected override XElement LoadXml(string processKey)
        {
            WfProcessDescriptorInfo info = WfProcessDescriptorInfoAdapter.Instance.Load(processKey);

            return XElement.Parse(info.Data);
        }

        /// <summary>
        /// 保存Xml到数据库
        /// </summary>
        /// <param name="processDesp"></param>
        /// <param name="xml"></param>
        protected override void SaveXml(IWfProcessDescriptor processDesp, XElement xml)
        {
            WfProcessDescriptorInfo info = WfProcessDescriptorInfo.FromProcessDescriptor(processDesp, xml);

            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                WfProcessDescriptorInfoAdapter.Instance.Update(info);

                WfProcessDescriptorDimensionAdapter.Instance.Update(WfProcessDescriptorDimension.FromProcessDescriptor(processDesp));

                scope.Complete();
            }
        }

        protected override void DeleteXml(string processKey)
        {
            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                WfProcessDescriptorInfoAdapter.Instance.Delete(processKey);
                WfMatrixAdapter.Instance.DeleteByProcessKey(processKey);
                WfProcessDescriptorDimensionAdapter.Instance.DeleteByProcessKey(processKey);

                scope.Complete();
            }
        }

        protected override void ClearAllXml()
        {
            WfProcessDescriptorInfoAdapter.Instance.ClearAll();
        }
    }
}
