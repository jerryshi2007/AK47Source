using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Core;
using MCS.Library.Services;
using MCS.Library.SOA.DataObjects;

namespace ActivateWfProcessService
{
	public class ExecuteSysTasksThread : ThreadTaskBase
	{
		public override void OnThreadTaskStart()
		{
			SysTaskAdapter.Instance.FetchNotRuningSysTasks(this.Params.BatchCount, task =>
			{
				ExecuteTask(task);
			});
		}

		private static void ExecuteTask(SysTask task)
		{
			try
			{
				ISysTaskExecutor executor = SysTaskSettings.GetSettings().GetExecutor(task.TaskType);

				executor.BeforeExecute(task);
				Task.Factory.StartNew(() => executor.Execute(task));
			}
			catch (System.Exception ex)
			{
				SysTaskAdapter.Instance.MoveToCompletedSysTask(task.TaskID, SysTaskStatus.Aborted, ex.GetRealException().ToString());
			}
		}
	}
}
