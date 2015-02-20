using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	public class StartWorkflowSyncTask : JobRelatedSyncSysTaskBase
	{
		protected override JobBase GetJobInfo(SysTask task)
		{
			JobBase job = StartWorkflowJobAdapter.Instance.LoadSingleDataByJobID(task.ResourceID);

			(job != null).FalseThrow("不能找到ID为\"{0}\"的启动流程作业（Job）", task.ResourceID);

			return job;
		}
	}
}
