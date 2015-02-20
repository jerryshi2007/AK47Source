using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 加签操作的执行器
	/// </summary>
	public class WfAddApproverExecutor : WfMoveToExecutorBase
	{
		private WfAssigneeCollection _Assignees = null;
		private WfAddApproverMode _AddApproverMode = WfAddApproverMode.StandardMode;

		/// <summary>
		/// 加签模式
		/// </summary>
		public WfAddApproverMode AddApproverMode
		{
			get
			{
				return this._AddApproverMode;
			}
			set
			{
				this._AddApproverMode = value;
			}
		}

		public WfAssigneeCollection Assignees
		{
			get
			{
				if (this._Assignees == null)
					this._Assignees = new WfAssigneeCollection();

				return this._Assignees;
			}
		}

		public WfAddApproverExecutor(IWfActivity operatorActivity, IWfActivity targetActivity, WfAssigneeCollection assignees)
			: base(operatorActivity, targetActivity, WfControlOperationType.AddApprover)
		{
			assignees.NullCheck("assingees");

			this.Assignees.CopyFrom(assignees);
		}

		public WfAddApproverExecutor(IWfActivity operatorActivity, IWfActivity targetActivity)
			: this(operatorActivity, targetActivity, targetActivity.Assignees)
		{

		}

		protected override void OnModifyWorkflow(WfExecutorDataContext dataContext)
		{
			(this.Assignees.Count > 0).FalseThrow<WfRuntimeException>("加签的用户数必须大于零");

			IWfProcess process = this.TargetActivity.Process;

			WfRuntime.ProcessContext.BeginChangeActivityChangingContext();

			try
			{
				WfRuntime.ProcessContext.ActivityChangingContext.CreatorInstanceID = OperatorActivity.ID;
				string clonedKey = TargetActivity.Descriptor.AssociatedActivityKey.IsNotEmpty() ?
							TargetActivity.Descriptor.AssociatedActivityKey : TargetActivity.Descriptor.Key;

				IWfActivity templateAct = process.Activities.FindActivityByDescriptorKey(clonedKey);

				//不是仅添加审批人时
				if ((this.AddApproverMode & WfAddApproverMode.AreAssociatedActivities) != WfAddApproverMode.OnlyAddApprover)
					WfRuntime.ProcessContext.ActivityChangingContext.AssociatedActivityKey = clonedKey;

				//生成加签的点
				WfActivityDescriptor addActDesp = CreateAddApproverActivityDescriptor(templateAct.Descriptor);

				IWfActivity addedActivity = this.TargetActivity.Append(addActDesp);

				//当仅添加审批人时
				if ((this.AddApproverMode & WfAddApproverMode.AreAssociatedActivities) == WfAddApproverMode.OnlyAddApprover)
				{
					//将指派人直接赋值给办理人
					addedActivity.Candidates.CopyFrom(this.Assignees);

					IWfActivityDescriptor mainStreamActDesp = templateAct.GetMainStreamActivityDescriptor();

					if (mainStreamActDesp != null)
					{
						WfActivityDescriptor newMSActDesp = CreateAddApproverActivityDescriptor(mainStreamActDesp);

						newMSActDesp.Resources.Clear();

						foreach (IUser user in this.Assignees.ToUsers())
							newMSActDesp.Resources.Add(new WfUserResourceDescriptor(user));

						mainStreamActDesp.Append(newMSActDesp);
					}
				}

				//不是仅添加审批人时
				if ((this.AddApproverMode & WfAddApproverMode.AppendCurrentActivity) != WfAddApproverMode.OnlyAddApprover)
				{
					//Clone当前的点，在加签点后生成再添加当前活动
					IWfActivity foundActivity = process.Activities.FindActivityByDescriptorKey(clonedKey);
					WfActivityDescriptor clonedCurrentActDesp = ((WfActivityDescriptor)foundActivity.Descriptor).Clone() as WfActivityDescriptor;

					//加签按照加签活动模板进行属性设置
					WfActivityBase.ResetPropertiesByDefinedName(addActDesp, "DefaultAddApproverActivityTemplate");

					//if (foundActivity.Descriptor.ActivityType == WfActivityType.InitialActivity)
					//	WfActivityBase.ResetPropertiesByDefinedName(clonedCurrentActDesp, "DefaultAddApproverActivityTemplate");

					clonedCurrentActDesp.AssociatedActivityKey = WfRuntime.ProcessContext.ActivityChangingContext.AssociatedActivityKey;
					clonedCurrentActDesp.ClonedKey = this.TargetActivity.Descriptor.Key;
					clonedCurrentActDesp.IsReturnSkipped = true;

					IWfActivity clonedActivity = addActDesp.Instance.Append(clonedCurrentActDesp);
					clonedActivity.Candidates.CopyFrom(foundActivity.Candidates);
				}

				WfTransferParams tp = new WfTransferParams(addActDesp);

				tp.Assignees.CopyFrom(this.Assignees);

				process.MoveTo(tp);
			}
			finally
			{
				WfRuntime.ProcessContext.RestoreChangeActivityChangingContext();
			}
		}

		private WfActivityDescriptor CreateAddApproverActivityDescriptor(IWfActivityDescriptor templateActDesp)
		{
			WfActivityDescriptor addActDesp = templateActDesp.Clone() as WfActivityDescriptor;

			addActDesp.ActivityType = WfActivityType.NormalActivity;
			addActDesp.ClonedKey = templateActDesp.Key;
			addActDesp.AssociatedActivityKey = WfRuntime.ProcessContext.ActivityChangingContext.AssociatedActivityKey;
			addActDesp.IsReturnSkipped = true;
			addActDesp.Properties.TrySetValue("AutoMaintain", false);

			if ((this.AddApproverMode & WfAddApproverMode.AreAssociatedActivities) == WfAddApproverMode.OnlyAddApprover)
				addActDesp.Name = string.Empty;

			return addActDesp;
		}
	}
}
