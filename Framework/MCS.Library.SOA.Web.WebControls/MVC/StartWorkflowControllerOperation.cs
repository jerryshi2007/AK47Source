using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Principal;
using MCS.Library.Core;

namespace MCS.Web.Library.MVC
{
	public class StartWorkflowControllerOperation : ControllerOperationBase
	{
		private WfProcessStartupParams _StartupParams = new WfProcessStartupParams();

		/// <summary>
		/// 流程是不是表单复制产生的
		/// </summary>
		public bool IsCloned
		{
			get;
			set;
		}

		public WfProcessStartupParams StartupParams
		{
			get
			{
				return this._StartupParams;
			}
		}

		public string ApplicationName
		{
			get;
			set;
		}

		public string ProgramName
		{
			get;
			set;
		}

		internal protected override void DoOperation()
		{
			StartupParams.ResourceID = UuidHelper.NewUuidString();

			if (this.ApplicationName.IsNotEmpty())
				StartupParams.ProcessDescriptor.ApplicationName = this.ApplicationName;

			if (this.ProgramName.IsNotEmpty())
				StartupParams.ProcessDescriptor.ProgramName = this.ProgramName;

			if (string.IsNullOrEmpty(StartupParams.ProcessDescriptor.Url))
				StartupParams.ProcessDescriptor.Url = WfClientContext.Current.EntryUri.ToString();

			if (StartupParams.ProcessDescriptor.InitialActivity != null)
				StartupParams.ProcessDescriptor.InitialActivity.Properties.SetValue("AutoSendUserTask", false);

			WfStartWorkflowExecutor executor = new WfStartWorkflowExecutor(WfClientContext.Current.OriginalActivity, StartupParams);

			executor.PrepareApplicationData += new ExecutorEventHandler(StartWorkflowExecutor_PrepareApplicationData);
			//需要触发相关的事件

			WfClientContext.Current.Execute(executor);

			PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("SetScene", () =>
				{
					if (WfClientContext.Current.CurrentActivity.Status == WfActivityStatus.Running)
						SetSceneByActivity(WfClientContext.Current.CurrentActivity);
					else
						SetReadOnlyScene(WfClientContext.Current.CurrentActivity);
				});

			//需要导航到目标视图
			TransferToTargetView();
		}

		private void StartWorkflowExecutor_PrepareApplicationData(WfExecutorDataContext dataContext)
		{
			if (this.IsCloned)
				((WfProcess)WfClientContext.Current.OriginalActivity.Process).LoadingType = DataLoadingType.Cloned;

			//dataContext.CurrentProcess.Committed = false;
			OnProcessReady(WfClientContext.Current.OriginalActivity.Process);
			OnPrepareCommandState(WfClientContext.Current.OriginalActivity.Process);
		}
	}
}
