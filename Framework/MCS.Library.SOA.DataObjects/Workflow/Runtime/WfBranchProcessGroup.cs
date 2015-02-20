using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[Serializable]
	[XElementSerializable]
	public class WfBranchProcessGroup : IWfBranchProcessGroup
	{
		[NonSerialized]
		private IWfActivity _OwnerActivity = null;

		[NonSerialized]
		private WfProcessCollection _Branches = null;

		[NonSerialized]
		private DataLoadingType _LoadingType;

		private string _OwnerActivityID = null;

		//private WfBranchProcessStatistics _BranchProcessStatistics = null;

		/// <summary>
		/// 构造方法。反序列化时，也是通过这个方法的
		/// </summary>
		public WfBranchProcessGroup()
		{
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="template"></param>
		public WfBranchProcessGroup(IWfActivity owner, IWfBranchProcessTemplateDescriptor template)
		{
			owner.NullCheck("owner");

			this.OwnerActivity = owner;
			this.ProcessTemplate = template;

			////从内存中构造时，会走这个构造方法。
			//this._BranchProcessStatistics = new WfBranchProcessStatistics();
		}

		#region IWfBranchProcessGroup Members

		[XElementFieldSerialize(AlternateFieldName = "Temp")]
		public IWfBranchProcessTemplateDescriptor ProcessTemplate
		{
			get;
			set;
		}

		///// <summary>
		///// 分支流程的统计信息，为了保持兼容性，对于老流程，会生成此属性，然后根据流程数目进行初始化
		///// </summary>
		//public WfBranchProcessStatistics BranchProcessStatistics
		//{
		//    get
		//    {
		//        if (this._BranchProcessStatistics == null)
		//        {
		//            this._BranchProcessStatistics = new WfBranchProcessStatistics();

		//            if (this.LoadingType == DataLoadingType.External)
		//                this._BranchProcessStatistics.SyncProcessesStatus(this.Branches);
		//        }

		//        return this._BranchProcessStatistics;
		//    }
		//}

		public IWfActivity OwnerActivity
		{
			get
			{
				if (this._OwnerActivity == null)
					this._OwnerActivity = LoadActivity(_OwnerActivityID);

				return this._OwnerActivity;
			}
			set
			{
				this._OwnerActivity = value;

				if (_OwnerActivity != null)
					this._OwnerActivityID = this._OwnerActivity.ID;
				else
					this._OwnerActivity = null;
			}
		}

		public WfProcessCollection Branches
		{
			get
			{
				//if (this._Branches == null)
				//{
				//    if (LoadingType == DataLoadingType.External)
				//        this._Branches = LoadBranches(this._OwnerActivity.ID, this.ProcessTemplate.Key, this);
				//    else
				//        this._Branches = new WfProcessCollection();
				//}
				if (LoadingType == DataLoadingType.External)
				{
					CheckProcessInstancesInBranches();
				}
				else
				{
					if (this._Branches == null)
						this._Branches = new WfProcessCollection();
				}

				return this._Branches;
			}
		}

		internal WfProcessCollection InternalBranches
		{
			get
			{
				return this._Branches;
			}
		}

		public DataLoadingType LoadingType
		{
			get
			{
				this._LoadingType = DataLoadingType.Memory;

				if (this._OwnerActivity != null)
					this._LoadingType = this._OwnerActivity.LoadingType;

				return this._LoadingType;
			}
		}

		//public bool IsBlocking()
		//{
		//    return this.BranchProcessStatistics.IsBlocking(this.ProcessTemplate.BlockingType);
		//}

		public bool IsBlocking()
		{
			bool result = true;

			this.CheckProcessesStatusInBranches();

			switch (this.ProcessTemplate.BlockingType)
			{
				case WfBranchProcessBlockingType.WaitAllBranchProcessesComplete:
					result = this.Branches.AllProcessedCompletedOrAborted() == false;
					break;
				case WfBranchProcessBlockingType.WaitNoneOfBranchProcessComplete:
					result = false;
					break;
				case WfBranchProcessBlockingType.WaitAnyoneBranchProcessComplete:
					result = this.Branches.AnyoneProcessCompleted() == false;
					break;
				case WfBranchProcessBlockingType.WaitSpecificBranchProcessesComplete:
					//todo://确定当前组中的所有流程，某些特定流程中的分支完成了，就算完成
					break;
				default:
					break;
			}

			return result;
		}
		#endregion

		/// <summary>
		/// 修改分支流程中的流程状态
		/// </summary>
		/// <param name="branchProcessID"></param>
		/// <param name="originalStatus"></param>
		/// <param name="newStatus"></param>
		/// <returns></returns>
		internal bool ChangeProcessStatus(string branchProcessID, WfProcessStatus originalStatus, WfProcessStatus newStatus)
		{
			bool result = false;

			if (originalStatus != newStatus)
			{
				result = true;

				CheckProcessesStatusInBranches();

				if (this._Branches != null && this._Branches.AllProcessesStatus != null)
				{
					if (this._Branches.AllProcessesStatus.ContainsKey(branchProcessID))
					{
						this._Branches.AllProcessesStatus[branchProcessID].Status = newStatus;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// 检查分支流程信息是否存在，且检查分支流程中是否有AllProcessesStatus
		/// </summary>
		internal void CheckProcessesStatusInBranches()
		{
			if (this.LoadingType == DataLoadingType.External)
			{
				if (this._Branches == null)
				{
					this._Branches = new WfProcessCollection();

					this._Branches.AllProcessesStatus = WfRuntime.GetProcessStatusByOwnerActivityID(this._OwnerActivity.ID, this.ProcessTemplate.Key, true);
				}
			}
		}

		/// <summary>
		/// 检查流程实例信息是否在Branches中
		/// </summary>
		private void CheckProcessInstancesInBranches()
		{
			if (this._Branches == null)
			{
				this._Branches = LoadBranches(this._OwnerActivity.ID, this.ProcessTemplate.Key, this);
			}
			else
			{
				if (this._Branches.AllProcessesStatus != null)
				{
					if (this._Branches.AllProcessesStatus.Count > 0 && this._Branches.Count == 0)
					{
						WfProcessCollection processLoaded = LoadBranches(this._OwnerActivity.ID, this.ProcessTemplate.Key, this);

						this._Branches.CopyFrom(processLoaded);
					}
				}
			}
		}

		private static WfProcessCollection LoadBranches(string ownerActID, string templateKey, IWfBranchProcessGroup group)
		{
			WfProcessCollection branches = WfRuntime.GetProcessByOwnerActivityID(ownerActID, templateKey);

			foreach (WfProcess process in branches)
			{
				process.EntryInfo = group;
			}

			branches.Sort((p1, p2) => p1.Sequence - p2.Sequence);

			return branches;
		}

		private static IWfActivity LoadActivity(string ownerActID)
		{
			IWfProcess process = WfRuntime.GetProcessByActivityID(ownerActID);

			return process.Activities[ownerActID];
		}
	}

	[Serializable]
	[XElementSerializable]
	public class WfBranchProcessGroupCollection : SerializableEditableKeyedDataObjectCollectionBase<string, IWfBranchProcessGroup>
	{
		protected override string GetKeyForItem(IWfBranchProcessGroup item)
		{
			return item.ProcessTemplate.Key;
		}

		public bool IsBlocking(WfBranchGroupBlockingType blockingType)
		{
			bool result = false;

			switch (blockingType)
			{
				case WfBranchGroupBlockingType.WaitAllBranchGroupsComplete:
					result = this.Exists(g => g.IsBlocking());
					break;
				case WfBranchGroupBlockingType.WaitAnyoneBranchGroupComplete:
					result = this.AllAndNotEmpty(g => g.IsBlocking());
					break;
			}

			return result;
		}
	}
}
