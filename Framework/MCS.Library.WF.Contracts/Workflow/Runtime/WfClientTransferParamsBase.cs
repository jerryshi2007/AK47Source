using MCS.Library.WF.Contracts.Ogu;
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
    public abstract class WfClientTransferParamsBase
    {
        private WfClientAssigneeCollection _Assignees = null;

        public WfClientTransferParamsBase()
        {
        }

        public WfClientTransferParamsBase(string nextActivityDescriptorKey)
        {
            this.NextActivityDescriptorKey = nextActivityDescriptorKey;
        }

        public string NextActivityDescriptorKey
        {
            get;
            set;
        }

        public string FromTransitionDescriptorKey
        {
            get;
            set;
        }

        public WfClientUser Operator
        {
            get;
            set;
        }

        public WfClientAssigneeCollection Assignees
        {
            get
            {
                if (this._Assignees == null)
                    this._Assignees = new WfClientAssigneeCollection();

                return _Assignees;
            }
        }
    }
}
