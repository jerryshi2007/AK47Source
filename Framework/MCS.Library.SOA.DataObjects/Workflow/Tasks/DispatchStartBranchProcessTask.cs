using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow.Tasks
{
	[Serializable]
	public class DispatchStartBranchProcessTask : InvokeProcessServiceTaskBase
	{
		public DispatchStartBranchProcessTask(string ownerActivityID, IWfBranchProcessTemplateDescriptor template, bool autoAddExitMaintainingStatusTask)
			: base()
		{
			ownerActivityID.CheckStringIsNullOrEmpty("ownerActivityID");
			template.NullCheck("template");

			this.OwnerActivityID = ownerActivityID;
			this.Template = template;
			this.AutoAddExitMaintainingStatusTask = autoAddExitMaintainingStatusTask;

			this.InitServiceDefinitions();
			this.Context["template"] = this.Template;
		}

		public DispatchStartBranchProcessTask(SysTask other) :
			base(other)
		{
		}

		/// <summary>
		/// 构造并且发送任务到任务列表中
		/// </summary>
		/// <param name="ownerActivityID"></param>
		/// <param name="template"></param>
		/// <param name="autoAddExitMaintainingStatusTask"></param>
		/// <returns></returns>
		public static DispatchStartBranchProcessTask SendTask(string ownerActivityID, IWfBranchProcessTemplateDescriptor template, bool autoAddExitMaintainingStatusTask)
		{
			DispatchStartBranchProcessTask task = CreateTask(ownerActivityID, template, autoAddExitMaintainingStatusTask);

			InvokeServiceTaskAdapter.Instance.Update(task);

			return task;
		}

		public static DispatchStartBranchProcessTask CreateTask(string ownerActivityID, IWfBranchProcessTemplateDescriptor template, bool autoAddExitMaintainingStatusTask)
		{
			DispatchStartBranchProcessTask task = new DispatchStartBranchProcessTask(ownerActivityID, template, autoAddExitMaintainingStatusTask);

			task.TaskID = UuidHelper.NewUuidString();
			task.ResourceID = ownerActivityID;
			task.TaskTitle = string.Format("分发活动点{0}的子流程", ownerActivityID);

			return task;
		}

		public string OwnerActivityID
		{
			get;
			private set;
		}

		public IWfBranchProcessTemplateDescriptor Template
		{
			get;
			private set;
		}

		public bool AutoAddExitMaintainingStatusTask
		{
			get;
			private set;
		}

		protected override string GetOperationName()
		{
			return "DispatchStartBranchProcessTasks";
		}

		protected override void PrepareParameters(WfServiceOperationParameterCollection parameters)
		{
			WfServiceOperationParameter ownerActivityIDParam = new WfServiceOperationParameter("ownerActivityID", this.OwnerActivityID);

			parameters.Add(ownerActivityIDParam);

			WfServiceOperationParameter transferParams = new WfServiceOperationParameter("template", WfSvcOperationParameterType.RuntimeParameter, "template");

			parameters.Add(transferParams);

			WfServiceOperationParameter autoAddExitMaintainingStatusTaskParam = new WfServiceOperationParameter("autoAddExitMaintainingStatusTask", WfSvcOperationParameterType.Boolean, this.AutoAddExitMaintainingStatusTask);

			parameters.Add(autoAddExitMaintainingStatusTaskParam);
		}
	}
}
