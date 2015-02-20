using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow
{
    [DataContract(IsReference= true)]
    [Serializable]
    

    public class WfClientProcessActionContext
    {
        private WfClientActivityChangingContext _ActivityChangingContext = null;
        private WfClientProcess _CurrentProcess = null;
        private WfClientActivity _OriginalActivity = null;
        private WfClientActivity _CurrentActivity = null;
        private string _Purpose = string.Empty;
        private Dictionary<string, object> _Dictionary = null;

        [DataMember]
        public WfClientActivityChangingContext ActivityChangingContext
        {
            get
            {
                if (this._ActivityChangingContext == null)
                {
                    this._ActivityChangingContext = new WfClientActivityChangingContext();
                }
                return this._ActivityChangingContext;
            }
            set
            {
                this._ActivityChangingContext = value;
            }
        }

        [DataMember]
        public WfClientProcess CurrentProcess
        {
            get
            {
                if (this._CurrentProcess == null)
                {
                    this._CurrentProcess = new WfClientProcess();
                }
                return this._CurrentProcess;
            }
            set
            {
                this._CurrentProcess = value;
            }
        }

        [DataMember]
        public WfClientActivity OriginalActivity
        {
            get
            {
                if (this._OriginalActivity == null)
                {
                    this._OriginalActivity = new WfClientActivity();
                }
                return this._OriginalActivity;
            }
            set { this._OriginalActivity = value; }
        }

        [DataMember]
        public WfClientActivity CurrentActivity
        {
            get 
            {
                if (this._CurrentActivity == null)
                {
                    this._CurrentActivity = new WfClientActivity();
                }
                return this._CurrentActivity;
            }
            set 
            {
                this._CurrentActivity = value;
            }
        }

        [DataMember]
        public ClientFileEmergency Emergency
        {
            get;
            set;
        }

        [DataMember]
        public string Purpose
        {
            get { return this._Purpose; }
            set { this._Purpose = value; }
        }

        /// <summary>
        /// 被作废的流程
        /// </summary>
       
        internal WfClientProcessCollection AbortedProcesses
        {
            get;
            set;
        }

        /// <summary>
        /// 已经完成的流程
        /// </summary>
        
        internal WfClientProcessCollection ClosedProcesses
        {
            get;
            set;
        }

        /// <summary>
        /// 保存前受影响的流程
        /// </summary>
       
        internal WfClientProcessCollection AffectedProcesses
        {
            get;
            set;
        }

        /// <summary>
        /// 保存前受影响的Action
        /// </summary>
        
        internal WfClientActionCollection AffectedActions
        {
            get;
            set;
        }

        /// <summary>
        /// 访问控制列表
        /// </summary>
        
        internal WfClientAclItemCollection Acl
        {
            get;
            set;
        }

        #region UserTask Properties
        [DataMember]
        internal ClientUserTaskCollection MoveToUserTasks
        {
            get;
            set;
        }

        internal ClientUserTaskCollection NotifyUserTasks
        {
            get;
            set;
        }

        internal ClientUserTaskCollection AccomplishedUserTasks
        {
            get;
            set;
        }

        internal ClientUserTaskCollection DeletedUserTasks
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// 附加项
        /// </summary>
        [DataMember]
        public Dictionary<string, object> Items
        {
            get 
            {
                if (this._Dictionary == null)
                {
                    this._Dictionary = new Dictionary<string, object>();
                }
                return this._Dictionary;
            }
            set
            {
                this._Dictionary = value;
            }
        }

        /// <summary>
        /// 流转时，目标节点是否可以继续流转。如果不能，那么目标节点是Pending状态
        /// </summary>
        [DataMember]
        public bool TargetActivityCanMoveTo
        {
            get;
            set;
        }
    }
}
