using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Library.Passport;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.WebControls;

namespace MCS.Web.Library.MVC
{
	/// <summary>
	/// 打开表单的前置控制逻辑。包括判断当前用户是否在Acl中，是否处于流转状态以及加锁状态
	/// </summary>
	public class OpenFormControllerOperation : ControllerOperationBase
	{
		public OpenFormControllerOperation()
		{
			IsRelativeForm = AccessTicketManager.IsValidAccessTicket(AccessTicket.DefaultTimeout);
		}

		public IWfProcess Process { get; set; }
		public event IsCurrentUserInAclHandler IsCurrentUserInAcl;

		/// <summary>
		/// 是否是关联的Form，如果是，会绕开权限检查。且设置只读场景
		/// </summary>
		public bool IsRelativeForm { get; set; }

		protected internal override void DoOperation()
		{
			if (IsRelativeForm == false)
			{
				PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("CheckCurrentUserInAcl",
					() => CheckCurrentUserInAcl(WfClientContext.Current.OriginalActivity.Process));
			}

			OnProcessReady(WfClientContext.Current.OriginalActivity.Process);
			OnPrepareCommandState(WfClientContext.Current.OriginalActivity.Process);

			SetLockResult lockResult = null;

			if (WfClientContext.Current.InMoveToMode)
			{
				if (LockConfigSetting.GetConfig().Enabled)
				{
					PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("AddLock", () =>
						{
							lockResult = WfClientContext.Current.AddFormLock(
								WfClientContext.Current.OriginalActivity.Process.ResourceID, WfClientContext.Current.OriginalActivity.ID);
						});
				}
			}

			PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("SetScene",
				() => SetScene(lockResult));

			//需要导航到目标视图
			TransferToTargetView();
		}

		/// <summary>
		/// 当前用户是否在表单的Acl表中
		/// </summary>
		/// <param name="process"></param>
		/// <param name="continueCheck"></param>
		protected virtual void OnIsCurrentUserInAcl(IWfProcess process, ref bool continueCheck)
		{
			if (IsCurrentUserInAcl != null)
				IsCurrentUserInAcl(process, ref continueCheck);
		}

		/// <summary>
		/// 检查当前用户是否在Acl中
		/// </summary>
		/// <param name="process"></param>
		private void CheckCurrentUserInAcl(IWfProcess process)
		{
			bool continueCheck = true;

			OnIsCurrentUserInAcl(process, ref continueCheck);

			if (continueCheck)
			{
				foreach (IUserProcessAclChecker checker in OpenFormSettings.GetConfig().AclCheckers)
				{
					checker.CheckUserInAcl(DeluxeIdentity.CurrentUser, process, ref continueCheck);

					if (continueCheck == false)
						break;
				}
			}
		}

		private void SetScene(SetLockResult lockResult)
		{
			bool isReadOnly = true;

			MCS.Library.SOA.DataObjects.OperationType logOpType = MCS.Library.SOA.DataObjects.OperationType.OpenForm;

			if (lockResult != null && lockResult.Succeed && WfClientContext.Current.InMoveToMode && IsRelativeForm == false)
			{
				isReadOnly = false;
				logOpType = MCS.Library.SOA.DataObjects.OperationType.OpenFormForMove;
			}

			//如果流程已经办结，那么检查结束点的场景来决定是否只读
			if (WfClientContext.Current.CurrentActivity != null && WfClientContext.Current.CurrentActivity.Process.Status == WfProcessStatus.Completed)
			{
				isReadOnly = WfClientContext.Current.CurrentActivity.Descriptor.Scene.IsNullOrEmpty();
			}

			if (isReadOnly)
				SetReadOnlyScene(WfClientContext.Current.OriginalActivity);
			else
				SetSceneByActivity(WfClientContext.Current.CurrentActivity);

			WriteOpenFormLog(WfClientContext.Current.CurrentActivity, logOpType);
		}

		/// <summary>
		/// 写打开表单的日志
		/// </summary>
		/// <param name="currentActivity"></param>
		/// <param name="logOpType"></param>
		private static void WriteOpenFormLog(IWfActivity currentActivity, MCS.Library.SOA.DataObjects.OperationType logOpType)
		{
			if (currentActivity != null)
			{
				UserOperationLog log = UserOperationLog.FromActivity(currentActivity);

				log.OperationType = logOpType;
				log.OperationName = EnumItemDescriptionAttribute.GetDescription(logOpType);

				UserOperationLogAdapter.Instance.Update(log);
			}
		}
	}
}
