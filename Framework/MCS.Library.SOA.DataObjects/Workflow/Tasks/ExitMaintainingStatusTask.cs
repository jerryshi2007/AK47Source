using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow.Tasks
{
	/// <summary>
	/// 退出流程的运维模式的任务
	/// </summary>
	public class ExitMaintainingStatusTask : InvokeProcessServiceTaskBase
	{
		public ExitMaintainingStatusTask(string processID, bool processPendingActivity)
			: base()
		{
			processID.CheckStringIsNullOrEmpty("processID");

			this.ProcessID = processID;
			this.ProcessPendingActivity = processPendingActivity;

			this.InitServiceDefinitions();
		}

		public ExitMaintainingStatusTask(SysTask other) :
			base(other)
		{
		}

		/// <summary>
		/// 构造并且发送任务到任务列表中
		/// </summary>
		/// <param name="currentActivityID"></param>
		/// <param name="processID"></param>
		/// <param name="processPendingActivity"></param>
		/// <returns></returns>
		public static ExitMaintainingStatusTask SendTask(string currentActivityID, string processID, bool processPendingActivity)
		{
			ExitMaintainingStatusTask task = CreateTask(currentActivityID, processID, processPendingActivity);

			InvokeServiceTaskAdapter.Instance.Update(task);

			return task;
		}

		public static ExitMaintainingStatusTask CreateTask(string currentActivityID, string processID, bool processPendingActivity)
		{
			ExitMaintainingStatusTask task = new ExitMaintainingStatusTask(processID, processPendingActivity);

			task.TaskID = UuidHelper.NewUuidString();
			task.ResourceID = currentActivityID;
			task.TaskTitle = string.Format("退出运维模式，流程ID为{0}", processID);

			return task;
		}

		public string ProcessID
		{
			get;
			private set;
		}

		public bool ProcessPendingActivity
		{
			get;
			private set;
		}

		protected override string GetOperationName()
		{
			return "ExitMaintainingStatus";
		}

		protected override void PrepareParameters(WfServiceOperationParameterCollection parameters)
		{
			WfServiceOperationParameter processIDParam = new WfServiceOperationParameter("processID", this.ProcessID);

			parameters.Add(processIDParam);

			WfServiceOperationParameter processPendingActivityParam = new WfServiceOperationParameter("processPendingActivity",
				WfSvcOperationParameterType.Boolean, this.ProcessPendingActivity);

			parameters.Add(processPendingActivityParam);
		}
	}
}
