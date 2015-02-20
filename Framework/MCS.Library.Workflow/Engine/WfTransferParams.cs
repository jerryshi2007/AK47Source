using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Workflow.Descriptors;
using MCS.Library.Workflow.OguObjects;

namespace MCS.Library.Workflow.Engine
{
    public abstract class WfTransferParamsBase
    {
        private IWfActivityDescriptor _NextActivityDescriptor = null;
        private IUser _Operator = null;
		private WfAssigneeCollection _Receivers = null;
		private IWfTransitionDescriptor _FromTransitionDescriptor = null;

        public WfTransferParamsBase(IWfActivityDescriptor nextActivityDescriptor)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(nextActivityDescriptor != null, "nextActivityDescriptor");

            _NextActivityDescriptor = nextActivityDescriptor;
        }

        public IWfActivityDescriptor NextActivityDescriptor
        {
            get
            {
                return _NextActivityDescriptor;
            }
        }

        public IUser Operator
        {
            get
            {
                return _Operator;
            }
            set
            {
                _Operator = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
		public WfAssigneeCollection Receivers
        {
            get
            {
				if (this._Receivers == null)
					this._Receivers = new WfAssigneeCollection();

                return _Receivers;
            }
        }

		/// <summary>
		/// 来自于哪里的线的定义
		/// </summary>
		public IWfTransitionDescriptor FromTransitionDescriptor
		{
			get
			{
				return _FromTransitionDescriptor;
			}
			set
			{
				_FromTransitionDescriptor = value;
			}
		}
    }

    /// <summary>
    /// 
    /// </summary>
    public class WfTransferParams : WfTransferParamsBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nextActivityDescriptor"></param>
        public WfTransferParams(IWfActivityDescriptor nextActivityDescriptor)
            : base(nextActivityDescriptor)
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
	/*
    [Serializable]
    public abstract class WfReceiverParams
    {

    }*/

    /// <summary>
    /// 
    /// </summary>
    public class WfBranchesTransferParams : WfTransferParamsBase
    {
        private IWfOperationDescriptor _OperationDescriptor = null;
        private BranchesOperationalType _OperationalType = BranchesOperationalType.Parallel;
        private WfAssigneeCollection _AutoTransferReceivers = null;
        private WfBranchStartupParamsCollection<WfBranchStartupParams> _BranchParams
            = new WfBranchStartupParamsCollection<WfBranchStartupParams>();

        /// <summary>
        /// 
        /// </summary>
        public WfBranchStartupParamsCollection<WfBranchStartupParams> BranchParams
        {
            get 
            { 
                return _BranchParams; 
            }
            set
            { 
                _BranchParams = value; 
            }
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nextActivityDescriptor"></param>
        public WfBranchesTransferParams(IWfActivityDescriptor nextActivityDescriptor)
            : base(nextActivityDescriptor)
        {
        }

        /// <summary>
        /// 
        /// </summary>
		public IWfOperationDescriptor OperationDescriptor
        {
            get
            {
                return _OperationDescriptor;
            }
            set
            {
                _OperationDescriptor = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public BranchesOperationalType OperationalType
        {
            get
            {
                return _OperationalType;
            }
            set
            {
                _OperationalType = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public WfAssigneeCollection AutoTransferReceivers
        {
            get 
            {
				if (_AutoTransferReceivers == null)
					_AutoTransferReceivers = new WfAssigneeCollection();

                return _AutoTransferReceivers; 
            }
        }
    }
}
