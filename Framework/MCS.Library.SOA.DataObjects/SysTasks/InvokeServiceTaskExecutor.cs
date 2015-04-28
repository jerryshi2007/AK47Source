using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 直接调用调用Web Service的Task的执行器。不需要任何Job
	/// </summary>
	public class InvokeServiceTaskExecutor : SyncSysTaskExecutorBase
	{
		protected override void OnExecute(SysTask task)
		{
			InvokeServiceTask invokeServiceTask = new InvokeServiceTask(task);

			WfServiceInvoker.InvokeContext.Clear();
			WfServiceInvoker.InvokeContext.CopyFrom(invokeServiceTask.Context);

			foreach (WfServiceOperationDefinition svcDefinition in invokeServiceTask.SvcOperationDefs)
			{
				WfServiceInvoker svcInvoker = new WfServiceInvoker(svcDefinition);

				svcInvoker.Headers.Set("SysTaskID", task.TaskID);

                svcInvoker.Invoke();
			}
		}
	}
}
