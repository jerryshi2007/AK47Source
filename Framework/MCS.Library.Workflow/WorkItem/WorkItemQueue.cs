using System;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.Workflow.Services
{
	/// <summary>
	/// ���WorkItem�еĶ���
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
