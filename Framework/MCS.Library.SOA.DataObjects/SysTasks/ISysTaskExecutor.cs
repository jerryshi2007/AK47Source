using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 系统任务的执行器
	/// </summary>
	public interface ISysTaskExecutor
	{
		/// <summary>
		/// 执行之前的处理
		/// </summary>
		/// <param name="task"></param>
		void BeforeExecute(SysTask task);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="task"></param>
		/// <returns>已经完成的任务</returns>
		SysAccomplishedTask Execute(SysTask task);
	}
}
