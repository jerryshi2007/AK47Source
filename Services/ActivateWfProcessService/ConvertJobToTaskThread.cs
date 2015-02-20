using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Services;
using MCS.Library.SOA.DataObjects;

namespace ActivateWfProcessService
{
	/// <summary>
	/// 从Job中获取任务，将其转换成Task
	/// </summary>
	public class ConvertJobToTaskThread : ThreadTaskBase
	{
		public override void OnThreadTaskStart()
		{
			JobBaseAdapter.Instance.FetchNotDispatchedJobsAndConvertToTask(this.Params.BatchCount,
				TimeSpan.FromSeconds((this.Params.ActivateDuration.TotalSeconds / 2) * 1.1), null);
		}
	}
}
