using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow.Tasks
{
	public class DispatchCancelBranchProcessesTask : InvokeProcessServiceTaskBase
	{
		public DispatchCancelBranchProcessesTask(string ownerSysActivityID, string ownerActivityID, bool cancelAllBranchProcesses)
			: base()
		{
			ownerActivityID.CheckStringIsNullOrEmpty("ownerActivityID");

			this.OwnerSysActivityID = ownerSysActivityID;
			this.OwnerActivityID = ownerActivityID;
			this.CancelAllBranchProcesses = cancelAllBranchProcesses;

			this.InitServiceDefinitions();
		}

		public DispatchCancelBranchProcessesTask(SysTask other) :
			base(other)
		{
		}

		/// <summary>
		/// 构造并且发送任务到任务列表中
		/// </summary>
		/// <param name="processID"></param>
		/// <param name="cancelAllBranchProcesses"></param>
		/// <returns></returns>
		public static DispatchCancelBranchProcessesTask SendTask(string ownerSysActivityID, string ownerActivityID, bool cancelAllBranchProcesses)
		{
			DispatchCancelBranchProcessesTask task = CreateTask(ownerSysActivityID, ownerActivityID, cancelAllBranchProcesses);

			InvokeServiceTaskAdapter.Instance.Update(task);

			return task;
		}

		public static DispatchCancelBranchProcessesTask CreateTask(string ownerSysActivityID, string ownerActivityID, bool cancelAllBranchProcesses)
		{
			DispatchCancelBranchProcessesTask task = new DispatchCancelBranchProcessesTask(ownerSysActivityID, ownerActivityID, cancelAllBranchProcesses);

			task.TaskID = UuidHelper.NewUuidString();
			task.ResourceID = ownerActivityID;
			task.TaskTitle = string.Format("作废活动下{0}的分支流程", ownerActivityID);

			return task;
		}

		protected override string GetOperationName()
		{
			return "DispatchCancelBranchProcessesTasks";
		}

		public string OwnerActivityID
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

		protected override void PrepareParameters(WfServiceOperationParameterCollection parameters)
		{
			WfServiceOperationParameter ownerSysActivityIDParam = new WfServiceOperationParameter("ownerSysActivityID", this.OwnerSysActivityID);

			parameters.Add(ownerSysActivityIDParam);

			WfServiceOperationParameter ownerActivityIDParam = new WfServiceOperationParameter("ownerActivityID", this.OwnerActivityID);

			parameters.Add(ownerActivityIDParam);

			WfServiceOperationParameter cancelAllBranchProcessesParam = new WfServiceOperationParameter("cancelAllBranchProcesses",
				WfSvcOperationParameterType.Boolean, this.CancelAllBranchProcesses);

			parameters.Add(cancelAllBranchProcessesParam);
		}
	}
}
