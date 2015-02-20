using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Workflow.Runtime
{
    [DataContract]
    [Serializable]
    public class WfClientTransferParams : WfClientTransferParamsBase
    {
        private WfClientBranchProcessTransferParamsCollection _BranchTransferParams = null;

        public WfClientTransferParams()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nextActivityDescriptor"></param>
        public WfClientTransferParams(string nextActivityDescriptorKey)
            : base(nextActivityDescriptorKey)
        {
        }

        public WfClientBranchProcessTransferParamsCollection BranchTransferParams
        {
            get
            {
                if (this._BranchTransferParams == null)
                    this._BranchTransferParams = new WfClientBranchProcessTransferParamsCollection();

                return this._BranchTransferParams;
            }
        }
    }
}
