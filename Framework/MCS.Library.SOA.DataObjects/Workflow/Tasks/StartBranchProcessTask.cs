using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow.Tasks
{
	/// <summary>
	/// 启动分支流程的系统任务
	/// </summary>
	[Serializable]
	public class StartBranchProcessTask : InvokeProcessServiceTaskBase
	{
		public StartBranchProcessTask(string ownerActivityID, WfBranchProcessTransferParams transferParams)
			: base()
		{
			ownerActivityID.CheckStringIsNullOrEmpty("ownerActivityID");
			transferParams.NullCheck("transferParams");

			this.OwnerActivityID = ownerActivityID;
			this.TransferParams = transferParams;

			this.InitServiceDefinitions();
			this.Context["transferParams"] = this.TransferParams;
		}

		public StartBranchProcessTask(SysTask other) :
			base(other)
		{
		}

		/// <summary>
		/// 构造并且发送任务到任务列表中
		/// </summary>
		/// <param name="ownerActivityID"></param>
		/// <param name="transferParams"></param>
		/// <returns></returns>
		public static StartBranchProcessTask SendTask(string ownerActivityID, WfBranchProcessTransferParams transferParams)
		{
			StartBranchProcessTask task = CreateTask(ownerActivityID, transferParams);

			InvokeServiceTaskAdapter.Instance.Update(task);

			return task;
		}

		/// <summary>
		/// 构造发送流程的任务
		/// </summary>
		/// <param name="ownerActivityID"></param>
		/// <param name="transferParams"></param>
		/// <returns></returns>
		public static StartBranchProcessTask CreateTask(string ownerActivityID, WfBranchProcessTransferParams transferParams)
		{
			StartBranchProcessTask task = new StartBranchProcessTask(ownerActivityID, transferParams);

			task.TaskID = UuidHelper.NewUuidString();
			task.ResourceID = ownerActivityID;
			task.TaskTitle = string.Format("启动活动{0}的子流程", ownerActivityID);

			return task;
		}

		public string OwnerActivityID
		{
			get;
			private set;
		}

		public WfBranchProcessTransferParams TransferParams
		{
			get;
			private set;
		}

		protected override string GetOperationName()
		{
			return "StartBranchProcesses";
		}

		protected override void PrepareParameters(WfServiceOperationParameterCollection parameters)
		{
			WfServiceOperationParameter ownerActivityIDParam = new WfServiceOperationParameter("ownerActivityID", this.OwnerActivityID);

			parameters.Add(ownerActivityIDParam);

			WfServiceOperationParameter transferParams = new WfServiceOperationParameter("branchTransferParams", WfSvcOperationParameterType.RuntimeParameter, "transferParams");

			parameters.Add(transferParams);
		}
	}
}
