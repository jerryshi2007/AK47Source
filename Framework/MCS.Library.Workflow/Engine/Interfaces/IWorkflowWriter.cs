using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Workflow.Services;

namespace MCS.Library.Workflow.Engine
{
	/// <summary>
	/// �־û��������Ľӿڵض���
	/// </summary>
	public interface IWorkflowWriter
	{
		void SaveWorkItems(WorkItemBase[] items);
	}
}
