using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using MCS.Web.Library.Script;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.MVC;
using MCS.Library.Globalization;
using MCS.Library.OGUPermission;
using MCS.Library.Core;
using MCS.Web.Library;
using System.Web.UI.WebControls;
using System.Web;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 加签控件
	/// </summary>
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[ClientScriptResource("MCS.Web.WebControls.WfAddApproverControl", "MCS.Web.WebControls.Workflow.Abstract.WfControls.js")]
	[ToolboxData("<{0}:WfAddApproverControl runat=server></{0}:WfAddApproverControl>")]
	public class WfAddApproverControl : WfProcessControlBase
	{
		private UserSelector userInput = new UserSelector() { InvokeWithoutViewState = true };
		private List<IUser> selectedUsers = new List<IUser>();
		private string opinionText = string.Empty;

		/// <summary>
		/// 加签模式
		/// </summary>
		[Browsable(true)]
		[DefaultValue(WfAddApproverMode.StandardMode)]
		public WfAddApproverMode AddApproverMode
		{
			get
			{
				return GetPropertyValue("AddApproverMode", WfAddApproverMode.StandardMode);
			}
			set
			{
				SetPropertyValue("AddApproverMode", value);
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("userInputClientID")]
		private string UserInputClientID
		{
			get
			{
				return this.userInput.ClientID;
			}
		}

		[DefaultValue(false)]
		public bool ShowOpinionInput
		{
			get
			{
				return GetPropertyValue("ShowOpinionInput", false);
			}
			set
			{
				SetPropertyValue("ShowOpinionInput", value);
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("activityID")]
		private string ActivityID
		{
			get
			{
				string result = string.Empty;

				if (WfClientContext.Current.OriginalActivity != null)
					result = WfClientContext.Current.OriginalActivity.ID;

				return result;
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			if (Page.IsCallback)
				EnsureChildControls();
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			this.userInput.SelectMask = UserControlObjectMask.User | UserControlObjectMask.Sideline;
			this.userInput.ListMask = UserControlObjectMask.User | UserControlObjectMask.Sideline | UserControlObjectMask.Organization;
			this.userInput.MultiSelect = false;
			//this.userInput.Category = Define.DefaultCategory;
			this.userInput.ShowOpinionInput = this.ShowOpinionInput;

			this.userInput.DialogTitle = Translator.Translate(Define.DefaultCulture, "请选择加签人");

			this.Controls.Add(this.userInput);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (DesignMode)
				writer.Write("AddApprove Control");
			else
				base.Render(writer);
		}

		protected override void LoadClientState(string clientState)
		{
			if (clientState.IsNotEmpty())
			{
				Dictionary<string, object> dict = JSONSerializerExecute.Deserialize<Dictionary<string, object>>(clientState);

				if (dict != null)
				{
					object[] originalUsers = (object[])dict["users"];

					foreach (IUser user in originalUsers)
						this.selectedUsers.Add(user);

					this.opinionText = (string)dict["opinion"];
				}
			}
		}

		protected override WfExecutorBase CreateExecutor()
		{
			WfAssigneeCollection wfac = new WfAssigneeCollection();
			wfac.Add(this.selectedUsers);

			return new WfAddApproverExecutor(WfClientContext.Current.OriginalCurrentActivity, WfClientContext.Current.OriginalCurrentActivity, wfac) { AddApproverMode = this.AddApproverMode };
		}

		protected override void OnSuccess()
		{
			base.OnSuccess();

			WebUtility.ResponseTimeoutScriptBlock("top.$HBRootNS.WfProcessControlBase.close();",
				ExtScriptHelper.DefaultResponseTimeout);
		}

		protected override WfControlAccessbility OnGetAccessbility(WfControlAccessbility defaultAccessbility)
		{
			WfControlAccessbility result = WfControlAccessbility.None;

			if (this.Visible && !this.ReadOnly && WfClientContext.Current.InMoveToMode && this.Enabled
				&& WfClientContext.Current.CurrentActivity.Descriptor.Properties.GetValue("AllowAddApprover", false))
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
			buttonWrapper.HiddenButton.PopupCaption = Translator.Translate(Define.DefaultCulture, "正在加签...");
		}

		private void InitProcessControl(IAttributeAccessor target)
		{
			if (target != null)
			{
				target.SetAttribute("onclick", string.Format("if (!event.srcElement.disabled) $find('{0}').doInternalOperation(); return false;", this.ClientID));

				SetTargetControlVisible((Control)target);
			}
		}
	}
}
