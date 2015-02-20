using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow.Tasks
{
	[Serializable]
	public class DispatchCancelProcessTask : InvokeProcessServiceTaskBase
	{
		public DispatchCancelProcessTask(string ownerSysActivityID, string processID, bool cancelAllBranchProcesses)
			: base()
		{
			processID.CheckStringIsNullOrEmpty("processID");

			this.OwnerSysActivityID = ownerSysActivityID;
			this.ProcessID = processID;
			this.CancelAllBranchProcesses = cancelAllBranchProcesses;

			this.InitServiceDefinitions();
		}

		public DispatchCancelProcessTask(SysTask other) :
			base(other)
		{
		}

		/// <summary>
		/// 构造并且发送任务到任务列表中
		/// </summary>
		/// <param name="processID"></param>
		/// <param name="cancelAllBranchProcesses"></param>
		/// <returns></returns>
		public static DispatchCancelProcessTask SendTask(string ownerSysActivityID, string processID, bool cancelAllBranchProcesses)
		{
			DispatchCancelProcessTask task = CreateTask(ownerSysActivityID, processID, cancelAllBranchProcesses);

			InvokeServiceTaskAdapter.Instance.Update(task);

			return task;
		}

		public static DispatchCancelProcessTask CreateTask(string ownerSysActivityID, string processID, bool cancelAllBranchProcesses)
		{
			DispatchCancelProcessTask task = new DispatchCancelProcessTask(ownerSysActivityID, processID, cancelAllBranchProcesses);

			task.TaskID = UuidHelper.NewUuidString();
			task.ResourceID = processID;
			task.TaskTitle = string.Format("分发作废流程{0}的任务", processID);

			return task;
		}

		public string ProcessID
		{
			get;
			private set;
		}

		public bool CancelSelf
		{
			get;
			private set;
		}

		public bool CancelAllBranchProcesses
		{
			get;
			private set;
		}

		public string OwnerSysActivityID
		{
			get;
			private set;
		}

		protected override string GetOperationName()
		{
			return "DispatchCancelProcessTask";
		}

		protected override void PrepareParameters(WfServiceOperationParameterCollection parameters)
		{
			WfServiceOperationParameter ownerSysActivityIDParam = new WfServiceOperationParameter("ownerSysActivityID", this.OwnerSysActivityID);

			parameters.Add(ownerSysActivityIDParam);

			WfServiceOperationParameter processIDParam = new WfServiceOperationParameter("processID", this.ProcessID);

			parameters.Add(processIDParam);

			WfServiceOperationParameter cancelAllBranchProcessesParam = new WfServiceOperationParameter("cancelAllBranchProcesses",
				WfSvcOperationParameterType.Boolean, this.CancelAllBranchProcesses);

			parameters.Add(cancelAllBranchProcessesParam);
		}
	}
}
