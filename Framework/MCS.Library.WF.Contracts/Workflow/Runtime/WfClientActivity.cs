using MCS.Library.Data.DataObjects;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
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
    public class WfClientActivity
    {
        private WfClientActivityStatus _Status = WfClientActivityStatus.NotRunning;
        private WfClientDataLoadingType _LoadingType = WfClientDataLoadingType.Memory;
        private WfClientAssigneeCollection _Candidates = null;
        private WfClientAssigneeCollection _Assignees = null;
        private WfClientBranchProcessReturnType _BranchProcessReturnValue = WfClientBranchProcessReturnType.AllFalse;

        public string ID
        {
            get;
            set;
        }

        public WfClientActivityDescriptor Descriptor
        {
            get;
            set;
        }

        public string DescriptorKey
        {
            get;
            set;
        }

        /// <summary>
        /// 分支流程组的个数，如果大于0，则表示有分支流程
        /// </summary>
        public int BranchProcessGroupsCount
        {
            get;
            set;
        }

        public WfClientActivityStatus Status
        {
            get
            {
                return this._Status;
            }
            set
            {
                this._Status = value;
            }
        }

        public DateTime StartTime
        {
            get;
            set;
        }

        public DateTime EndTime
        {
            get;
            set;
        }

        public WfClientDataLoadingType LoadingType
        {
            get
            {
                return this._LoadingType;
            }
            set
            {
                this._LoadingType = value;
            }
        }

        /// <summary>
        /// 该环节的候选人
        /// </summary>
        public WfClientAssigneeCollection Candidates
        {
            get
            {
                if (this._Candidates == null)
                    this._Candidates = new WfClientAssigneeCollection();

                return this._Candidates;
            }
        }

        public WfClientAssigneeCollection Assignees
        {
            get
            {
                if (this._Assignees == null)
                    this._Assignees = new WfClientAssigneeCollection();

                return this._Assignees;
            }
        }

        public WfClientUser Operator
        {
            get;
            set;
        }

        /// <summary>
        /// 本活动对应着主线活动的描述的Key
        /// </summary>
        public string MainStreamActivityKey
        {
            get;
            set;
        }

        /// <summary>
        /// 分支流程的返回结果
        /// </summary>
        public WfClientBranchProcessReturnType BranchProcessReturnValue
        {
            get
            {
                return this._BranchProcessReturnValue;
            }
            set
            {
                this._BranchProcessReturnValue = value;
            }
        }
    }

    [DataContract]
    [Serializable]
    public class WfClientActivityCollection : SerializableEditableKeyedDataObjectCollectionBase<string, WfClientActivity>
    {
        protected override string GetKeyForItem(WfClientActivity item)
        {
            return item.ID;
        }
    }
}
