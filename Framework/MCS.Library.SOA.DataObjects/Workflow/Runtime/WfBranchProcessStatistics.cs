using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 分支流程的统计信息
	/// </summary>
	[Serializable]
	[XElementSerializable]
	public class WfBranchProcessStatistics
	{
		private int _Total = 0;
		private int _Completed = 0;
		private int _Aborted = 0;
		private int _MaxSequence = 0;

		/// <summary>
		/// 分支流程的总数
		/// </summary>
		public int Total
		{
			get
			{
				return this._Total;
			}
			internal set
			{
				this._Total = value;
			}
		}

		/// <summary>
		/// 已经完成的
		/// </summary>
		public int Completed
		{
			get
			{
				return this._Completed;
			}
			internal set
			{
				this._Completed = value;
			}
		}

		/// <summary>
		/// 已经作废的
		/// </summary>
		public int Aborted
		{
			get
			{
				return this._Aborted;
			}
			internal set
			{
				this._Aborted = value;
			}
		}

		public int MaxSequence
		{
			get
			{
				return this._MaxSequence;
			}
			internal set
			{
				this._MaxSequence = value;
			}
		}

		/// <summary>
		/// 判断阻塞状态
		/// </summary>
		/// <param name="blockingType"></param>
		/// <returns></returns>
		public bool IsBlocking(WfBranchProcessBlockingType blockingType)
		{
			bool result = true;

			switch (blockingType)
			{
				case WfBranchProcessBlockingType.WaitAllBranchProcessesComplete:
					result = this.Completed + this.Aborted < this.Total;
					break;
				case WfBranchProcessBlockingType.WaitNoneOfBranchProcessComplete:
					result = false;
					break;
				case WfBranchProcessBlockingType.WaitAnyoneBranchProcessComplete:
					result = this.Completed == 0;
					break;
				case WfBranchProcessBlockingType.WaitSpecificBranchProcessesComplete:
					//todo://确定当前组中的所有流程，某些特定流程中的分支完成了，就算完成
					break;
				default:
					break;
			}

			return result;
		}

		/// <summary>
		/// 根据流程实例，初始化统计值。主要用于分支流程的初始化
		/// </summary>
		/// <param name="processes"></param>
		internal void SyncProcessesStatus(WfProcessCollection processes)
		{
			this.Total += processes.Count;

			foreach (IWfProcess process in processes)
			{
				IncreaseByStatus(process.Status);
			}

			this.MaxSequence = processes.GetMaxSequence();
		}

		/// <summary>
		/// 根据流程的状态变化，修改分支流程的统计计数
		/// </summary>
		/// <param name="originalStatus"></param>
		/// <param name="newStatus"></param>
		/// <returns>是否改变了统计值</returns>
		internal bool ChangeProcessStatus(WfProcessStatus originalStatus, WfProcessStatus newStatus)
		{
			bool changed = false;

			if (originalStatus != newStatus)
			{
				bool decChanged = DecreaseByStatus(originalStatus);
				bool incChanged = IncreaseByStatus(newStatus);

				changed = decChanged || incChanged;
			}

			return changed;
		}

		internal bool IncreaseByStatus(WfProcessStatus status)
		{
			bool changed = false;

			switch (status)
			{
				case WfProcessStatus.Aborted:
					if (this.Aborted < this.Total)
					{
						this.Aborted++;
						changed = true;
					}
					break;
				case WfProcessStatus.Completed:
					if (this.Completed < this.Total)
					{
						this.Completed++;
						changed = true;
					}
					break;
			}

			return changed;
		}

		internal bool DecreaseByStatus(WfProcessStatus status)
		{
			bool changed = false;

			switch (status)
			{
				case WfProcessStatus.Aborted:
					if (this.Aborted > 0)
					{
						this.Aborted--;
						changed = true;
					}
					break;
				case WfProcessStatus.Completed:
					if (this.Completed > 0)
					{
						this.Completed--;
						changed = true;
					}
					break;
			}

			return changed;
		}
	}
}

