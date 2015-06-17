using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 待持久化的流程信息队列信息
	/// </summary>
	[Serializable]
	[ORTableMapping("WF.PERSIST_QUEUE")]
    [TenantRelativeObject]
	public class WfPersistQueue
	{
		[ORFieldMapping("SORT_ID", IsIdentity = true, PrimaryKey = true)]
		public int SortID
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

		[ORFieldMapping("UPDATE_TAG")]
		public int UpdateTag
		{
			get;
			set;
		}

		[ORFieldMapping("CREATE_TIME")]
		[SqlBehavior(DefaultExpression = "GETDATE()")]
		public DateTime CreateTime
		{
			get;
			set;
		}

		[ORFieldMapping("PROCESS_TIME")]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Select | ClauseBindingFlags.Where)]
		public DateTime ProcessTime
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

		public static WfPersistQueue FromProcess(IWfProcess process)
		{
			process.NullCheck("process");

			WfPersistQueue result = new WfPersistQueue();

			result.ProcessID = process.ID;

			return result;
		}
	}

	[Serializable]
	public class WfPersistQueueCollection : EditableDataObjectCollectionBase<WfPersistQueue>
	{
	}
}
