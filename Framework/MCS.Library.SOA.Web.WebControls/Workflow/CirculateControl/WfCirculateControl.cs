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

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 送阅控件
	/// </summary>
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[ClientScriptResource("MCS.Web.WebControls.WfCirculateControl", "MCS.Web.WebControls.Workflow.Abstract.WfControls.js")]
	[ToolboxData("<{0}:WfCirculateControl runat=server></{0}:WfCirculateControl>")]
	public class WfCirculateControl : WfProcessControlBase
	{
		private UserSelector userInput = new UserSelector() { InvokeWithoutViewState = true };

		private List<IUser> selectedUsers = new List<IUser>();
		private string opinionText = string.Empty;

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

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			if (Page.IsCallback)
				EnsureChildControls();
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			this.userInput.DialogTitle = Translator.Translate(Define.DefaultCulture, "请选择传阅人");

			this.Controls.Add(this.userInput);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (DesignMode)
				writer.Write("Circulate Control");
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
			return new WfCirculateExecutor(WfClientContext.Current.OriginalActivity, WfClientContext.Current.OriginalActivity, this.selectedUsers);
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

			if (this.Visible && !this.ReadOnly && this.Enabled
				&& WfClientContext.Current.OriginalActivity.ApprovalRootActivity.Descriptor.Properties.GetValue("AllowCirculate", false))
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
			buttonWrapper.HiddenButton.PopupCaption = Translator.Translate(Define.DefaultCulture, "正在传阅...");
		}
	}
}
