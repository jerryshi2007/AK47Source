using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;

namespace MCS.OA.CommonPages.AppTrace
{
	public enum AssigneesFilterType
	{
		[EnumItemDescription("当前环节")]
		CurrentActivity = 0,

		[EnumItemDescription("所有环节")]
		AllActivities = 1
	}

	public enum ProcessFilterType
	{
		[EnumItemDescription("全部")]
		All = 0,

		[EnumItemDescription("当前环节人员异常")]
		CurrentActivityError = 1,

		[EnumItemDescription("当前环节和后续环节人员异常")]
		ExsitedActivitiesError = 2
	}

	[Serializable]
	public class ProcessQueryCondition
	{
		[ConditionMapping("APPLICATION_NAME", "Like")]
		public string ApplicationName { get; set; }

		[ConditionMapping("PROCESS_NAME", "Like")]
		public string ProcessName { get; set; }

        [ConditionMapping("DEPARTMENT_NAME", "Like")]
        public string DepartmentName { get; set; }

		[ConditionMapping("START_TIME", ">=")]
		public DateTime BeginStartTime { get; set; }

		[ConditionMapping("START_TIME", "<")]
		public DateTime EndStartTime { get; set; }

		[ConditionMapping("STATUS")]
		public string ProcessStatus { get; set; }

		[NoMapping]
		private ProcessFilterType _ProcessSelectType = ProcessFilterType.All;
		[NoMapping]
		public ProcessFilterType ProcessSelectType
		{
			get
			{
				return this._ProcessSelectType;
			}
			set
			{
				this._ProcessSelectType = value;
			}
		}

		private AssigneesFilterType _AssigneesSelectType = AssigneesFilterType.CurrentActivity;
		[NoMapping]
		public AssigneesFilterType AssigneesSelectType
		{
			get { return this._AssigneesSelectType; }
			set { this._AssigneesSelectType = value; }
		}

		[NoMapping]
		public string AssigneesUserName
		{
			get;
			set;
		}

		private OguDataCollection<IUser> _CurrentAssignees = null;

		[NoMapping]
		public OguDataCollection<IUser> CurrentAssignees
		{
			get
			{
				if (this._CurrentAssignees == null)
					this._CurrentAssignees = new OguDataCollection<IUser>();

				return this._CurrentAssignees;
			}
		}
	}
}