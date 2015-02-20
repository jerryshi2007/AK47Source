using System;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.Workflow.Services
{
	/// <summary>
	/// WorkItemִ�����ӿڣ�ͨ�����ʱִ��
	/// </summary>
	public interface IWorkItemWriter
	{
		/// <summary>
		/// WorkItem�־û���ִ�з���
		/// </summary>
		/// <param name="item"></param>
		void Execute(WorkItemBase item);
	}
}
