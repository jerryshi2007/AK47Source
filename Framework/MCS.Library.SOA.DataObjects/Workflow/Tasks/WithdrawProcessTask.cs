using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow.Tasks
{
	[Serializable]
	public class WithdrawProcessTask : InvokeProcessServiceTaskBase
	{
		public WithdrawProcessTask(string processID, bool cancelAllBranchProcesses)
			: base()
		{
			processID.CheckStringIsNullOrEmpty("processID");

			this.ProcessID = processID;
			this.CancelAllBranchProcesses = cancelAllBranchProcesses;

			this.InitServiceDefinitions();
		}

		public WithdrawProcessTask(SysTask other) :
			base(other)
		{
		}

		/// <summary>
		/// 构造并且发送任务到任务列表中
		/// </summary>
		/// <param name="processID"></param>
		/// <param name="cancelAllBranchProcesses"></param>
		/// <returns></returns>
		public static WithdrawProcessTask SendTask(string processID, bool cancelAllBranchProcesses)
		{
			WithdrawProcessTask task = CreateTask(processID, cancelAllBranchProcesses);

			InvokeServiceTaskAdapter.Instance.Update(task);

			return task;
		}

		public static WithdrawProcessTask CreateTask(string processID, bool cancelAllBranchProcesses)
		{
			WithdrawProcessTask task = new WithdrawProcessTask(processID, cancelAllBranchProcesses);

			task.TaskID = UuidHelper.NewUuidString();
			task.ResourceID = processID;
			task.TaskTitle = string.Format("撤回流程{0}", processID);

			return task;
		}

		public string ProcessID
		{
			get;
			private set;
		}

		public bool CancelAllBranchProcesses
		{
			get;
			private set;
		}

		protected override string GetOperationName()
		{
			return "WithdrawProcess";
		}

		protected override void PrepareParameters(WfServiceOperationParameterCollection parameters)
		{
			WfServiceOperationParameter processIDParam = new WfServiceOperationParameter("processID", this.ProcessID);

			parameters.Add(processIDParam);

			WfServiceOperationParameter cancelAllBranchProcessesParam = new WfServiceOperationParameter("cancelAllBranchProcesses",
				WfSvcOperationParameterType.Boolean, this.CancelAllBranchProcesses);

			parameters.Add(cancelAllBranchProcessesParam);
		}
	}
}
