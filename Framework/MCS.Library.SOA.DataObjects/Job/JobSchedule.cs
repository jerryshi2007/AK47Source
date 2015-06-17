using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MCS.Library.Core;
using System.Xml.Linq;
using System.Web.Script.Serialization;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 计划的类型
	/// </summary>
	public enum JobSchduleType
	{
		[EnumItemDescription("正常计划")]
		Normal = 0,

		[EnumItemDescription("临时计划")]
		Temporary = 1
	}

	[Serializable]
	[ORTableMapping("WF.JOB_SCHEDULE_DEF")]
    [TenantRelativeObject]
	public class JobSchedule
	{
		/// <summary>
		/// <remarks>此构造函数只是为适应JobScheduleAdapter</remarks>
		/// </summary>
		public JobSchedule()
		{
			this.Enabled = true;
		}

		public JobSchedule(string id, string name,
			DateTime startTime,
			JobScheduleFrequencyBase schFrequency)
			: this()
		{
			this.ID = id;
			this.Name = name;
			this.StartTime = startTime;
			this.SchduleType = JobSchduleType.Normal;
			this._scheduleFrequency = schFrequency;
		}

		public JobSchedule(string id, string name,
			DateTime startTime,
			DateTime endTime,
			JobScheduleFrequencyBase schFrequency)
			: this(id, name, startTime, schFrequency)
		{
			this.EndTime = endTime;
		}

		[ORFieldMapping("SCHEDULE_ID", PrimaryKey = true)]
		public string ID { get; set; }

		[ORFieldMapping("SCHEDULE_NAME")]
		public string Name { get; set; }

		[ORFieldMapping("SCHEDULE_TYPE")]
		public JobSchduleType SchduleType { get; set; }

		[ORFieldMapping("START_TIME")]
		public DateTime StartTime { get; set; }

		[ORFieldMapping("END_TIME")]
		public DateTime? EndTime { get; set; }

		[NoMapping]
		[ScriptIgnore]
		public DateTime NormalizedEndTime
		{
			get
			{
				return this.EndTime.HasValue ? this.EndTime.Value.Date.AddDays(1).AddSeconds(-1) : DateTime.MaxValue;
			}
		}

		[ORFieldMapping("ENABLED")]
		public bool Enabled { get; set; }

		private string _frequencyData = null;
		[ORFieldMapping("FREQUENCY_DATA")]
		public string FrequencyData
		{
			get
			{
				if (this._frequencyData.IsNullOrEmpty() && this._scheduleFrequency != null)
				{
					XElementFormatter formatter = new XElementFormatter();
					XElement element = formatter.Serialize(this._scheduleFrequency);

					this._frequencyData = element.ToString();
				}

				return this._frequencyData;
			}
			set
			{
				this._frequencyData = value;
				this._scheduleFrequency = null;
			}
		}

		[NoMapping]
		public string Description
		{
			get
			{
				string tempStr = string.Format("计划起始于{0}，", StartTime.ToString());

				if (EndTime != null)
				{
					tempStr = string.Format("计划在{0}至{1}期间运行，", StartTime.ToString(), EndTime.ToString());
				}

				return tempStr + ScheduleFrequency.Description;
			}
		}

		private JobScheduleFrequencyBase _scheduleFrequency;

		[NoMapping]
		public JobScheduleFrequencyBase ScheduleFrequency
		{
			get
			{
				if (this._scheduleFrequency == null && this._frequencyData.IsNotEmpty())
				{
					XElement element = XElement.Parse(this._frequencyData);
					this._scheduleFrequency = new XElementFormatter().Deserialize(element) as JobScheduleFrequencyBase;
				}

				return this._scheduleFrequency;
			}
			set
			{
				this._scheduleFrequency = value;
				this._frequencyData = null;
			}
		}

		/*
		public DateTime GetScheduleTime(DateTime lastExeTime)
		{
			DateTime result = DateTime.MaxValue;

			if (EndTime == null || DateTime.Now <= EndTime)
			{
				DateTime baseTime = lastExeTime;

				if (this.ScheduleFrequency.LastModifyTime > lastExeTime || lastExeTime < StartTime)
					baseTime = StartTime;

				result = ScheduleFrequency.CalculateDate(baseTime);
			}

			return result;
		}
		*/

		/// <summary>
		/// 检查checkPoint是否是下一个执行时间
		/// </summary>
		/// <param name="lastExeTime"></param>
		/// <returns></returns>
		public bool IsNextExecuteTime(DateTime lastExeTime, DateTime checkPoint, TimeSpan timeOffset)
		{
			bool result = false;

			if (this.StartTime < checkPoint && (this.EndTime == null || checkPoint < this.NormalizedEndTime))
			{
				if (this.ScheduleFrequency != null)
					result = this.ScheduleFrequency.IsNextExecuteTime(this.StartTime, lastExeTime, checkPoint, timeOffset);
			}

			return result;
		}

		/// <summary>
		/// 预估后面多少次的执行时间
		/// </summary>
		/// <param name="timeOffset"></param>
		/// <param name="maxCount"></param>
		/// <param name="timeout"></param>
		/// <returns></returns>
		public List<DateTime> EstimateExecuteTime(TimeSpan timeOffset, int maxCount, TimeSpan timeout)
		{
			List<DateTime> result = null;
			DateTime now = DateTime.Now;

			if (this.ScheduleFrequency != null && now >= this.StartTime && now < this.NormalizedEndTime)
				result = this.ScheduleFrequency.EstimateExecuteTime(this.StartTime, timeOffset, maxCount, timeout);
			else
				result = new List<DateTime>();

			return result;
		}
	}

	[Serializable]
	public class JobScheduleWithJobID : JobSchedule
	{
		[ORFieldMapping("JOB_ID")]
		public string JobID
		{
			get;
			set;
		}
	}

	[Serializable]
	public class JobScheduleCollection : EditableKeyedDataObjectCollectionBase<string, JobSchedule>
	{
		protected override string GetKeyForItem(JobSchedule item)
		{
			return item.ID;
		}
	}

	[Serializable]
	public class JobScheduleWithJobIDCollection : EditableDataObjectCollectionBase<JobScheduleWithJobID>
	{
	}
}
