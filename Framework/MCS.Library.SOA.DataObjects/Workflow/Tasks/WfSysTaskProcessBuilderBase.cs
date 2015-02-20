using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow.Tasks
{
	/// <summary>
	/// 任务流程构造器的基类
	/// </summary>
	public abstract class WfSysTaskProcessBuilderBase
	{
		public SysTaskProcess Build()
		{
			SysTaskProcess sysTaskProcess = CreateProcessInstance();

			SysTaskProcessRuntime.ProcessContext.AffectedProcesses.AddOrReplace(sysTaskProcess);

			this.AfterCreateProcessInstance(sysTaskProcess);

			SysTaskProcessRuntime.Persist();

			sysTaskProcess = SysTaskProcessRuntime.GetProcessByID(sysTaskProcess.ID);

			return sysTaskProcess;
		}

		protected virtual void AfterCreateProcessInstance(SysTaskProcess process)
		{
		}

		private SysTaskProcess CreateProcessInstance()
		{
			SysTaskProcess sysProcess = new SysTaskProcess();

			sysProcess.ID = UuidHelper.NewUuidString();
			sysProcess.Status = SysTaskProcessStatus.NotRunning;
			sysProcess.StartTime = DateTime.Now;

			return sysProcess;
		}
	}
}
