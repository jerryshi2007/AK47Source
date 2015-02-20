using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using MCS.Library.OGUPermission;
using MCS.Library.Workflow.Descriptors;

namespace MCS.Library.Workflow.Engine
{
    public class WfProcessStartupParams
    {
        private IWfProcessDescriptor _Descriptor = null;
        private IOrganization _Department = null;
        private IUser _Operator = null;
        private string _ResourceID = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptor"></param>
        public WfProcessStartupParams(IWfProcessDescriptor descriptor)
        {
            _Descriptor = descriptor;
        }

        /// <summary>
        /// 
        /// </summary>
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
        public IOrganization Department
        {
            get
            {
                return _Department;
            }
            set
            {
                _Department = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IWfProcessDescriptor Descriptor
        {
            get
            {
                return _Descriptor;
            }
			set
			{
				_Descriptor = value;
			}
        }

        public string ResourceID
        {
            get 
            { 
                return _ResourceID; 
            }
            set 
            { 
                _ResourceID = value; 
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class WfBranchStartupParams : WfProcessStartupParams
    {
        private bool _IsSpecificProcess = false;
        private int _Sequence;
        private WfAssigneeCollection _BranchReceiverResource = null;

        public int Sequence
        {
            get { return _Sequence; }
            set { _Sequence = value; }
        }

        public WfAssigneeCollection BranchReceiverResource
        {
            get
            {
                if (_BranchReceiverResource == null)
                    _BranchReceiverResource = new WfAssigneeCollection();

                return _BranchReceiverResource;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSpecificProcess
        {
            get
            {
                return _IsSpecificProcess;
            }
            set
            {
                _IsSpecificProcess = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptor"></param>
        public WfBranchStartupParams(IWfProcessDescriptor descriptor)
            : base(descriptor)
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
	public class WfBranchStartupParamsCollection<T> : WfCollectionBase<T> where T : WfBranchStartupParams
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startupParams"></param>
        public void Add(WfBranchStartupParams startupParams)
        {
			InnerAdd((T)startupParams);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public WfBranchStartupParams this[int index]
        {
            get
            {
                return InnerGet(index);
            }
        }
    }
}
