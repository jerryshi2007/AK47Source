using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Collections.Specialized;
using MCS.Web.Library;
using MCS.Web.Library.Script;
using MCS.Library.OGUPermission;
using MCS.Library.Core;
using MCS.Library.Passport;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;

[assembly: WebResource("MCS.Web.WebControls.Workflow.WfActivityDescriptorEditor.WfActivityDescriptorEditor.js", "application/x-javascript")]

namespace MCS.Web.WebControls
{
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[ClientScriptResource("MCS.Web.WebControls.WfActivityDescriptorEditor", "MCS.Web.WebControls.Workflow.WfActivityDescriptorEditor.WfActivityDescriptorEditor.js")]
	[DialogContent("MCS.Web.WebControls.Workflow.WfActivityDescriptorEditor.WfActivityDescriptorEditor.htm", "MCS.Library.SOA.Web.WebControls")]
	[ToolboxData("<{0}:WfActivityDescriptorEditor runat=server></{0}:WfActivityDescriptorEditor>")]
	public class WfActivityDescriptorEditor : WfActivityDescriptorEditorBase
	{
		private OuUserInputControl userInput = new OuUserInputControl() { InvokeWithoutViewState = true };
		private OuUserInputControl circulateUserInput = new OuUserInputControl() { InvokeWithoutViewState = true };

		#region properties
		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("userInputClientID")]
		private string UserInputClientID
		{
			get
			{
				return userInput.ClientID;
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("circulateUserInputClientID")]
		private string CirculateUserInputClientID
		{
			get
			{
				return circulateUserInput.ClientID;
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("actNameClientID")]
		protected override string ActNameClientID
		{
			get
			{
				return base.ActNameClientID;
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("allAgreeWhenConsignCheckboxClientID")]
		protected override string AllAgreeWhenConsignCheckboxClientID
		{
			get
			{
				return base.AllAgreeWhenConsignCheckboxClientID;
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("operation")]
		protected override string Operation
		{
			get
			{
				return ControlParams.Operation;
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("currentActivityKey")]
		protected override string CurrentActivityKey
		{
			get
			{
				return base.CurrentActivityKey;
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("maximizeAssigneeCount")]
		protected override int MaximizeAssigneeCount
		{
			get
			{
				return base.MaximizeAssigneeCount;
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("smallDialogUrl")]
		private string SmallDialogUrl
		{
			get
			{
				return GetSmallDialogUrl();
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("smallDialogFeature")]
		private string SmallDialogFeature
		{
			get
			{
				return GetSmallDialogFeature();
			}
		}
		#endregion

		#region
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			if (this.Page.IsCallback)
				EnsureChildControls();
		}

		protected override void InitUserInputArea(Control container)
		{
			InitUserInput(WebControlUtility.FindControlByHtmlIDProperty(container, "resourceContainer", true));
			InitCirculateControls(container);
		}

		private void InitUserInput(Control container)
		{
			userInput.ID = "userInput";
			userInput.SelectMask = UserControlObjectMask.User | UserControlObjectMask.Sideline;

			if (CurrentActivityDescriptor != null)
				userInput.MultiSelect = AllowMultiUsers;

			if (this.Operation == "update")
			{
				OguDataCollection<IOguObject> selectedObjs = new OguDataCollection<IOguObject>();

				if (CurrentActivityDescriptor.Instance != null)
					CurrentActivityDescriptor.Instance.Candidates.ForEach(a => selectedObjs.Add(a.User));

				userInput.SelectedOuUserData = selectedObjs;
			}

			container.Controls.Add(userInput);
		}

		private void InitCirculateControls(Control container)
		{
			Control circulateRow = WebControlUtility.FindControlByHtmlIDProperty(container, "circulateRow", true);

			if (circulateRow != null)
				circulateRow.Visible = this.ShowCirculateUsers;

			if (this.ShowCirculateUsers)
			{
				InitCirculateUserInput(WebControlUtility.FindControlByHtmlIDProperty(container, "circulateContainer", true));
			}
		}

		private void InitCirculateUserInput(Control container)
		{
			circulateUserInput.ID = "circulateUserInput";
			circulateUserInput.SelectMask = UserControlObjectMask.User;

			if (CurrentActivityDescriptor != null)
				circulateUserInput.MultiSelect = AllowMultiUsers;

			if (this.Operation == "update")
			{
				OguDataCollection<IUser> users = WfVariableDefine.GetCirculateUsers(CurrentActivityDescriptor);

				OguDataCollection<IOguObject> selectedObjs = new OguDataCollection<IOguObject>();

				users.ForEach(u => selectedObjs.Add(u));

				circulateUserInput.SelectedOuUserData = selectedObjs;
			}

			container.Controls.Add(circulateUserInput);
		}
		#endregion
	}
}
