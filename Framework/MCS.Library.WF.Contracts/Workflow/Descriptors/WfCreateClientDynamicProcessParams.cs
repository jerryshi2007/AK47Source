using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Workflow.Descriptors
{
    /// <summary>
    /// 创建具有一个动态活动的流程的参数
    /// </summary>
    [DataContract]
    [Serializable]
    public class WfCreateClientDynamicProcessParams : WfClientProcessDescriptorBase
    {
        private WfClientActivityMatrixResourceDescriptor _ActivityMatrix = null;

        /// <summary>
        /// 活动矩阵
        /// </summary>
        public WfClientActivityMatrixResourceDescriptor ActivityMatrix
        {
            get
            {
                if (this._ActivityMatrix == null)
                    this._ActivityMatrix = new WfClientActivityMatrixResourceDescriptor();

                return this._ActivityMatrix;
            }
            set
            {
                this._ActivityMatrix = value;
            }
        }
    }
}
