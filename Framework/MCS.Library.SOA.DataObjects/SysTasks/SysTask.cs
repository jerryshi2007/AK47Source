using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[ORTableMapping("WF.SYS_TASK")]
	public class SysTask : SysTaskBase
	{
		public SysTask()
		{
		}

		public SysTask(SysTaskBase other)
			: base(other)
		{
		}

		[ORFieldMapping("SORT_ID", IsIdentity = true)]
		public override int SortID
		{
			get
			{
				return base.SortID;
			}
			set
			{
				base.SortID = value;
			}
		}
	}

	/// <summary>
	/// 系统任务集合
	/// </summary>
	[Serializable]
	public class SysTaskCollection : EditableDataObjectCollectionBase<SysTask>
	{
	}
}
