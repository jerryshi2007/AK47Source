using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[ORTableMapping("WF.SYS_ACCOMPLISHED_TASK")]
	public class SysAccomplishedTask : SysTaskBase
	{
		public SysAccomplishedTask()
		{
		}

		public SysAccomplishedTask(SysTaskBase other)
			: base(other)
		{
		}

		/// <summary>
		/// 结束时间
		/// </summary>
		[ORFieldMapping("END_TIME")]
		[SqlBehavior(DefaultExpression = "GETDATE()")]
		public override DateTime EndTime
		{
			get;
			set;
		}

		public SysTask CreateNewSystask(string id)
		{
			var result = new SysTask(this);

			result.TaskID = id;
			result.Status = SysTaskStatus.NotRunning;
			result.StatusText = string.Empty;
			result.StartTime = DateTime.MinValue;
			result.EndTime = DateTime.MinValue;

			return result;
		}
	}

	/// <summary>
	/// 已完成的系统任务集合
	/// </summary>
	[Serializable]
	public class SysAccomplishedTaskCollection : EditableDataObjectCollectionBase<SysAccomplishedTask>
	{
	}
}
