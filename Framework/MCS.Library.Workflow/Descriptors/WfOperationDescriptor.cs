using System;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Security.Permissions;
using MCS.Library.Core;

namespace MCS.Library.Workflow.Descriptors
{
    /// <summary>
    /// 
    /// </summary>
    public enum BranchesOperationalType
    {
        Parallel,
        Serial
    }

    public enum AnchorOperationCompleteCondition
    {
        /// <summary>
        /// 等待全部分支流程完成
        /// </summary>
        WaitAllBranchProcessesComplete,

        /// <summary>
        /// 任何分支流程都不等待
        /// </summary>
        WaitNoneOfBranchProcessComplete,

        /// <summary>
        /// 等待任意一个分支流程完成
        /// </summary>
        WaitAnyoneBranchProcessComplete,

        /// <summary>
        /// 等待特定的某些分支流程完成
        /// </summary>
        WaitSpecificBranchProcessesComplete
    }

    
    [Serializable]
    public class WfOperationDescriptor : WfDescriptorBase, IWfOperationDescriptor
    {
        private BranchesOperationalType _OperationalType = BranchesOperationalType.Parallel;
        private string _DefaultBranchProcessDescKey = string.Empty;
        private bool _AutoTransferWhenCompleted = false;
        private AnchorOperationCompleteCondition _CompleteCondition = AnchorOperationCompleteCondition.WaitAllBranchProcessesComplete;
		private WfResourceDescriptorCollection _Resources = null;
        private WfVariableDescriptorCollection _Variables = new WfVariableDescriptorCollection();

        public WfOperationDescriptor() 
        { 
        }

        public WfOperationDescriptor(string key)
            : base(key)
        {
        }

        #region IWfOperationDescriptor

        /// <summary>
        /// 
        /// </summary>
        public BranchesOperationalType OperationalType
        {
            get { return _OperationalType; }
            set { _OperationalType = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string DefaultBranchProcessDescKey
        {
            get
            {
                return _DefaultBranchProcessDescKey;
            }
            set 
            {
                _DefaultBranchProcessDescKey = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public AnchorOperationCompleteCondition CompleteCondition
        {
            get 
            {
                return _CompleteCondition;
            }
            set 
            {
                _CompleteCondition = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public WfResourceDescriptorCollection Resources
        {
            get
            {
				if (_Resources == null)
					_Resources = CreateResourceCollection();

                return _Resources;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public WfVariableDescriptorCollection Variables
        {
            get
            {
                return _Variables;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AutoTransferWhenCompleted
        {
            get
            {
                return _AutoTransferWhenCompleted;
            }
            set
            {
                _AutoTransferWhenCompleted = value;
            }
        }

        #endregion

		protected virtual WfResourceDescriptorCollection CreateResourceCollection()
		{
			return new WfResourceDescriptorCollection();
		}

        #region ISerializable Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
 
            info.AddValue("DefaultBranchProcessDescKey", this.DefaultBranchProcessDescKey, typeof(string));
            info.AddValue("CompleteCondition", this.CompleteCondition, typeof(AnchorOperationCompleteCondition));
            info.AddValue("Resources", this.Resources, typeof(WfResourceDescriptorCollection));
            info.AddValue("Variables", this.Variables, typeof(WfVariableDescriptorCollection));
            info.AddValue("AutoTransferWhenCompleted", this.AutoTransferWhenCompleted);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected WfOperationDescriptor(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this._DefaultBranchProcessDescKey = (string)info.GetValue("DefaultBranchProcessDescKey", typeof(string));
            this._CompleteCondition = (AnchorOperationCompleteCondition)info.GetValue("CompleteCondition", typeof(AnchorOperationCompleteCondition));
            this._Resources = (WfResourceDescriptorCollection)info.GetValue("Resources",
                typeof(WfResourceDescriptorCollection));

            this._Variables = (WfVariableDescriptorCollection)info.GetValue("Variables", typeof(WfVariableDescriptorCollection));
            this._AutoTransferWhenCompleted = (bool)info.GetValue("AutoTransferWhenCompleted", typeof(bool));
        }

        #endregion
    }

    [Serializable]
    public class WfOperationDescriptorCollection : WfDescriptorCollectionBase<IWfOperationDescriptor>
    {
        public void Add(IWfOperationDescriptor item)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(item != null, "item");

            InnerAdd(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(key, "key");

			this.RWLock.AcquireWriterLock(lockTimeout);
			try
			{
				IWfOperationDescriptor operationDesp = this[key];

				Remove(operationDesp);
			}
			finally
			{
				this.RWLock.ReleaseWriterLock();
			}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationDesp"></param>
        public void Remove(IWfOperationDescriptor operationDesp)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(operationDesp != null, "operationDesp");

            this.InnerRemove(operationDesp);
        }
    }
}
