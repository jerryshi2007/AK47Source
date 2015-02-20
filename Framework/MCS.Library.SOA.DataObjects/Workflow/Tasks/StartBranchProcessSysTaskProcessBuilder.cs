using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow.Tasks
{
	/// <summary>
	/// 创建启动分支流程的任务流程的创建器
	/// </summary>
	public class StartBranchProcessSysTaskProcessBuilder : WfSysTaskProcessBuilderBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ownerActivityID">分支流程所挂接的活动</param>
		/// <param name="template">分支流程模板</param>
		/// <param name="autoAddExitMaintainingStatusTask">是否在最后补充一个退出维护模式的任务</param>
		public StartBranchProcessSysTaskProcessBuilder(string ownerActivityID, IWfBranchProcessTemplateDescriptor template, bool autoAddExitMaintainingStatusTask)
		{
			ownerActivityID.CheckStringIsNullOrEmpty("ownerActivityID");
			template.NullCheck("template");

			this.OwnerActivityID = ownerActivityID;
			this.Template = template;
			this.AutoAddExitMaintainingStatusTask = autoAddExitMaintainingStatusTask;
		}

		public IWfBranchProcessTemplateDescriptor Template
		{
			get;
			private set;
		}

		public string OwnerActivityID
		{
			get;
			private set;
		}

		public bool AutoAddExitMaintainingStatusTask
		{
			get;
			private set;
		}

		protected override void AfterCreateProcessInstance(SysTaskProcess sysTaskProcess)
		{
			sysTaskProcess.Name = string.Format("创建活动{0}的分支流程", this.OwnerActivityID);

			//这一步可能是很长时间的操作
			OguDataCollection<IUser> users = this.Template.Resources.ToUsers();

			int index = 0;

			if (this.Template.ExecuteSequence == WfBranchProcessExecuteSequence.SerialInSameProcess)
			{
				sysTaskProcess.Activities.Add(CreateUserActivity(sysTaskProcess, index++, users));

				SysTaskProcessRuntime.Persist();
			}
			else
			{
				foreach (IUser user in users)
				{
					sysTaskProcess.Activities.Add(CreateUserActivity(sysTaskProcess, index++, new IUser[] { user }));

					SysTaskProcessRuntime.Persist();
				}
			}

			if (this.AutoAddExitMaintainingStatusTask)
			{
				IWfProcess process = WfRuntime.GetProcessByActivityID(this.OwnerActivityID);

				sysTaskProcess.Activities.Add(sysTaskProcess.CreateExitMaintainingActivity(process.ID, index++));

				SysTaskProcessRuntime.Persist();
			}
		}

		private SysTaskActivity CreateUserActivity(SysTaskProcess sysTaskProcess, int index, IEnumerable<IUser> users)
		{
			SysTask task = CreateUserTaskInActivity(users);

			SysTaskActivity activity = WfSysTaskActivityHelper.CreateActivity(
				sysTaskProcess,
				index,
				string.Format("创建活动{0}的分支流程的任务流程活动{1}",
				this.OwnerActivityID, index), task);

			SysTaskProcessRuntime.ProcessContext.AffectedActivities.AddOrReplace(activity);

			return activity;
		}

		/// <summary>
		/// 创建任务流程活动中的任务
		/// </summary>
		/// <param name="users"></param>
		/// <returns></returns>
		private SysTask CreateUserTaskInActivity(IEnumerable<IUser> users)
		{
			WfBranchProcessTransferParams transferParams = new WfBranchProcessTransferParams(this.Template, users);

			transferParams.BranchParams.ForEach(bp => bp.ApplicationRuntimeParameters["ProcessName"] = this.Template.DefaultProcessName);

			return StartBranchProcessTask.CreateTask(this.OwnerActivityID, transferParams);
		}
	}
}
