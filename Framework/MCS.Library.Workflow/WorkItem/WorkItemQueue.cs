using System;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.Workflow.Services
{
	/// <summary>
	/// 存放WorkItem中的队列
	/// </summary>
	[Serializable]
	public class WorkItemQueue : Queue<WorkItemBase>
	{
		internal WorkItemQueue()
			: base()
		{
		}
	}
}
