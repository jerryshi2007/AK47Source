using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Web.Library.Script;
using System.Transactions;
using MCS.Library.Data;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 包含了任务的活动
	/// </summary>
	[Serializable]
	[ORTableMapping("WF.SYS_TASK_ACTIVITY")]
	public class SysTaskActivity
	{
		private SysTaskActivityStatus _Status = SysTaskActivityStatus.NotRunning;

		[NonSerialized]
		private SysTaskProcessCollection _Branches = null;

		[NonSerialized]
		private SysTask _Task = null;

		private bool _Loaded = false;

		[NonSerialized]
		[ScriptIgnore]
		private SysTaskProcess _Process = null;

		public SysTaskActivity()
		{
		}

		public SysTaskActivity(SysTask task)
		{
			task.NullCheck("task");

			this._Task = task;
		}

		[ORFieldMapping("ID", PrimaryKey = true)]
		public string ID
		{
			get;
			set;
		}

		[ORFieldMapping("PROCESS_ID")]
		public string ProcessID
		{
			get;
			set;
		}

		[ORFieldMapping("NAME")]
		public string Name
		{
			get;
			set;
		}

		[ORFieldMapping("SEQUENCE")]
		public int Sequence
		{
			get;
			set;
		}

		[NoMapping]
		public SysTaskProcess Process
		{
			get
			{
				if (this._Process == null)
					this._Process = SysTaskProcessRuntime.GetProcessByID(this.ProcessID);

				return this._Process;
			}
			internal set
			{
				this._Process = value;
			}
		}

		[NoMapping]
		public SysTaskProcessCollection Branches
		{
			get
			{
				if (this._Branches == null)
				{
					if (this.Loaded == false)
						this._Branches = new SysTaskProcessCollection();
					else
						this._Branches = SysTaskProcessAdapter.Instance.LoadByOwnerActivityID(this.ID);
				}

				return this._Branches;
			}
		}

		[ORFieldMapping("STATUS")]
		[SqlBehavior(EnumUsageTypes.UseEnumString)]
		public SysTaskActivityStatus Status
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

		[ORFieldMapping("STATUS_TEXT")]
		public string StatusText
		{
			get;
			set;
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

		[ORFieldMapping("BLOCKING_TYPE")]
		[SqlBehavior(EnumUsageTypes.UseEnumString)]
		public SysTaskProcessBlockingType BlockingType
		{
			get;
			set;
		}

		[ScriptIgnore]
		[NoMapping]
		public SysTask Task
		{
			get
			{
				if (this._Task == null && this.TaskData.IsNotEmpty())
					this._Task = JSONSerializerExecute.Deserialize<SysTask>(this.TaskData);

				return this._Task;
			}
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

		/// <summary>
		/// 执行任务
		/// </summary>
		public void ExecuteTask()
		{
			this.Status = SysTaskActivityStatus.Running;

			if (this.Task != null)
			{
				try
				{
					//不需要保存在待办任务中
					//SysTaskAdapter.Instance.Update(this.Task);

					ISysTaskExecutor executor = SysTaskSettings.GetSettings().GetExecutor(this.Task.TaskType);

					SysAccomplishedTask accomplishedTask = executor.Execute(this.Task);

					//SysAccomplishedTask accomplishedTask = SysAccomplishedTaskAdapter.Instance.Load(this.Task.TaskID);

					if (accomplishedTask != null && accomplishedTask.Status == SysTaskStatus.Aborted)
						throw new ApplicationException(string.Format("执行ID为{0}的任务失败，错误为\n{1}",
							accomplishedTask.TaskID, accomplishedTask.StatusText));
				}
				catch (System.Exception ex)
				{
					DateTime now = DateTime.Now;

					this.Status = SysTaskActivityStatus.Aborted;
					this.EndTime = now;
					this.StatusText = ex.ToString();

					SysTaskProcessRuntime.ProcessContext.AffectedActivities.AddOrReplace(this);

					this.Process.Status = SysTaskProcessStatus.Aborted;
					this.Process.EndTime = now;
					SysTaskProcessRuntime.ProcessContext.AffectedProcesses.AddOrReplace(this.Process);
				}
			}
		}

		/// <summary>
		/// 是否可以流转
		/// </summary>
		/// <returns></returns>
		public bool CanMoveTo()
		{
			return this.IsBlocking() == false;
		}

		public bool IsBlocking()
		{
			bool result = false;

			switch (this.BlockingType)
			{
				case SysTaskProcessBlockingType.WaitAllBranchProcessesComplete:
					result = this.Branches.AllBranchProcessesCompleted() == false;
					break;
				case SysTaskProcessBlockingType.WaitAnyoneBranchProcessComplete:
					result = this.Branches.AnyoneBranchProcessCompleted() == false;
					break;
			}

			return result;
		}

		/// <summary>
		/// 序列化后的任务数据
		/// </summary>
		[ORFieldMapping("TASK_DATA")]
		internal string TaskData
		{
			get;
			set;
		}
	}

	[Serializable]
	public class SysTaskActivityCollection : SerializableEditableKeyedDataObjectCollectionBase<string, SysTaskActivity>
	{
		[NonSerialized]
		[ScriptIgnore]
		private SysTaskProcess _Process = null;

		public SysTaskActivityCollection()
		{
		}

		internal SysTaskActivityCollection(SysTaskProcess process)
		{
			this._Process = process;
		}

		public SysTaskActivityCollection(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}

		public void AddOrReplace(SysTaskActivity activity)
		{
			activity.NullCheck("activity");

			if (this.ContainsKey(activity.ID) == false)
				this.Add(activity);
		}

		protected override string GetKeyForItem(SysTaskActivity item)
		{
			return item.ID;
		}

		protected override void OnInsert(int index, object value)
		{
			if (this._Process != null)
				((SysTaskActivity)value).Process = this._Process;

			base.OnInsert(index, value);
		}

		protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete(index, value);

			if (this._Process != null)
				SysTaskProcessRuntime.ProcessContext.AffectedActivities.AddOrReplace((SysTaskActivity)value);
		}

		protected override void OnRemoveComplete(int index, object value)
		{
			base.OnRemoveComplete(index, value);

			if (this._Process != null)
				SysTaskProcessRuntime.ProcessContext.DeletedActivities.AddOrReplace((SysTaskActivity)value);
		}

		protected override void OnSet(int index, object oldValue, object newValue)
		{
			if (this._Process != null)
				((SysTaskActivity)newValue).Process = this._Process;

			base.OnSet(index, oldValue, newValue);
		}

		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			base.OnSetComplete(index, oldValue, newValue);

			if (this._Process != null)
			{
				SysTaskProcessRuntime.ProcessContext.DeletedActivities.AddOrReplace((SysTaskActivity)oldValue);
				SysTaskProcessRuntime.ProcessContext.AffectedActivities.AddOrReplace((SysTaskActivity)newValue);
			}
		}
	}

}
