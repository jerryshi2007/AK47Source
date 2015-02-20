using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfConsignExecutor : WfMoveToExecutorBase
	{
		private WfAssigneeCollection _Assignees = null;

		public WfAssigneeCollection Assignees
		{
			get
			{
				if (this._Assignees == null)
					this._Assignees = new WfAssigneeCollection();

				return this._Assignees;
			}
		}

		public IEnumerable<IUser> ConsignUsers
		{
			get;
			private set;
		}

		public IEnumerable<IUser> CirculateUsers
		{
			get;
			private set;
		}

		public WfBranchProcessBlockingType BlockingType
		{
			get;
			private set;
		}

		public WfBranchProcessExecuteSequence Sequence
		{
			get;
			private set;
		}

		public WfConsignExecutor(IWfActivity operatorActivity,
								IWfActivity targetActivity,
								WfAssigneeCollection assignees,
								IEnumerable<IUser> consignUsers,
								IEnumerable<IUser> circulateUsers,
								WfBranchProcessBlockingType blockingType,
								WfBranchProcessExecuteSequence sequence)
			: base(operatorActivity, targetActivity, WfControlOperationType.Consign)
		{
			assignees.NullCheck("assignees");
			consignUsers.NullCheck("users");

			(consignUsers.Count() > 0).FalseThrow<WfRuntimeException>("参与会签的用户数必须大于零");

			this.Assignees.CopyFrom(assignees);
			this.ConsignUsers = consignUsers;
			this.CirculateUsers = circulateUsers;
			this.BlockingType = blockingType;
			this.Sequence = sequence;
		}

		protected override void OnModifyWorkflow(WfExecutorDataContext dataContext)
		{
            this.TargetActivity.Process.Committed = true;

			IWfProcess process = TargetActivity.Process;

			WfRuntime.ProcessContext.BeginChangeActivityChangingContext();
			try
			{
				WfRuntime.ProcessContext.ActivityChangingContext.CreatorInstanceID = TargetActivity.ID;
				WfRuntime.ProcessContext.ActivityChangingContext.AssociatedActivityKey =
					TargetActivity.Descriptor.AssociatedActivityKey.IsNotEmpty() ?
						TargetActivity.Descriptor.AssociatedActivityKey : TargetActivity.Descriptor.Key;

				//string activityKey = process.Descriptor.FindNotUsedActivityKey();

				//WfActivityDescriptor actDesp = new WfActivityDescriptor(activityKey);

				//actDesp.Name = "会签";
				var toReturnTrans = process.CurrentActivity.Descriptor.ToTransitions.FindAll(t => t.IsBackward == true);

				WfActivityDescriptor actDesp = process.CurrentActivity.Descriptor.Clone() as WfActivityDescriptor;
				actDesp.ActivityType = WfActivityType.NormalActivity;
				actDesp.Properties.SetValue("AutoMoveAfterPending", false);
				actDesp.ClonedKey = process.CurrentActivity.Descriptor.Key;
				actDesp.BranchProcessTemplates.Clear();
				process.CurrentActivity.Append(actDesp);

				foreach (WfTransitionDescriptor t in toReturnTrans)
				{
					WfTransitionDescriptor trans = t.Clone() as WfTransitionDescriptor;
					if (t.FromActivityKey == t.ToActivityKey)
						trans.JoinActivity(actDesp, actDesp);
					else
						trans.JoinActivity(actDesp, t.ToActivity);

					actDesp.ToTransitions.Add(trans);
				}

				//添加子流程 
				WfTransferParams tp = new WfTransferParams(actDesp);
				tp.Assignees.CopyFrom(Assignees);

				tp.BranchTransferParams.Add(new WfBranchProcessTransferParams(
					WfTemplateBuilder.CreateDefaultConsignTemplate(
						"WfConsignProcessTemplateDescriptorKey",
						this.Sequence,
						this.BlockingType,
						this.ConsignUsers)));

				if (this.CirculateUsers.Count<IUser>() > 0)
				{
					tp.BranchTransferParams.Add(new WfBranchProcessTransferParams(
						WfTemplateBuilder.CreateDefaultCirculationTemplate(
							"WfCirculationProcessTemplateDescriptorKey",
							this.CirculateUsers)));
				}

				WfRuntime.ProcessContext.AfterStartupBranchProcess += new WfAfterStartupBranchProcessHandler(WfActivityRelativeExecutorBase.AfterStartupBranchProcess);
				process.MoveTo(tp);
			}
			finally
			{
				WfRuntime.ProcessContext.RestoreChangeActivityChangingContext();
			}
		}

		private static IWfBranchProcessTemplateDescriptor CreateConsignTemplate(WfBranchProcessExecuteSequence execSequence, WfBranchProcessBlockingType blockingType)
		{
			string key = "WfBranchProcessTemplateDescriptorKey";

			WfBranchProcessTemplateDescriptor template = new WfBranchProcessTemplateDescriptor(key);

			template.BranchProcessKey = WfProcessDescriptorManager.DefaultConsignProcessKey;
			template.ExecuteSequence = execSequence;
			template.BlockingType = blockingType;

			return template;
		}
	}
}
