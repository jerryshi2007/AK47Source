using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Workflow.Descriptors
{
    [DataContract]
    [Serializable]
    public class WfClientMainStreamActivityDescriptor
    {
        private WfClientActivityDescriptorCollection _AssociatedActivities = null;
        private WfClientAssigneeCollection _Assignees = null;
        private WfClientActivityStatus _Status = WfClientActivityStatus.NotRunning;

        public WfClientMainStreamActivityDescriptor()
        {
        }

        public WfClientMainStreamActivityDescriptor(WfClientActivityDescriptor actDesp)
        {
            actDesp.NullCheck("actDesp");

            this.Activity = actDesp;
        }

        /// <summary>
        /// 对应的活动描述
        /// </summary>
        public WfClientActivityDescriptor Activity
        {
            get;
            set;
        }

        public int Level
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

        /// <summary>
        /// 最后一次的活动的实例ID
        /// </summary>
        public string ActivityInstanceID
        {
            get;
            set;
        }

        public WfClientActivityDescriptorCollection AssociatedActivities
        {
            get
            {
                if (this._AssociatedActivities == null)
                    this._AssociatedActivities = new WfClientActivityDescriptorCollection();

                return this._AssociatedActivities;
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

        /// <summary>
        /// 活动的操作人
        /// </summary>
        public WfClientUser Operator
        {
            get;
            set;
        }

        /// <summary>
        /// 分支流程组的个数。大于0表示有分支流程
        /// </summary>
        public int BranchProcessGroupsCount
        {
            get;
            set;
        }

        /// <summary>
        /// 从哪一根线进入到这个活动
        /// </summary>
        public WfClientTransitionDescriptor FromTransitionDescriptor
        {
            get;
            set;
        }

        /// <summary>
        /// 这个活动出线是哪一条
        /// </summary>
        public WfClientTransitionDescriptor ToTransitionDescriptor
        {
            get;
            set;
        }
    }

    [DataContract]
    [Serializable]
    public class WfClientMainStreamActivityDescriptorCollection : SerializableEditableKeyedDataObjectCollectionBase<string, WfClientMainStreamActivityDescriptor>
    {
        public WfClientMainStreamActivityDescriptorCollection()
        {
        }

        protected WfClientMainStreamActivityDescriptorCollection(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }

        protected override string GetKeyForItem(WfClientMainStreamActivityDescriptor item)
        {
            return item.Activity.Key;
        }
    }
}
