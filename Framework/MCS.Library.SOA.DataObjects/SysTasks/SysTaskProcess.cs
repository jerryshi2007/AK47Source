using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 包含一组顺序执行的Task的流程
	/// </summary>
	[Serializable]
	[ORTableMapping("WF.SYS_TASK_PROCESS")]
	public class SysTaskProcess
	{
		private SysTaskProcessStatus _Status = SysTaskProcessStatus.NotRunning;
		private int _CurrentActivityIndex = 0;
		private bool _Loaded = false;

		[NonSerialized]
		private SysTaskActivity _OwnerActivity = null;

		[NonSerialized]
		private SysTaskActivityCollection _Activities = null;

		/// <summary>
		/// 流程的ID
		/// </summary>
		[ORFieldMapping("ID", PrimaryKey = true)]
		public string ID
		{
			get;
			set;
		}

		/// <summary>
		/// 流程的名称
		/// </summary>
		[ORFieldMapping("NAME")]
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// 相关资源ID
		/// </summary>
		[ORFieldMapping("RESOURCE_ID")]
		public string ResourceID
		{
			get;
			set;
		}

		/// <summary>
		/// 父流程活动的ID
		/// </summary>
		[ORFieldMapping("OWNER_ACTIVITY_ID")]
		public string OwnerActivityID
		{
			get;
			set;
		}

		[ORFieldMapping("STATUS")]
		[SqlBehavior(EnumUsageTypes.UseEnumString)]
		public SysTaskProcessStatus Status
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

		[ORFieldMapping("CURRENT_ACTIVITY_INDEX")]
		public int CurrentActivityIndex
		{
			get
			{
				return this._CurrentActivityIndex;
			}
			set
			{
				this._CurrentActivityIndex = value;
			}
		}

		[ORFieldMapping("SEQUENCE")]
		public int Sequence
		{
			get;
			set;
		}

		[NoMapping]
		public bool Loaded
		{
			get
			{
				return this._Loaded;
			}
			internal set
			{
				this._Loaded = value;
			}
		}

		[ORFieldMapping("START_TIME")]
		public DateTime StartTime
		{
			get;
			set;
		}

		[ORFieldMapping("END_TIME")]
		public DateTime EndTime
		{
			get;
			set;
		}

		[ORFieldMapping("UPDATE_TAG")]
		[SqlBehavior(ClauseBindingFlags.All & ~ClauseBindingFlags.Insert)]
		public int UpdateTag
		{
			get;
			set;
		}

		[ORFieldMapping("STATUS_TEXT")]
		public string StatusText
		{
			get;
			set;
		}

		[NoMapping]
		public SysTaskActivity CurrentActivity
		{
			get
			{
				return this.Activities[this._CurrentActivityIndex];
			}
		}

		[NoMapping]
		public SysTaskActivity OwnerActivity
		{
			get
			{
				if (this._OwnerActivity == null)
				{
					if (this.OwnerActivityID.IsNotEmpty())
					{
						SysTaskProcess process = SysTaskProcessAdapter.Instance.LoadByActivityID(this.OwnerActivityID);

						this._OwnerActivity = process.Activities[this.OwnerActivityID];
					}
				}

				return this._OwnerActivity;
			}
		}

		[NoMapping]
		public SysTaskActivityCollection Activities
		{
			get
			{
				if (this._Activities == null)
				{
					if (this.Loaded)
						this._Activities = SysTaskActivityAdapter.Instance.LoadByProcessID(this.ID);
					else
						this._Activities = new SysTaskActivityCollection(this);
				}

				return this._Activities;
			}
		}

		/// <summary>
		/// 流转到下一步活动。如果已经到了最后一个活动，则更新流程的状态为已完成。
		/// </summary>
		/// <returns>返回下一个活动，如果已经到了最后一个活动。则只返回最后一个活动</returns>
		public SysTaskActivity MoveToNextActivity()
		{
			DateTime now = DateTime.Now;
			this.Status = SysTaskProcessStatus.Running;

			if (this._CurrentActivityIndex + 1 < this.Activities.Count)
			{
				this.CurrentActivity.EndTime = now;
				this.CurrentActivity.Status = SysTaskActivityStatus.Completed;

				SysTaskProcessRuntime.ProcessContext.AffectedActivities.AddOrReplace(this.CurrentActivity);

				this._CurrentActivityIndex++;

				EnterNextActivity(this.CurrentActivity, now);
			}
			else
			{
				this.EndTime = now;
				this.Status = SysTaskProcessStatus.Completed;
			}

			SysTaskProcessRuntime.ProcessContext.AffectedProcesses.AddOrReplace(this);

			return this.CurrentActivity;
		}

		/// <summary>
		/// 处理进入下一个活动的操作
		/// </summary>
		/// <param name="nextActivity"></param>
		private static void EnterNextActivity(SysTaskActivity nextActivity, DateTime now)
		{
			nextActivity.StartTime = now;

			if (nextActivity.CanMoveTo())
				nextActivity.Status = SysTaskActivityStatus.Running;
			else
				nextActivity.Status = SysTaskActivityStatus.Pending;

			foreach (SysTaskProcess branch in nextActivity.Branches)
			{
				if (branch.Activities.Count == 0)
					branch.Status = SysTaskProcessStatus.Completed;
				else
				if (branch.Activities.Count > 0)
					ExecuteSysTaskActivityTask.SendTask(branch.Activities[0]);
			}

			SysTaskProcessRuntime.ProcessContext.AffectedActivities.AddOrReplace(nextActivity);
		}

		internal void NormalizeActivities()
		{
			if (this._Activities != null)
			{
				for (int i = 0; i < this._Activities.Count; i++)
				{
					SysTaskActivity activity = this._Activities[i];

					activity.ProcessID = this.ID;
					activity.Sequence = i;
				}
			}
		}
	}

	[Serializable]
	public class SysTaskProcessCollection : SerializableEditableKeyedDataObjectCollectionBase<string, SysTaskProcess>
	{
		public SysTaskProcessCollection()
		{
		}

		public SysTaskProcessCollection(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}

		public void AddOrReplace(SysTaskProcess process)
		{
			process.NullCheck("process");

			if (this.ContainsKey(process.ID) == false)
				this.Add(process);
		}

		public bool AllBranchProcessesCompleted()
		{
			return this.All(p => p.Status == SysTaskProcessStatus.Completed);
		}

		public bool AnyoneBranchProcessCompleted()
		{
			return this.Count == 0 || this.Exists(p => p.Status == SysTaskProcessStatus.Completed);
		}

		protected override string GetKeyForItem(SysTaskProcess item)
		{
			return item.ID;
		}
	}
}
