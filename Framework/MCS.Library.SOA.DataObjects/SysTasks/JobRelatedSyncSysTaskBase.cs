using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 任务相关的作业ID
	/// </summary>
	public abstract class JobRelatedSyncSysTaskBase : SyncSysTaskExecutorBase
	{
		protected override void OnExecute(SysTask task)
		{
			JobBase job = GetJobInfo(task);

			job.Start();
		}

		/// <summary>
		/// 得到Job信息
		/// </summary>
		/// <param name="task"></param>
		/// <returns></returns>
		protected abstract JobBase GetJobInfo(SysTask task);
	}
}
