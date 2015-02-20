using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Workflow.Services;

namespace MCS.Library.Workflow.Engine
{
	/// <summary>
	/// 持久化工作流的接口地定义
	/// </summary>
	public interface IWorkflowWriter
	{
		void SaveWorkItems(WorkItemBase[] items);
	}
}
