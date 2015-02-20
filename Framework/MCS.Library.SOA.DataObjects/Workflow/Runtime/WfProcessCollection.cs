using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[Serializable]
	[XElementSerializable]
	public class WfProcessCollection : SerializableEditableKeyedDataObjectCollectionBase<string, IWfProcess>
	{
		private class WfProcessAndWeight
		{
			public WfProcessAndWeight()
			{
			}

			public WfProcessAndWeight(int weight, IWfProcess process)
			{
				this.Weight = weight;
				this.Process = process;
			}

			public int Weight
			{
				get;
				set;
			}

			public IWfProcess Process
			{
				get;
				set;
			}
		}

		public WfProcessCollection()
		{
		}

		public WfProcessCollection(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}

		/// <summary>
		/// 按照用户的相关性进行排序
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public WfProcessCollection SortByUserRelativity(IUser user)
		{
			List<WfProcessAndWeight> pws = new List<WfProcessAndWeight>();

			foreach (IWfProcess process in this)
			{
				int nWeight = 0;

				if (process.HasParentProcess == false)
					nWeight++;

				if (user != null && process.CurrentActivity != null && process.CurrentActivity.Assignees.Contains(user))
					nWeight += 2;

				pws.Add(new WfProcessAndWeight(nWeight, process));
			}

			pws.Sort((p1, p2) => p2.Weight - p1.Weight);

			WfProcessCollection result = new WfProcessCollection();

			pws.ForEach(pw => result.Add(pw.Process));

			return result;
		}

		/// <summary>
		/// 用于保存流程额外的状态信息，不包括流程反序列化的内容，仅包含状态信息
		/// </summary>
		[NonSerialized]
		private WfProcessCurrentInfoCollection _AllProcessesStatus = null;

		internal WfProcessCurrentInfoCollection AllProcessesStatus
		{
			get
			{
				return this._AllProcessesStatus;
			}
			set
			{
				this._AllProcessesStatus = value;
			}
		}

		protected override string GetKeyForItem(IWfProcess item)
		{
			return item.ID;
		}

		public void AddOrReplace(IWfProcess process)
		{
			process.NullCheck("process");

			if (this.ContainsKey(process.ID) == false)
				this.Add(process);
		}

		/// <summary>
		/// 得到流程集合中最大的序号
		/// </summary>
		/// <returns></returns>
		internal int GetMaxSequence()
		{
			int result = -1;

			foreach (IWfProcess process in this)
			{
				if (result < process.Sequence)
					result = process.Sequence;
			}

			return result;
		}

		/// <summary>
		/// 是否集合中所有流程都完成了
		/// </summary>
		/// <returns></returns>
		public bool AllProcessesCompleted()
		{
			bool result = false;

			if (this._AllProcessesStatus != null && this.Count == 0)
				result = this.AllProcessesCompletedByStatus();
			else
				result = this.AllProcessesCompletedByInstance();

			return result;
		}

		private bool AllProcessesCompletedByInstance()
		{
			//不存在流程状态不是完成的
			return (this.Exists(p => p.Status != WfProcessStatus.Completed)) == false;
		}

		private bool AllProcessesCompletedByStatus()
		{
			//不存在流程状态不是完成的
			return (this._AllProcessesStatus.Exists(p => this.GetStatus(p) != WfProcessStatus.Completed)) == false;
		}

		/// <summary>
		/// 是否集合中所有流程都完成或终止了
		/// </summary>
		/// <returns></returns>
		public bool AllProcessedCompletedOrAborted()
		{
			bool result = false;

			if (this._AllProcessesStatus != null && this.Count == 0)
				result = this.AllProcessedCompletedOrAbortedByStatus();
			else
				result = this.AllProcessedCompletedOrAbortedByInstance();

			return result;
		}

		private bool AllProcessedCompletedOrAbortedByInstance()
		{
			return (this.Exists(p => p.Status != WfProcessStatus.Completed && p.Status != WfProcessStatus.Aborted)) == false;
		}

		private bool AllProcessedCompletedOrAbortedByStatus()
		{
			return (this._AllProcessesStatus.Exists(p => this.GetStatus(p) != WfProcessStatus.Completed && this.GetStatus(p) != WfProcessStatus.Aborted)) == false;
		}

		/// <summary>
		/// 是否存在完成的流程
		/// </summary>
		/// <returns></returns>
		public bool AnyoneProcessCompleted()
		{
			bool result = false;

			if (this._AllProcessesStatus != null && this.Count == 0)
				result = this.AnyoneProcessCompletedByStatus();
			else
				result = this.AnyoneProcessCompletedByInstance();

			return result;
		}

		private bool AnyoneProcessCompletedByInstance()
		{
			return (this.Exists(p => p.Status == WfProcessStatus.Completed));
		}

		private bool AnyoneProcessCompletedByStatus()
		{
			return (this._AllProcessesStatus.Exists(p => this.GetStatus(p) == WfProcessStatus.Completed));
		}

		public List<string> FindProcessIDsByStatus(Predicate<WfProcessStatus> predicate)
		{
			predicate.NullCheck("predicate");

			List<string> result = null;

			if (this._AllProcessesStatus != null && this.Count == 0)
				result = FindStatusByProcessStatus(predicate);
			else
				result = FindStatusByProcessInstance(predicate);

			return result;
		}

		private List<string> FindStatusByProcessInstance(Predicate<WfProcessStatus> predicate)
		{
			List<string> result = new List<string>();

			foreach (IWfProcess process in this)
			{
				if (predicate(process.Status))
					result.Add(process.ID);
			}

			return result;
		}

		private List<string> FindStatusByProcessStatus(Predicate<WfProcessStatus> predicate)
		{
			List<string> result = new List<string>();

			foreach (WfProcessCurrentInfo info in this.AllProcessesStatus)
			{
				if (predicate(GetStatus(info)))
					result.Add(info.InstanceID);
			}

			return result;
		}

		private WfProcessStatus GetStatus(WfProcessCurrentInfo info)
		{
			WfProcessStatus status = info.Status;

			if (this.ContainsKey(info.InstanceID))
				status = this[info.InstanceID].Status;

			return status;
		}
	}
}
