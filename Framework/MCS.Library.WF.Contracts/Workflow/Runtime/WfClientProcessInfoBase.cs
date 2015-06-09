using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Descriptors;

namespace MCS.Library.WF.Contracts.Workflow.Runtime
{
    [DataContract]
    [Serializable]
    public abstract class WfClientProcessInfoBase
    {
        private Dictionary<string, object> _ProcessContext = null;
        private Dictionary<string, object> _ApplicationRuntimeParameters = null;
        private WfClientMainStreamActivityDescriptorCollection _MainStreamActivityDescriptors = null;
        private WfClientProcessStatus _Status = WfClientProcessStatus.NotRunning;

        public string ProcessDescriptorKey
        {
            get;
            set;
        }

        public string CurrentActivityKey
        {
            get;
            set;
        }

        public string ID
        {
            get;
            set;
        }

        public string SearchID
        {
            get;
            set;
        }

        public string RelativeID
        {
            get;
            set;
        }

        public string RelativeUrl
        {
            get;
            set;
        }

        public string ResourceID
        {
            get;
            set;
        }

        /// <summary>
        /// 流程逻辑上是否可以撤回。包括流程的状态判断以及活动点的状态判断（不含业务逻辑和管理逻辑）
        /// </summary>
        public bool CanWithdraw
        {
            get;
            set;
        }

        /// <summary>
        /// 流程逻辑上是否可以作废。包括流程的状态（不含业务逻辑和管理逻辑）
        /// </summary>
        public bool CanCancel
        {
            get;
            set;
        }

        /// <summary>
        /// 流程逻辑上是否可以暂停。包括流程的状态（不含业务逻辑和管理逻辑）
        /// </summary>
        public bool CanPause
        {
            get;
            set;
        }

        /// <summary>
        /// 流程逻辑上是否可以继续。包括流程的状态（不含业务逻辑和管理逻辑）
        /// </summary>
        public bool CanResume
        {
            get;
            set;
        }

        /// <summary>
        /// 流程逻辑上是否可以恢复。包括流程的状态（不含业务逻辑和管理逻辑）
        /// </summary>
        public bool CanRestore
        {
            get;
            set;
        }

        public WfClientProcessStatus Status
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

        public WfClientUser Creator
        {
            get;
            set;
        }

        public WfClientOrganization OwnerDepartment
        {
            get;
            set;
        }

        public string OwnerActivityID
        {
            get;
            set;
        }

        public string OwnerTemplateKey
        {
            get;
            set;
        }

        /// <summary>
        /// 流程的更新标记
        /// </summary>
        public int UpdateTag
        {
            get;
            set;
        }

        public bool Committed
        {
            get;
            set;
        }

        /// <summary>
        /// 运行时的流程名称
        /// </summary>
        public string RuntimeProcessName
        {
            get;
            set;
        }

        /// <summary>
        /// 流程的授权信息get
        /// </summary>
        public WfClientAuthorizationInfo AuthorizationInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 主线活动信息
        /// </summary>
        public WfClientMainStreamActivityDescriptorCollection MainStreamActivityDescriptors
        {
            get
            {
                if (this._MainStreamActivityDescriptors == null)
                    this._MainStreamActivityDescriptors = new WfClientMainStreamActivityDescriptorCollection();

                return this._MainStreamActivityDescriptors;
            }
        }

        public Dictionary<string, object> ApplicationRuntimeParameters
        {
            get
            {
                if (this._ApplicationRuntimeParameters == null)
                    this._ApplicationRuntimeParameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                return this._ApplicationRuntimeParameters;
            }
        }

        public Dictionary<string, object> ProcessContext
        {
            get
            {
                if (this._ProcessContext == null)
                    this._ProcessContext = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                return this._ProcessContext;
            }
        }

        public WfClientOpinion CurrentOpinion
        {
            get;
            set;
        }

        /// <summary>
        /// 是否有父流程
        /// </summary>
        public bool HasParentProcess
        {
            get
            {
                return this.OwnerActivityID.IsNotEmpty() && this.OwnerTemplateKey.IsNotEmpty();
            }
        }
    }

    /// <summary>
    /// 流程信息集合
    /// </summary>
    [DataContract]
    [Serializable]
    public abstract class WfClientProcessInfoCollectionBase<T> : SerializableEditableKeyedDataObjectCollectionBase<string, T> where T : WfClientProcessInfoBase
    {
        public WfClientProcessInfoCollectionBase()
        {
        }

        protected WfClientProcessInfoCollectionBase(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }

        protected override string GetKeyForItem(T item)
        {
            return item.ID;
        }
    }
}
