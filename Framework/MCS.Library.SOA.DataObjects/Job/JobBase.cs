using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects
{
	public enum JobType
	{
		[EnumItemDescription("启动流程")]
		StartWorkflow = 0,

		[EnumItemDescription("调用Web服务")]
		InvokeService = 1,
	}

	/// <summary>
	/// 执行状态
	/// </summary>
	public enum JobStatus
	{
		/// <summary>
		/// 等待执行
		/// </summary>
		[EnumItemDescription("等待中")]
		StandBy = 0,
		/// <summary>
		/// 正在运行中
		/// </summary>
		[EnumItemDescription("执行中")]
		Running = 1,
	}

	[Serializable]
	[ORTableMapping("WF.JOBS")]
    [TenantRelativeObject]
	public class JobBase
	{
		public JobBase()
		{
			this.Enabled = true;
		}

		[ORFieldMapping("JOB_ID", PrimaryKey = true)]
		public virtual string JobID { get; set; }

		[ORFieldMapping("JOB_NAME")]
		public virtual string Name { get; set; }

		[ORFieldMapping("JOB_CATEGORY")]
		public virtual string Category { get; set; }

		[ORFieldMapping("DESCRIPTION")]
		public virtual string Description { get; set; }

		[ORFieldMapping("ENABLED")]
		public virtual bool Enabled { get; set; }

		[ORFieldMapping("LAST_EXE_TIME")]
		public virtual DateTime? LastExecuteTime { get; set; }

		[ORFieldMapping("JOB_TYPE")]
		public virtual JobType JobType { get; set; }

		private JobStatus _Status = JobStatus.StandBy;
		[ORFieldMapping("JOB_STATUS")]
		public JobStatus Status
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

		[ORFieldMapping("LAST_START_EXE_TIME")]
		public DateTime? LastStartExecuteTime { get; set; }

		private IUser _Creator = null;

		[SubClassORFieldMapping("ID", "CREATOR_ID")]
		[SubClassORFieldMapping("DisplayName", "CREATOR_NAME")]
		[SubClassType(typeof(OguUser))]
		public virtual IUser Creator
		{
			get
			{
				return this._Creator;
			}
			set
			{
				this._Creator = (IUser)OguBase.CreateWrapperObject(value);
			}
		}

		private JobScheduleCollection _schedules;
		public JobScheduleCollection Schedules
		{
			get
			{
				if (this._schedules == null)
					this._schedules = new JobScheduleCollection();

				return this._schedules;
			}
			set
			{
				this._schedules = value;
			}
		}

		/*
		[NoMapping]
		public DateTime NextExecuteTime
		{
			get
			{
				DateTime result = DateTime.MaxValue;
				DateTime lastExeTime = LastExecuteTime ?? default(DateTime);

				foreach (JobSchedule schedule in Schedules)
				{
					DateTime scheduledTime = schedule.GetScheduleTime(lastExeTime);

					if (scheduledTime.CompareTo(result) < 0)
						result = scheduledTime;
				}

				return result;
			}
		}
		*/

		/// <summary>
		/// 转换成SysTask对象
		/// </summary>
		/// <returns></returns>
		public virtual SysTask ToSysTask()
		{
			SysTask task = new SysTask();

			task.TaskID = UuidHelper.NewUuidString();
			task.TaskTitle = this.Name;
			task.TaskType = this.JobType.ToString();
			task.ResourceID = this.JobID;
			task.Source = this.Creator;
			task.Category = this.Category;

			return task;
		}

		public virtual void Start()
		{
		}

		public void SetCurrentJobBeginStatus()
		{
			this.Status = JobStatus.Running;
			this.LastStartExecuteTime = DateTime.Now;

			JobBaseAdapter.Instance.SetStartJobStatus(this);
		}

		public void SetCurrentJobEndStatus()
		{
			this.LastExecuteTime = DateTime.Now;
			this.Status = JobStatus.StandBy;

			JobBaseAdapter.Instance.UpdateLastExeTime(this);
		}

		/// <summary>
		/// 是否可以执行
		/// </summary>
		/// <param name="timeOffset"></param>
		/// <returns></returns>
		public virtual bool CanStart(TimeSpan timeOffset)
		{
			JobSchedule matchedSchedule = null;

			return CanStart(timeOffset, out matchedSchedule);
		}

		/// <summary>
		/// 是否可以执行
		/// </summary>
		/// <param name="timeOffset"></param>
		/// <param name="matchedSchedule">返回匹配的计划对象</param>
		/// <returns></returns>
		public virtual bool CanStart(TimeSpan timeOffset, out JobSchedule matchedSchedule)
		{
			bool result = false;
			DateTime lastExeTime = LastStartExecuteTime ?? default(DateTime);
			DateTime now = DateTime.Now;
			matchedSchedule = null;

			foreach (JobSchedule schedule in Schedules)
			{
				result = schedule.Enabled && schedule.IsNextExecuteTime(lastExeTime, now, timeOffset);

				if (result)
				{
					matchedSchedule = schedule;
					break;
				}
			}

			return result;
		}

		//public virtual bool CanStart(TimeSpan timeOffset)
		//{
		//    DateTime nextExecuteTime = this.NextExecuteTime;

		//    bool result = nextExecuteTime != DateTime.MaxValue;

		//    if (result)
		//    {
		//        DateTime lowBound = nextExecuteTime - timeOffset;
		//        DateTime highBound = nextExecuteTime + timeOffset;

		//        DateTime now = DateTime.Now;

		//        result = (now >= lowBound && now <= highBound);
		//    }

		//    return result;
		//}

		public void InitJobBaseData(JobBase jobBase)
		{
			this.JobID = jobBase.JobID;
			this.Name = jobBase.Name;
			this.Description = jobBase.Description;
			this.Enabled = jobBase.Enabled;
			this.LastExecuteTime = jobBase.LastExecuteTime;
			this.LastStartExecuteTime = jobBase.LastStartExecuteTime;
			this.Category = jobBase.Category;

			this.JobType = jobBase.JobType;
			this.Creator = jobBase.Creator;
			this.Schedules = jobBase.Schedules;
		}
	}

	public class JobCollection : EditableKeyedDataObjectCollectionBase<string, JobBase>
	{
		protected override string GetKeyForItem(JobBase item)
		{
			return item.JobID;
		}
	}
}
