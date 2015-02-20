using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 与调用Web服务相关的执行任务
	/// </summary>
	public class InvokeServiceSyncTask : JobRelatedSyncSysTaskBase
	{
		protected override JobBase GetJobInfo(SysTask task)
		{
			InvokeWebServiceJob job = InvokeWebServiceJobAdapter.Instance.LoadSingleDataByJobID(task.ResourceID);

			(job != null).FalseThrow("不能找到ID为\"{0}\"的调用服务作业（Job）", task.ResourceID);

			WfServiceInvoker.InvokeContext.Clear();

			WfServiceInvoker.InvokeContext["taskID"] = task.TaskID;
			WfServiceInvoker.InvokeContext["jobID"] = job.JobID;

			return job;
		}
	}
}
