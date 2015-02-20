using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;
using System.Security.Permissions;
using MCS.Library.OGUPermission;
using MCS.Library.Workflow.Descriptors;
using MCS.Library.Workflow.Configuration;
using MCS.Library.Core;

namespace MCS.Library.Workflow.Engine
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public enum State
	{
		Added,
		Deleted,
		Modified,
		Unchanged
	}

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class WfBranchProcessInfo : ISerializable,IComparable<WfBranchProcessInfo>
	{
		private IWfProcess _Process = null;
		private IWfOperation _OwnerOperation = null;
		private string _OperationID = string.Empty;

		private int _Sequence = 0;
		private bool _IsSpecificProcess = false;
		private IWfProcessDescriptor _ProcessDescriptor = null;
		private string _ProcessDescriptorKey;

		private State _BranchInfoState = State.Added;

		private WfBranchProcessInfoContext _Context = null;

		private DataLoadingType _LoadingType = DataLoadingType.Memory;

		public WfBranchProcessInfo(IWfProcess process)
		{
			_Process = process;
		}

		#region ISerializable 成员

		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Process", this._Process, typeof(IWfProcess));
			info.AddValue("OwnerOperation", this._OwnerOperation, typeof(IWfOperation));
			info.AddValue("OperationID", _OperationID);
			info.AddValue("Sequence", this._Sequence);
			info.AddValue("IsSpecificProcess", this._IsSpecificProcess);
			info.AddValue("ProcessDescriptorKey", this._ProcessDescriptorKey);

			info.AddValue("BranchInfoState", this._BranchInfoState, typeof(State));

			info.AddValue("Context", _Context, typeof(WfBranchProcessInfoContext));

			info.AddValue("LoadingType", this._LoadingType, typeof(DataLoadingType));
		}

		protected WfBranchProcessInfo(SerializationInfo info, StreamingContext context)
		{
			this._Process = (IWfProcess)info.GetValue("Process", typeof(IWfProcess));
			this._OwnerOperation = (IWfOperation)info.GetValue("OwnerOperation", typeof(IWfOperation));
			this._OperationID = info.GetString("OperationID");
			this._Sequence = info.GetInt32("Sequence");
			this._IsSpecificProcess = info.GetBoolean("IsSpecificProcess");
			this._ProcessDescriptorKey = info.GetString("ProcessDescriptorKey");

			this._BranchInfoState = (State)info.GetValue("BranchInfoState", typeof(State));

			this._Context = (WfBranchProcessInfoContext)info.GetValue("Context", typeof(WfBranchProcessInfoContext));

			this._LoadingType = (DataLoadingType)info.GetValue("LoadingType", typeof(DataLoadingType));
		}

		#endregion

		#region Properties

		protected string ProcessDescriptorKey
		{
			get
			{
				return _ProcessDescriptorKey;
			}
			set
			{
				_ProcessDescriptorKey = value;
			}
		}

		protected string OperationID
		{
			get
			{
				return _OperationID;
			}
			set
			{
				_OperationID = value;
			}
		}

		public IWfProcess Process
		{
			get
			{
				return _Process;
			}
			protected set
			{
				_Process = value;
			}
		}

		public IWfOperation OwnerOperation
		{
			get
			{
				if (_OwnerOperation == null && LoadingType == DataLoadingType.External)
					this._OwnerOperation = LoadOperation();

				return _OwnerOperation;
			}
			set
			{
				_OwnerOperation = value;
			}
		}

		public WfBranchProcessInfoContext Context
		{
			get
			{
				if (_Context == null)
					if (LoadingType == DataLoadingType.External)
						_Context = LoadContext();
					else
						_Context = new WfBranchProcessInfoContext();

				return _Context;
			}
		}

		private WfBranchProcessInfoContext LoadContext()
		{
			WfBranchProcessInfoContext branchContext = null;
			IWorkflowReader persistProcess = WorkflowSettings.GetConfig().Reader;

			string strContext = persistProcess.GetBranchContext(this.OperationID, this.Process.ID);

			if (string.IsNullOrEmpty(strContext) == false)
				branchContext = SerializationHelper.DeserializeStringToObject<WfBranchProcessInfoContext>(strContext, SerializationFormatterType.Binary);
			else
				branchContext = new WfBranchProcessInfoContext();

			return branchContext;
		}

		private IWfOperation LoadOperation()
		{
			IWorkflowReader persistProcess = WorkflowSettings.GetConfig().Reader;

			return persistProcess.LoadOperationByBranchProcessID(this.Process.ID);
		}

		public int Sequence
		{
			get
			{
				return _Sequence;
			}
			set
			{
				_Sequence = value;
			}
		}

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
		/// 该属性用于记录所要预定义的流程（串行处理的时候可能会预先记录）
		/// </summary>
		public IWfProcessDescriptor ProcessDescriptor
		{
			get
			{
				/*
				if (this._ProcessDescriptor == null)
					this._ProcessDescriptor = WorkflowSettings.GetConfig().ProcessDescriptorManager.GetProcessDescriptor(this._ProcessDescriptorKey);
				*/

				if (this._ProcessDescriptor == null)
					this._ProcessDescriptor = WorkflowSettings.GetConfig().Reader.LoadProcessDescriptor(this._Process.ID, this._ProcessDescriptorKey);

				return _ProcessDescriptor;
			}
			set
			{
				_ProcessDescriptor = value;
			}
		}

		public WfAssigneeCollection BranchProcessReceiver
		{
			get
			{
				WfAssigneeCollection receivers = (WfAssigneeCollection)this.Context["BranchProcessReceiver"];

				if (receivers == null)
				{
					receivers = new WfAssigneeCollection();
					this.Context["BranchProcessReceiver"] = receivers;
				}

				return receivers;
			}
		}

		public State BranchInfoState
		{
			get { return _BranchInfoState; }
			set { _BranchInfoState = value; }
		}

		public DataLoadingType LoadingType
		{
			get
			{
				return _LoadingType;
			}
			protected set
			{
				_LoadingType = value;
			}
		}
		#endregion

        #region IComparable<WfBranchProcessInfo> 成员

        public int CompareTo(WfBranchProcessInfo other)
        {
           return Sequence.CompareTo(other.Sequence);
        }

        #endregion
    }
}
