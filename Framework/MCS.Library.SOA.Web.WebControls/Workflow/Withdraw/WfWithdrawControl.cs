using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library;
using MCS.Web.Library.Script;
using MCS.Library.Globalization;
using MCS.Web.Library.MVC;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;

namespace MCS.Web.WebControls
{
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[ClientScriptResource("MCS.Web.WebControls.WfWithdrawControl", "MCS.Web.WebControls.Workflow.Abstract.WfControls.js")]
	[ToolboxData("<{0}:WfWithdrawControl runat=server></{0}:WfWithdrawControl>")]
	public class WfWithdrawControl : WfProcessControlBase
	{
		#region Overridable
		protected override void OnPreRender(EventArgs e)
		{
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "确认要撤回吗？");
			base.OnPreRender(e);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (DesignMode)
				writer.Write("Widthdraw Control");
			else
				base.Render(writer);
		}

		protected override WfExecutorBase CreateExecutor()
		{
			return new WfWithdrawExecutor(WfClientContext.Current.CurrentActivity, WfClientContext.Current.CurrentActivity);
		}

		protected override WfControlAccessbility OnGetAccessbility(WfControlAccessbility defaultAccessbility)
		{
			WfControlAccessbility result = WfControlAccessbility.None;

			if (this.Visible && !this.ReadOnly && this.CanWithdraw())
				result |= WfControlAccessbility.Visible | WfControlAccessbility.Enabled;

			return base.OnGetAccessbility(result);
		}

		protected override void SetTargetControlVisible(Control target)
		{
			target.Visible = (OnGetAccessbility(WfControlAccessbility.None) & WfControlAccessbility.Visible) != WfControlAccessbility.None;

			((IAttributeAccessor)target).SetAttribute("class", "enable");

			if (target.Visible == false)
			{
				((IAttributeAccessor)target).SetAttribute("class", "disable");
			}
		}

		protected override void OnCreateHiddenButton(HiddenButtonWrapper buttonWrapper)
		{
			buttonWrapper.HiddenButton.PopupCaption = Translator.Translate(Define.DefaultCulture, "正在撤回...");
			buttonWrapper.HiddenButton.ProgressMode = SubmitButtonProgressMode.BySteps;
		}
		#endregion

		#region Protected
		protected override void OnSuccess()
		{
			base.OnSuccess();

			WebUtility.ResponseTimeoutScriptBlock(string.Format("if (parent) parent.location.replace('{0}');",
				WfClientContext.Current.ReplaceEntryPathByProcessID()),
				ExtScriptHelper.DefaultResponseTimeout);
		}

		#endregion Protected

		#region Private
		private bool CanWithdraw()
		{
			bool result = false;

			if (CurrentProcess != null)
			{
				IWfActivity currentActivity = CurrentProcess.CurrentActivity;

				if (currentActivity != null)
				{
					//流程状态允许撤回
					result = CurrentProcess.CanWithdraw;

					if (result)
					{
						IWfActivity targetActivity = FindTargetActivity(currentActivity);

						//管理员可以撤回
						if (WfClientContext.Current.InAdminMode)
						{
							result = true;
						}
						else
						{
							//活动的属性设置可以撤回
							result = currentActivity.Descriptor.Properties.GetValue("AllowToBeWithdrawn", true) &&
								targetActivity.Descriptor.Properties.GetValue("AllowWithdraw", true);

							if (result)
							{
								//不是管理员，进行更严格的权限判断(前一个点的操作人是我)
								result = OguBase.IsNullOrEmpty(targetActivity.Operator) == false &&
												string.Compare(targetActivity.Operator.ID, WfClientContext.Current.User.ID, true) == 0;
							}
						}
					}
				}
			}

			return result;
		}

		private static IWfActivity FindTargetActivity(IWfActivity currentActivity)
		{
			IWfActivity result = null;

			int startIndex = currentActivity.Process.ElapsedActivities.Count - 1;

			if (currentActivity.Descriptor.ActivityType == WfActivityType.CompletedActivity)
				startIndex--;

			if (startIndex >= 0)
				result = currentActivity.Process.ElapsedActivities[startIndex];

			return result;
		}
		#endregion Private
	}
}
