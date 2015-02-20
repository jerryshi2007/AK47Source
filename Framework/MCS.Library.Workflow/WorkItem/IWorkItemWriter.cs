using System;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.Workflow.Services
{
	/// <summary>
	/// WorkItem执行器接口，通常入库时执行
	/// </summary>
	public interface IWorkItemWriter
	{
		/// <summary>
		/// WorkItem持久化的执行方法
		/// </summary>
		/// <param name="item"></param>
		void Execute(WorkItemBase item);
	}
}
