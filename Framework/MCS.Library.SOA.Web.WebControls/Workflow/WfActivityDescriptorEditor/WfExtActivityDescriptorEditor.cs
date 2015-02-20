using System;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Collections.Generic;
using MCS.Web.Library.Script;
using MCS.Library.OGUPermission;
using MCS.Web.Library;
using MCS.Library.SOA.DataObjects;

[assembly: WebResource("MCS.Web.WebControls.Workflow.WfActivityDescriptorEditor.WfExtActivityDescriptorEditor.js", "application/x-javascript")]

namespace MCS.Web.WebControls
{
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[ClientScriptResource("MCS.Web.WebControls.WfExtActivityDescriptorEditor",
		"MCS.Web.WebControls.Workflow.WfActivityDescriptorEditor.WfExtActivityDescriptorEditor.js")]
	[DialogContent("MCS.Web.WebControls.Workflow.WfActivityDescriptorEditor.WfExtActivityDescriptorEditor.htm", "MCS.Library.SOA.Web.WebControls")]
	[ToolboxData("<{0}:WfExtActivityDescriptorEditor runat=server></{0}:WfExtActivityDescriptorEditor>")]
	public class WfExtActivityDescriptorEditor : WfActivityDescriptorEditorBase
	{
		private ExtOuUserInputControl userInput = new ExtOuUserInputControl() { InvokeWithoutViewState = true };
		private UserOUGraphControl fakeUserControl = new UserOUGraphControl() { InvokeWithoutViewState = true };

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			if (this.Page.IsCallback)
				EnsureChildControls();

			fakeUserControl.Visible = false;
			fakeUserControl.ID = "fakeUserControl";
			Controls.Add(fakeUserControl);
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("userInputClientID")]
		protected string UserInputClientID
		{
			get
			{
				return this.userInput.ClientID;
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

		protected override void InitUserInputArea(Control container)
		{
			InitUserInput(WebControlUtility.FindControlByHtmlIDProperty(container, "resourceContainer", true));
		}

		protected override void VariablesInitialized(List<VariableObject> variables)
		{
			/*
			if (variables.Exists(v => v.Key == WfVariableDefine.AllAgreeWhenConsignVariableName == false))
				variables.Add(new VariableObject(WfVariableDefine.AllAgreeWhenConsignVariableName, "会签时，所有用户通过后流转到下一步", true));
			*/
		}

		protected override string GetSmallDialogFeature()
		{
			WindowFeature feature = new WindowFeature();

			feature.Width = 580;
			feature.Height = 480;
			feature.Resizable = false;
			feature.ShowStatusBar = false;
			feature.ShowScrollBars = false;
			feature.Center = true;

			return feature.ToDialogFeatureClientString();
		}

		protected override string GetDialogFeature()
		{
			WindowFeature feature = new WindowFeature();

			feature.Width = 580;
			feature.Height = 540;
			feature.Resizable = false;
			feature.ShowStatusBar = false;
			feature.ShowScrollBars = false;
			feature.Center = true;

			return feature.ToDialogFeatureClientString();
		}

		private void InitUserInput(Control container)
		{
			userInput.ID = "userInput";
			userInput.SelectMask = UserControlObjectMask.User | UserControlObjectMask.Sideline;
			userInput.ShowCirculateUsers = this.ShowCirculateUsers;

			if (CurrentActivityDescriptor != null)
				userInput.MultiSelect = AllowMultiUsers;

			if (this.Operation == "update")
			{
				OguDataCollection<IUser> users = CurrentActivityDescriptor.Resources.ToUsers();

				OguDataCollection<IOguObject> selectedObjs = new OguDataCollection<IOguObject>();

				users.ForEach(u => selectedObjs.Add(u));

				userInput.ConsignUsers = selectedObjs;

				OguDataCollection<IUser> circulators = WfVariableDefine.GetCirculateUsers(CurrentActivityDescriptor);

				selectedObjs = new OguDataCollection<IOguObject>();

				circulators.ForEach(u => selectedObjs.Add(u));

				userInput.Circulators = selectedObjs;
			}

			container.Controls.Add(userInput);
		}
	}
}
