using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using System.Data;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[Serializable]
	[XElementSerializable]
	[ORTableMapping("WF.PROCESS_CURRENT_ACTIVITIES")]
    [TenantRelativeObject]
	public class WfProcessCurrentActivity
	{
		[ORFieldMapping("PROCESS_ID", PrimaryKey = true)]
		public string ProcessID
		{
			get;
			set;
		}

		[ORFieldMapping("ACTIVITY_ID", PrimaryKey = true)]
		public string ActivityID
		{
			get;
			set;
		}

		[ORFieldMapping("ACTIVITY_DESC_KEY")]
		public string ActivityDescriptorKey
		{
			get;
			set;
		}

		[ORFieldMapping("ACTIVITY_TYPE")]
		public WfActivityType ActivityType
		{
			get;
			set;
		}

		[ORFieldMapping("ACTIVITY_NAME")]
		public string ActivityName
		{
			get;
			set;
		}

		[ORFieldMapping("STATUS")]
		[SqlBehavior(EnumUsage = EnumUsageTypes.UseEnumString)]
		public WfActivityStatus Status
		{
			get;
			set;
		}

		private IUser _Operator = null;

		[SubClassORFieldMapping("ID", "OPERATOR_ID")]
        [SubClassSqlBehavior("ID", ClauseBindingFlags.Insert)]
		[SubClassORFieldMapping("DisplayName", "OPERATOR_NAME")]
        [SubClassSqlBehavior("DisplayName", ClauseBindingFlags.Insert)]
		[SubClassORFieldMapping("FullPath", "OPERATOR_PATH")]
		[SubClassSqlBehavior("FullPath", ClauseBindingFlags.Insert)]
		[SubClassType(typeof(OguUser))]
		public IUser Operator
		{
			get
			{
				return this._Operator;
			}
			set
			{
				this._Operator = (IUser)OguUser.CreateWrapperObject(value);
			}
		}

		[ORFieldMapping("START_TIME")]
		public DateTime StartTime { get; set; }

		[ORFieldMapping("END_TIME")]
		public DateTime EndTime { get; set; }

		/// <summary>
		/// 从Activity构造WfProcessCurrentActivity对象
		/// </summary>
		/// <param name="activity"></param>
		/// <returns></returns>
		public static WfProcessCurrentActivity FromActivity(IWfActivity activity)
		{
			WfProcessCurrentActivity result = new WfProcessCurrentActivity();

			result.ProcessID = activity.Process.ID;
			result.ActivityID = activity.ID;
			result.ActivityDescriptorKey = activity.Descriptor.Key;
			result.ActivityType = activity.Descriptor.ActivityType;
			result.ActivityName = activity.Descriptor.Name;
			result.StartTime = activity.StartTime;
			result.EndTime = activity.EndTime;
			result.Status = activity.Status;
			result.Operator = activity.Operator;

			return result;
		}
	}


	[Serializable]
	[XElementSerializable]
	public class WfProcessCurrentActivityCollection : SerializableEditableKeyedDataObjectCollectionBase<string, WfProcessCurrentActivity>
	{
		internal void LoadFromDataView(DataView dv)
		{
			this.Clear();

			ORMapping.DataViewToCollection(this, dv);
		}

		protected override string GetKeyForItem(WfProcessCurrentActivity item)
		{
			return item.ActivityID;
		}
	}

}
